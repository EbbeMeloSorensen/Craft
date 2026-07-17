using System.Collections;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Math;
using Craft.DataStructures.Geometry;
using Craft.Simulation.Bodies;
using Craft.Simulation.Boundaries;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Simulation;
using Craft.Simulation.BodyStates;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class SimulationLaboratoryViewModel : ViewModelBase, IFrameAware
    {
        private RelayCommand _startAnimationCommand;
        private RelayCommand _pauseAnimationCommand;
        private string _startOrResumeButtonText = "Start";
        private GeometryDataStore _geometryDataStore;

        public Engine.Engine Engine { get; }

        public SceneListViewModel SceneListViewModel { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public RelayCommand StartOrResumeAnimationCommand
        {
            get
            {
                return _startAnimationCommand ?? (_startAnimationCommand = new RelayCommand(StartAnimation, CanStartAnimation));
            }
        }

        public RelayCommand PauseAnimationCommand
        {
            get
            {
                return _pauseAnimationCommand ?? (_pauseAnimationCommand = new RelayCommand(PauseAnimation, CanPauseAnimation));
            }
        }

        public string StartOrResumeButtonText
        {
            get => _startOrResumeButtonText;
            set
            {
                _startOrResumeButtonText = value;
                RaisePropertyChanged();
            }
        }

        public SimulationLaboratoryViewModel()
        {
            Engine = new Engine.Engine(new DummyLogger());

            SceneListViewModel = new SceneListViewModel();

            GeometryViewModel = new GeometryViewModel()
            {
                ShowCoordinateSystem = true,
                LockAspectRatio = true,
                DampFocusShifts = false
            };

            SceneListViewModel.PropertyChanged += SceneListViewModel_PropertyChanged;
            GeometryViewModel.PropertyChanged += GeometryViewModel_PropertyChanged;
            Engine.CurrentStateChanged += Engine_CurrentStateChanged;
        }

        public void OnFrame(
            TimeSpan time,
            double dt)
        {
            // Bemærk, at man ikke bruger parametrene her
            Engine.UpdateModel();
        }

        public void HandleClosing()
        {
            Engine.HandleClosing();
        }

        private void StartAnimation()
        {
            if (Engine.EngineCore.Scene == null)
            {
                Engine.EngineCore.Scene = SceneListViewModel.ActiveScene;
                Engine.EngineCore.SpawnNewThread();
            }

            Engine.StartOrResumeAnimation();
            RefreshButtons();
        }

        private bool CanStartAnimation()
        {
            return SceneListViewModel.ActiveScene != null &&
                   !Engine.AnimationRunning;
        }

        private void PauseAnimation()
        {
            Engine.PauseAnimation();
            StartOrResumeButtonText = "Resume";
            RefreshButtons();
        }

        private bool CanPauseAnimation()
        {
            return SceneListViewModel.ActiveScene != null &&
                   Engine.AnimationRunning;
        }

        private void SceneListViewModel_PropertyChanged(
            object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SceneListViewModel.ActiveScene))
            {
                if (Engine.EngineCore.Scene != null)
                {
                    Engine.ResetEngine();
                    Engine.EngineCore.Scene = null;
                    StartOrResumeButtonText = "Start";
                }

                if (SceneListViewModel.ActiveScene == null)
                {
                    GeometryViewModel.ClearLayer(true);
                    GeometryViewModel.ClearLayer(false);
                    _geometryDataStore = null;
                }
                else
                {
                    InitializeAnimation(SceneListViewModel.ActiveScene);
                }
            }
        }

        private void GeometryViewModel_PropertyChanged(
            object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GeometryViewModel.WorldWindowExpanded))
            {
                UpdateStaticGeometricObjects();
            }
        }

        private void Engine_CurrentStateChanged(
            object? sender,
            Engine.CurrentStateChangedEventArgs e)
        {
            if (SceneListViewModel.ActiveScene == null)
            {
                return;
            }

            UpdateDynamicGeometricObjects(e.State);

            if (SceneListViewModel.ActiveScene.ViewMode == SceneViewMode.FocusOnFirstBody)
            {
                UpdateFocus(e.State.BodyStates.First().Position);
            }
        }

        private void UpdateDynamicGeometricObjects(
            State state)
        {
            var geometricObjects = new ArrayList();

            state.BodyStates.ForEach(bs =>
            {
                switch (bs.Body)
                {
                    case CircularBody circularBody:
                        geometricObjects.Add(new Circle2D(
                            new Point2D(bs.Position.X, bs.Position.Y),
                            circularBody.Radius));
                        break;
                    case BodyDoor bodyDoor:
                        var bodyStateDoor = bs as BodyStateDoor;
                        var angle = (bodyStateDoor.PercentageOpen) * 0.5 * System.Math.PI / 100;

                        var doorAsVector = new Vector2D(
                            bodyDoor.Point2.X - bodyDoor.Point1.X,
                            bodyDoor.Point2.Y - bodyDoor.Point1.Y);

                        var doorWidth = doorAsVector.Length;
                        var hatted = doorAsVector.Hat();

                        if (!bodyStateDoor.OpenClockWise)
                        {
                            hatted = -hatted;
                        }

                        var pt2_x = 
                            bodyDoor.Point1.X +
                            System.Math.Cos(angle) * doorAsVector.X +
                            System.Math.Sin(angle) * hatted.X;

                        var pt2_y =
                            bodyDoor.Point1.Y +
                            System.Math.Cos(angle) * doorAsVector.Y +
                            System.Math.Sin(angle) * hatted.Y;

                        geometricObjects.Add(new LineSegment2D(
                            new Point2D(
                                bodyDoor.Point1.X,
                                bodyDoor.Point1.Y),
                            new Point2D(
                                pt2_x,
                                pt2_y)));
                        break;
                }
            });

            GeometryViewModel.ReplaceDynamicGeometryLayer(geometricObjects);
        }

        private void UpdateStaticGeometricObjects()
        {
            GeometryViewModel.ClearLayer(false);

            if (_geometryDataStore != null)
            {
                GeometryViewModel.AddStaticGeometryLayer(
                    _geometryDataStore.Query(GeometryViewModel.WorldWindowExpanded));
            }
        }

        private void UpdateFocus(
            Vector2D focus)
        {
            GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
            {
                WorldPoint = new Point(focus.X, focus.Y),
                ViewportRatio = new System.Windows.Size(0.5, 0.5)
            };
        }

        private void InitializeAnimation(
            Scene scene)
        {
            var staticGeometryObjects = new List<object>();

            scene.Boundaries.ForEach(boundary =>
            {
                if (!boundary.Visible) return;

                switch (boundary)
                {
                    case HorizontalLineSegment horizontalLineSegment:
                        staticGeometryObjects.Add(new LineSegment2D(
                            new Point2D(horizontalLineSegment.X0, horizontalLineSegment.Y),
                            new Point2D(horizontalLineSegment.X1, horizontalLineSegment.Y)));
                        break;
                    case VerticalLineSegment verticalLineSegment:
                        staticGeometryObjects.Add(new LineSegment2D(
                            new Point2D(verticalLineSegment.X, verticalLineSegment.Y0),
                            new Point2D(verticalLineSegment.X, verticalLineSegment.Y1)));
                        break;
                    case LineSegment lineSegment:
                        staticGeometryObjects.Add(new LineSegment2D(
                            new Point2D(lineSegment.Point1.X, lineSegment.Point1.Y),
                            new Point2D(lineSegment.Point2.X, lineSegment.Point2.Y)));
                        break;
                    case BoundaryPoint boundaryPoint:
                        staticGeometryObjects.Add(
                            new Point2D(boundaryPoint.Point.X, boundaryPoint.Point.Y));
                        break;
                    case CircularBoundary circularBoundary:
                        staticGeometryObjects.Add(new Circle2D(
                            new Point2D(circularBoundary.Center.X, circularBoundary.Center.Y),
                            circularBoundary.Radius));
                        break;
                    default:
                        throw new ArgumentException();
                }
            });

            var boundingBoxes = staticGeometryObjects.Select(geometryObject =>
            {
                return geometryObject switch
                {
                    Point2D point => point.ComputeBoundingBox(),
                    LineSegment2D lineSegment => lineSegment.ComputeBoundingBox(),
                    Circle2D circle => circle.ComputeBoundingBox(),
                    _ => throw new InvalidOperationException(),
                };
            });

            if (boundingBoxes.Any())
            {
                _geometryDataStore = new GeometryDataStore(
                    new BoundingBox(
                        boundingBoxes.Min(b => b.MinX),
                        boundingBoxes.Max(b => b.MaxX),
                        boundingBoxes.Min(b => b.MinY),
                        boundingBoxes.Max(b => b.MaxY)),
                    8);
            }
            else
            {
                _geometryDataStore = new GeometryDataStore(
                    new BoundingBox(-1, 1, -1, 1),
                    8);
            }

            staticGeometryObjects.ForEach(_geometryDataStore.AddStaticGeometryObject);

            var initialWorldWindowFocus = scene.InitialWorldWindowFocus();
            var initialWorldWindowSize = scene.InitialWorldWindowSize();

            GeometryViewModel.RequestedWorldWindow = new BoundingBox(
                initialWorldWindowFocus.X - initialWorldWindowSize.Width / 2,
                initialWorldWindowFocus.X + initialWorldWindowSize.Width / 2,
                initialWorldWindowFocus.Y - initialWorldWindowSize.Height / 2,
                initialWorldWindowFocus.Y + initialWorldWindowSize.Height / 2);

            UpdateStaticGeometricObjects();
            UpdateDynamicGeometricObjects(scene.InitialState);

            if (scene.ViewMode == SceneViewMode.FocusOnFirstBody)
            {
                UpdateFocus(scene.InitialState.BodyStates.First().Position);
            }

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            StartOrResumeAnimationCommand.RaiseCanExecuteChanged();
            PauseAnimationCommand.RaiseCanExecuteChanged();
        }
    }
}
