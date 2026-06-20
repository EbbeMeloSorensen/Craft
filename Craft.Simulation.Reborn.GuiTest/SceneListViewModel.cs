using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class SceneListViewModel : ViewModelBase
    {
        private Dictionary<string, Scene> _sceneDictionary;
        private Scene _activeScene;

        public ObservableCollection<Scene> Scenes { get; }

        public Scene ActiveScene
        {
            get { return _activeScene; }
            set
            {
                _activeScene = value;
                RaisePropertyChanged();
            }
        }

        public SceneListViewModel()
        {
            _sceneDictionary = new Dictionary<string, Scene>();
            Scenes = new ObservableCollection<Scene>();

            AddScene(GenerateScene1());
            AddScene(GenerateScene2());
            AddScene(GenerateScene3(true, 10, 10));
        }

        private void AddScene(
            Scene scene)
        {
            if (string.IsNullOrEmpty(scene.Name) ||
                _sceneDictionary.ContainsKey(scene.Name))
            {
                throw new InvalidOperationException(
                    "A scene must have a unique name in order to be included in the collection");
            }

            _sceneDictionary[scene.Name] = scene;
            Scenes.Add(scene);
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
    }
}
