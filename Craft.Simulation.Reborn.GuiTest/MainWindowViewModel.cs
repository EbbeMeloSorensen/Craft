using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.DataStructures.Geometry;
using Craft.Logging;
using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Simulation;

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
                DampFocusShifts = false,
                TimeAxisMode = false,
                FocusShiftDamping = 5.0
            };

            _scene = GenerateScene();

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
        }

        public void HandleClosing()
        {
            Engine.HandleClosing();
        }

        private Scene GenerateScene()
        {
            var ballRadius = 0.125;
            var initialBallPosition = new Vector2D(1, -0.125);
            var initialBallVelocity = new Vector2D(2, 0);
            var affectedByGravity = true;
            var affectedByBoundaries = true;

            var initialState = new State();

            var ball = new CircularBody(1, ballRadius, 1, affectedByGravity, affectedByBoundaries);
            initialState.AddBodyState(new BodyState(ball, initialBallPosition) { NaturalVelocity = initialBallVelocity });

            var name = "Simple Game";
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

            scene.AddBoundary(new HalfPlane(new Vector2D(3, -0.3), new Vector2D(-1, 0)));
            scene.AddBoundary(new HalfPlane(new Vector2D(3, 1), new Vector2D(0, -1)));
            scene.AddBoundary(new HalfPlane(new Vector2D(-1, 1), new Vector2D(1, 0)));

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
    }
}
