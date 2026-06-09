using Craft.DataStructures.Geometry;
using Craft.Logging;
using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Simulation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Point = System.Windows.Point;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class MainWindowViewModel : ViewModelBase, IFrameAware
    {
        private RelayCommand _startAnimationCommand;
        private RelayCommand _pauseAnimationCommand;
        private string _startResumeButtonText = "Start";
        private GeometryDataStore _geometryDataStore;
        private Scene _scene;

        public Engine.Engine Engine { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public RelayCommand StartAnimationCommand
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

        public string StartResumeButtonText
        {
            get => _startResumeButtonText;
            set
            {
                _startResumeButtonText = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Engine = new Engine.Engine(new DummyLogger());

            Engine.CurrentStateChanged += Engine_CurrentStateChanged;

            //_scene = GenerateScene1();
            //_scene = GenerateScene2();
            //_scene = GenerateScene3(true, 1, 1); // The bigger values, the more computationally intensive the scene is
            //_scene = GenerateScene3(true, 3, 3); // The bigger values, the more computationally intensive the scene is
            _scene = GenerateScene3(true, 10, 10); // The bigger values, the more computationally intensive the scene is
            //_scene = GenerateScene3(true, 50, 50); // The bigger values, the more computationally intensive the scene is
            //_scene = GenerateScene3(true, 300, 300); // The bigger values, the more computationally intensive the scene is
            // (Det tager lang tid at loade med store værdier såsom 300 x 300, men når først det er loadet, kører det ret hurtigt)

            var staticGeometryObjects = new List<object>();

            _scene.Boundaries.ForEach(boundary =>
            {
                if (!boundary.Visible) return;

                switch (boundary)
                {
                    case HorizontalLineSegment horizontalLineSegment:
                        staticGeometryObjects.Add(new LineModel
                        {
                            P1 = new Point(horizontalLineSegment.X0, horizontalLineSegment.Y),
                            P2 = new Point(horizontalLineSegment.X1, horizontalLineSegment.Y)
                        });
                        break;
                    case VerticalLineSegment verticalLineSegment:
                        staticGeometryObjects.Add(new LineModel
                        {
                            P1 = new Point(verticalLineSegment.X, verticalLineSegment.Y0),
                            P2 = new Point(verticalLineSegment.X, verticalLineSegment.Y1)
                        });
                        break;
                    case LineSegment lineSegment:
                        staticGeometryObjects.Add(new LineModel
                        {
                            P1 = new Point(lineSegment.Point1.X, lineSegment.Point1.Y),
                            P2 = new Point(lineSegment.Point2.X, lineSegment.Point2.Y)
                        });
                        break;
                    default:
                        throw new ArgumentException();
                }
            });

            var boundingBoxes = staticGeometryObjects.Select(geometryObject =>
            {
                return geometryObject switch
                {
                    LineModel line => line.ComputeBoundingBox(),
                    _ => throw new InvalidOperationException(),
                };
            });

            _geometryDataStore = new GeometryDataStore(
                new BoundingBox(
                    boundingBoxes.Min(b => b.MinX),
                    boundingBoxes.Max(b => b.MaxX),
                    boundingBoxes.Min(b => b.MinY),
                    boundingBoxes.Max(b => b.MaxY)),
                    8);

            staticGeometryObjects.ForEach(_geometryDataStore.AddStaticGeometryObject);

            //GeometryViewModel = new GeometryViewModel(_geometryDataStore)
            GeometryViewModel = new GeometryViewModel()
            {
                ShowCoordinateSystem = true,
                LockAspectRatio = true,
                DampFocusShifts = false
            };

            GeometryViewModel.PropertyChanged += GeometryViewModel_PropertyChanged;

            Engine.EngineCore.Scene = _scene;
        }

        public void OnFrame(
            TimeSpan time,
            double dt)
        {
            // Bemærk, at man ikke bruger parametrene her
            Engine.UpdateModel();
        }

        public void HandleLoaded()
        {
            var initialWorldWindowFocus = _scene.InitialWorldWindowFocus();
            var initialWorldWindowSize = _scene.InitialWorldWindowSize();

            GeometryViewModel.RequestedWorldWindow = new BoundingBox(
                initialWorldWindowFocus.X - initialWorldWindowSize.Width / 2,
                initialWorldWindowFocus.X + initialWorldWindowSize.Width / 2,
                initialWorldWindowFocus.Y - initialWorldWindowSize.Height / 2,
                initialWorldWindowFocus.Y + initialWorldWindowSize.Height / 2);

            var initialState = Engine.EngineCore.SpawnNewThread();

            UpdateGeometricObjects(initialState);

            if (_scene.ViewMode == SceneViewMode.FocusOnFirstBody)
            {
                UpdateFocus(_scene.InitialState.BodyStates.First().Position);
            }
        }

        public void HandleClosing()
        {
            Engine.HandleClosing();
        }

        private Scene GenerateScene1()
        {
            var ballRadius = 0.125;
            var initialBallPosition = new Vector2D(1, -0.125);
            var initialBallVelocity = new Vector2D(2, 0);
            var affectedByGravity = true;
            var affectedByBoundaries = true;

            var initialState = new State();

            var ball = new CircularBody(1, ballRadius, 1, affectedByGravity, affectedByBoundaries);
            initialState.AddBodyState(new BodyState(ball, initialBallPosition) { NaturalVelocity = initialBallVelocity });

            var name = "Auto: Bouncing Ball";
            var standardGravity = 9.82;
            var initialWorldWindowUpperLeft = new Point2D(-1.4, -1.3);
            var initialWorldWindowLowerRight = new Point2D(5, 3);
            var gravitationalConstant = 0.0;
            var coefficientOfFriction = 0.0;
            var timeFactor = 1.0;
            var handleBoundaryCollisions = true;
            var handleBodyCollisions = false;
            var deltaT = 0.001;

            var scene = new Scene(
                name,
                initialWorldWindowUpperLeft,
                initialWorldWindowLowerRight,
                initialState,
                standardGravity,
                gravitationalConstant,
                coefficientOfFriction,
                timeFactor,
                handleBoundaryCollisions,
                handleBodyCollisions,
                deltaT);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            scene.AddRectangularBoundary(-1, 3, -0.3, 2, false);

            scene.InitializeBoundaryDataStore();

            return scene;
        }

        private Scene GenerateScene2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1.7))
            {
                Orientation = 0.5 * System.Math.PI
            });

            var handleBoundaryCollisions = true;

            var scene = new Scene("Interactive: Exploration", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, handleBoundaryCollisions, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First() as BodyStateClassic;
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += System.Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= System.Math.PI;
                }

                var newArtificialSpeed = 0.0;

                if (keyboardState.UpArrowDown)
                {
                    newArtificialSpeed += 1.5;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 1.5;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

                if (System.Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    System.Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            scene.AddRectangularBoundary(-1, 3, -0.3, 2, false);
            scene.AddRectangularBoundary(-0.2, 2.2, 0.6, 1.1, false);

            scene.InitializeBoundaryDataStore();

            return scene;
        }

        private Scene GenerateScene3(
            bool handleBoundaryCollisions,
            int rows,
            int cols)
        {
            var initialState = new State();

            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.2, 1, false), new Vector2D(1.5, 0.5))
            {
                Orientation = 0.5 * System.Math.PI
            });

            var scene = new Scene(
                "Interactive: Maze",
                new Point2D(-1.4, -1.3),
                new Point2D(5, 3),
                initialState,
                0,
                0,
                0,
                1,
                handleBoundaryCollisions,
                false,
                0.005,
                SceneViewMode.FocusOnFirstBody);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First() as BodyStateClassic;
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += System.Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= System.Math.PI;
                }

                var newArtificialSpeed = 0.0;

                if (keyboardState.UpArrowDown)
                {
                    newArtificialSpeed += 3.0;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 3.0;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

                if (System.Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    System.Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            var halfWidth = 0.5;

            for (var r = 0; r < rows; r++)
            {
                var y = -2.0 * r - 0.5;

                for (var c = 0; c < cols; c++)
                {
                    var x = 2.0 * c + 0.5;

                    scene.AddRectangularBoundary(
                        x - halfWidth,
                        x + halfWidth,
                        y - halfWidth,
                        y + halfWidth,
                        (r + c) % 2 == 0);
                }
            }

            scene.InitializeBoundaryDataStore();

            return scene;
        }

        private void StartAnimation()
        {
            Engine.StartOrResumeAnimation();
            StartAnimationCommand.RaiseCanExecuteChanged();
            PauseAnimationCommand.RaiseCanExecuteChanged();
        }

        private bool CanStartAnimation()
        {
            return !Engine.AnimationRunning;
        }

        private void PauseAnimation()
        {
            Engine.PauseAnimation();
            StartResumeButtonText = "Resume";
            StartAnimationCommand.RaiseCanExecuteChanged();
            PauseAnimationCommand.RaiseCanExecuteChanged();
        }

        private bool CanPauseAnimation()
        {
            return Engine.AnimationRunning;
        }

        private void Engine_CurrentStateChanged(
            object? sender,
            Engine.CurrentStateChangedEventArgs e)
        {
            UpdateGeometricObjects(e.State);

            if (_scene.ViewMode == SceneViewMode.FocusOnFirstBody)
            {
                UpdateFocus(e.State.BodyStates.First().Position);
            }
        }

        private void GeometryViewModel_PropertyChanged(
            object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GeometryViewModel.WorldWindowExpanded))
            {
                var geometricObjects = _geometryDataStore.Query(GeometryViewModel.WorldWindowExpanded);

                GeometryViewModel.ReplaceStaticGeometryLayer(
                    geometricObjects);
            }
        }

        private void UpdateGeometricObjects(
            State state)
        {
            var geometricObjects = state.BodyStates.Select(bs => new CircleModel
            {
                Center = new Point(bs.Position.X, bs.Position.Y),
                Radius = (bs.Body as CircularBody)!.Radius
            });

            GeometryViewModel.ReplaceDynamicGeometryLayer(geometricObjects);
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
    }
}
