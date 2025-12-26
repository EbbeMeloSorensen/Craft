using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Craft.Simulation.GuiTest.Tab1
{
    public class BouncingBallViewModel : ViewModelBase
    {
        private SceneViewController _sceneViewController;
        private RelayCommand _startAnimationCommand;
        private RelayCommand _pauseAnimationCommand;
        private string _startResumeButtonText = "Start";

        public Engine.Engine Engine { get; }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

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

        public BouncingBallViewModel()
        {
            Engine = new Engine.Engine(null);

            GeometryEditorViewModel = new GeometryEditorViewModel(1)
            {
                UpdateModelCallBack = Engine.UpdateModel
            };

            _sceneViewController = new SceneViewController(Engine, GeometryEditorViewModel);

            var scene = GenerateScene();

            GeometryEditorViewModel.InitializeWorldWindow(
                scene.InitialWorldWindowFocus(),
                scene.InitialWorldWindowSize(),
                false);

            _sceneViewController.ActiveScene = scene;
            //Engine.StartOrResumeAnimation();
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
    }
}
