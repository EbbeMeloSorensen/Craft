using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Math;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
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

            _geometryDataStore = new GeometryDataStore();

            GeometryViewModel = new GeometryViewModel(_geometryDataStore)
            {
                ShowCoordinateSystem = true,
                LockAspectRatio = true,
                DampFocusShifts = false
            };

            //_scene = GenerateScene1();
            //_scene = GenerateScene2();
            _scene = GenerateScene3();

            _scene.Boundaries.ForEach(boundary =>
            {
                if (!boundary.Visible) return;

                switch (boundary)
                {
                    case HorizontalLineSegment horizontalLineSegment:
                        _geometryDataStore.AddStaticGeometryObject(new LineModel
                        {
                            P1 = new Point(horizontalLineSegment.X0, horizontalLineSegment.Y),
                            P2 = new Point(horizontalLineSegment.X1, horizontalLineSegment.Y)
                        });
                        break;
                    case VerticalLineSegment verticalLineSegment:
                        _geometryDataStore.AddStaticGeometryObject(new LineModel
                        {
                            P1 = new Point(verticalLineSegment.X, verticalLineSegment.Y0),
                            P2 = new Point(verticalLineSegment.X, verticalLineSegment.Y1)
                        });
                        break;
                    default:
                        throw new ArgumentException();
                }
            });

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
            UpdateFocus(_scene.InitialState.BodyStates.First().Position);
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
                handleBodyCollisions,
                deltaT);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            scene.AddRectangularBoundary(-1, 3, -0.3, 2);

            return scene;
        }

        private Scene GenerateScene2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1.7))
            {
                Orientation = 0.5 * System.Math.PI
            });

            var scene = new Scene("Interactive: Exploration", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, false, 0.005);

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

            scene.AddRectangularBoundary(-1, 3, -0.3, 2);
            scene.AddRectangularBoundary(-0.2, 2.2, 0.6, 1.1);
            return scene;
        }

        private Scene GenerateScene3()
        {
            var initialState = new State();

            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.2, 1, false), new Vector2D(1.5, 0.5))
            {
                Orientation = 0.5 * System.Math.PI
            });

            var scene = new Scene("Interactive: Maze", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, false, 0.005);

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

            var rows = 30;
            var cols = 30;
            var halfWidth = 0.5;

            for (var r = 0; r < rows; r++)
            {
                var y = -2.0 * r - 0.5;

                for (var c = 0; c < cols; c++)
                {
                    var x = 2.0 * c + 0.5;

                    scene.AddRectangularBoundary(x - halfWidth, x + halfWidth, y - halfWidth, y + halfWidth);
                }
            }

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
            UpdateFocus(e.State.BodyStates.First().Position);
        }

        private void UpdateGeometricObjects(
            State state)
        {
            var geometricObjects = state.BodyStates.Select(bs => new CircleModel
            {
                Center = new System.Windows.Point(bs.Position.X, bs.Position.Y),
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
