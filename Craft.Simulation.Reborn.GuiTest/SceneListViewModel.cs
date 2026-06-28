using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO;

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

            AddScene(GenerateSceneBouncingBall());
            AddScene(GenerateSceneExploringRoom());
            AddScene(GenerateSceneExploringMaze(true, 10, 10));
            AddScene(GenerateSceneNewtonsCradle1());
            AddScene(GenerateSceneNewtonsCradle2());
            AddScene(GenerateSceneNewtonsCradle3());
            AddScene(GenerateSceneNewtonsCradle4());
            AddScene(GenerateSceneBouncingBallsOnALine1());
            AddScene(GenerateSceneBouncingBallsOnALine2());
            AddScene(GenerateScenePoolTableWithOneBall());
            AddScene(GenerateScenePoolTableWithTwoBalls());
            AddScene(GenerateScenePoolTableWithManyBalls());
            AddScene(GenerateScenePoolTableWithOneBallAndThreeBoundaryPoints());
            AddScene(GenerateScenePoolTableWithTwoBallsAnd1LineSegment());
            AddScene(GenerateScenePoolTableWith1BallAnd1LineSegment());
            AddScene(GenerateSceneBallTrain1());
            AddScene(GenerateSceneBallTrain2());
            AddScene(GenerateSceneBallTrain3());
            AddScene(GenerateSceneBodyFollowingPath());
            AddScene(GenerateSceneBallInteraction1());
            AddScene(GenerateSceneBallInteraction2());
            AddScene(GenerateSceneBallInteraction3());
            AddScene(GenerateSceneBallInteraction4());
            AddScene(GenerateSceneBallInteraction5());
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

        private Scene GenerateSceneBouncingBall()
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

        private Scene GenerateSceneExploringRoom()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1.7))
            {
                Orientation = 0.25 * System.Math.PI
            });

            var handleBoundaryCollisions = true;

            var scene = new Scene("Interactive: Exploring room", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, handleBoundaryCollisions, false, 0.005);

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
            scene.AddRectangularBoundary(-1, 3, -0.3, 2, false);
            scene.AddRectangularBoundary(-0.2, 2.2, 0.6, 1.1, false);

            scene.InitializeBoundaryDataStore();

            return scene;
        }

        private Scene GenerateSceneExploringMaze(
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
                "Interactive: Exploring maze",
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

        private static Scene GenerateSceneNewtonsCradle1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0)) { NaturalVelocity = new Vector2D(3, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.1, 1, true), new Vector2D(1, 0)));

            var scene = new Scene("Auto: Newtons cradle I", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateSceneNewtonsCradle2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0)) { NaturalVelocity = new Vector2D(3, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.1, 1, true), new Vector2D(1, 0)));
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(3, 0.1, 1, true), new Vector2D(1.2, 0)));

            var scene = new Scene("Auto: Newtons cradle II", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateSceneNewtonsCradle3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0)) { NaturalVelocity = new Vector2D(3, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.1, 1, true), new Vector2D(1, 0)));
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(3, 0.1, 1, true), new Vector2D(1.2, 0)));
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(4, 0.1, 1, true), new Vector2D(1.4, 0)));
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(5, 0.1, 1, true), new Vector2D(1.6, 0)));

            var scene = new Scene("Auto: Newtons cradle III", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateSceneNewtonsCradle4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0)) { NaturalVelocity = new Vector2D(3, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.1, 1, true), new Vector2D(0.2, 0)) { NaturalVelocity = new Vector2D(3, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(3, 0.1, 1, true), new Vector2D(1.2, 0)));
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(4, 0.1, 1, true), new Vector2D(1.4, 0)));
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(5, 0.1, 1, true), new Vector2D(1.6, 0)));

            var scene = new Scene("Auto: Newtons cradle IV", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateSceneBouncingBallsOnALine1()
        {
            var initialState = new State();

            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.1, 1, true), new Vector2D(-0.8, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.1, 1, true), new Vector2D(-0.5, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(3, 0.1, 1, true), new Vector2D(-0.2, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(4, 0.1, 1, true), new Vector2D(0.1, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(5, 0.1, 1, true), new Vector2D(0.4, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(6, 0.1, 1, true), new Vector2D(0.7, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(7, 0.1, 1, true), new Vector2D(1.0, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(8, 0.1, 1, true), new Vector2D(1.3, 0)) { NaturalVelocity = new Vector2D(0.2, 0) });

            var scene = new Scene("Auto: Bouncing balls on a line 1", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateSceneBouncingBallsOnALine2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.1, 1, true), new Vector2D(-0.8, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.1, 1, true), new Vector2D(-0.5, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(3, 0.1, 1, true), new Vector2D(-0.2, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(4, 0.1, 1, true), new Vector2D(0.1, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(5, 0.1, 1, true), new Vector2D(0.4, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(6, 0.1, 1, true), new Vector2D(0.7, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(7, 0.1, 1, true), new Vector2D(1.0, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(8, 0.1, 1, true), new Vector2D(1.3, 0)) { NaturalVelocity = new Vector2D(0.2, -1) });

            var scene = new Scene("Auto: Bouncing balls on a line 2", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateScenePoolTableWithOneBall()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125))
            {
                NaturalVelocity = new Vector2D(2, 1)
            });

            var scene = new Scene("Auto: Pool table, 1 ball", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateScenePoolTableWithTwoBalls()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 0)) { NaturalVelocity = new Vector2D(2, 0) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.125, 1, true), new Vector2D(2, 0.1)));

            var scene = new Scene("Auto: Pool table, 2 balls", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            return scene;
        }

        private static Scene GenerateScenePoolTableWithManyBalls()
        {
            var initialState = new State();

            var bounds_x0 = -1.0;
            var bounds_x1 = 3.0;
            var bounds_y0 = -0.3;
            var bounds_y1 = 1.0;

            var ballRadius = 0.125;
            var ballSpeed = 2.0;
            var spacing = 0.05;
            var random = new Random(0);

            for (var x = bounds_x0 + spacing + ballRadius;
                 x < bounds_x1 - spacing - ballRadius;
                 x += ballRadius * 2 + spacing)
            {
                for (var y = bounds_y0 + spacing + ballRadius;
                     y < bounds_y1 - spacing - ballRadius;
                     y += ballRadius * 2 + spacing)
                {
                    var angle = 2.0 * random.NextDouble() * System.Math.PI;

                    var velocity = new Vector2D(
                        ballSpeed * System.Math.Cos(angle),
                        ballSpeed * System.Math.Sin(angle));

                    initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, ballRadius, 1, true), new Vector2D(x, y))
                    {
                        NaturalVelocity = velocity
                    });
                }
            }

            var scene = new Scene("Auto: Pool table, many balls", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;

            scene.AddRectangularBoundary(bounds_x0, bounds_x1, bounds_y0, bounds_y1, false);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                foreach (var bs in propagatedState.BodyStates)
                {
                    if (bs.Position.X - ballRadius < bounds_x0 ||
                        bs.Position.X + ballRadius > bounds_x1 ||
                        bs.Position.Y - ballRadius < bounds_y0 ||
                        bs.Position.Y + ballRadius > bounds_y1)
                    {
                        response.Outcome = "Invalid data";
                        response.IndexOfLastState = propagatedState.Index;
                    }
                }

                return response;
            };

            return scene;
        }

        private static Scene GenerateScenePoolTableWithOneBallAndThreeBoundaryPoints()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125)) { NaturalVelocity = new Vector2D(2, 1) });

            var scene = new Scene("Auto: Pool table, 1 ball, 3 boundary points", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddBoundary(new BoundaryPoint(new Vector2D(1, 0.35)));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(0, 0.35)));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(2, 0.35)));
            return scene;
        }

        private static Scene GenerateScenePoolTableWithTwoBallsAnd1LineSegment()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125)) { NaturalVelocity = new Vector2D(2, 1) });
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(2, 0.125, 1, true), new Vector2D(1, 0.7)) { NaturalVelocity = new Vector2D(2, 1) });

            var scene = new Scene("Auto: Pool table, 2 balls, 1 boundary line segment", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddBoundary(new LineSegment(new Vector2D(-0.95, 0.35), new Vector2D(2.95, 0.35)));

            return scene;
        }

        private static Scene GenerateScenePoolTableWith1BallAnd1LineSegment()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125)) { NaturalVelocity = new Vector2D(2, 1) });

            var scene = new Scene("Auto: Pool table, 1 ball, 1 boundary line segment", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddBoundary(new LineSegment(new Vector2D(0, 0.35), new Vector2D(2, 0.35)));

            return scene;
        }

        private static Scene GenerateSceneBallTrain1(
            //UpdateAuxFields updateAuxFields
            )
        {
            var initialState = new State();

            var scene = new Scene("Auto: Ball train I (no gravity)", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, true, 0.005);

            scene.InteractionCallBack += (state, events, position, collisions, currentState) =>
            {
                var totalEnergy = currentState.CalculateTotalEnergy(0);

                //updateAuxFields($"E: {totalEnergy}", "");

                return false;
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;

            scene.AddBoundary(new HorizontalLineSegment(-1, 0, 2.5));
            scene.AddBoundary(new LineSegment(new Vector2D(2.5, -1), new Vector2D(3, -0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(3, -0.5), new Vector2D(2.5, 0)));
            scene.AddBoundary(new LineSegment(new Vector2D(2.5, 0), new Vector2D(3, 0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(3, 0.5), new Vector2D(2.5, 1)));
            scene.AddBoundary(new LineSegment(new Vector2D(2.5, 1), new Vector2D(3, 1.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(3, 1.5), new Vector2D(2.5, 2)));
            scene.AddBoundary(new HorizontalLineSegment(2, 0, 2.5));
            scene.AddBoundary(new LineSegment(new Vector2D(0, -1), new Vector2D(0.5, -0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(0.5, -0.5), new Vector2D(0, 0)));
            scene.AddBoundary(new LineSegment(new Vector2D(0, 0), new Vector2D(0.5, 0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(0.5, 0.5), new Vector2D(0, 1)));
            scene.AddBoundary(new LineSegment(new Vector2D(0, 1), new Vector2D(0.5, 1.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(0.5, 1.5), new Vector2D(0, 2)));

            var extraBodies = Enumerable.Range(1, 60)
                .Select(i => new
                {
                    StateIndex = i * 60,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(0.5, -0.75)) { NaturalVelocity = new Vector2D(1, 0) }
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneBallTrain2(
            //UpdateAuxFields updateAuxFields
            )
        {
            var standardGravity = 9.82;

            var initialState = new State();

            var scene = new Scene("Auto: Ball train II (with gravity)", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, standardGravity, 0, 0, 1, true, true, 0.005);

            scene.InteractionCallBack += (state, events, position, collisions, currentState) =>
            {
                var totalEnergy = currentState.CalculateTotalEnergy(standardGravity);

                //updateAuxFields($"E: {totalEnergy}", "");

                return false;
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            var extraBodies = Enumerable.Range(1, 16)
                .Select(i => new
                {
                    StateIndex = i * 150,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(-0.8, 0)) { NaturalVelocity = new Vector2D(0.3, 0) }
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneBallTrain3(
            //UpdateAuxFields updateAuxFields
            )
        {
            var standardGravity = 9.82;
            var initialState = new State();

            var scene = new Scene("Auto: Ball train III (with gravity)", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, standardGravity, 0, 0, 1, true, true, 0.005);

            scene.InteractionCallBack += (state, events, position, collisions, currentState) =>
            {
                var totalEnergy = currentState.CalculateTotalEnergy(standardGravity);

                //updateAuxFields($"E: {totalEnergy}", "");

                return false;
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddRectangularBoundary(0, 0.5, 0.2, 0.7, false);
            scene.AddRectangularBoundary(1.5, 2, 0.2, 0.7, false);

            var extraBodies = Enumerable.Range(1, 20)
                .Select(i => new
                {
                    StateIndex = i * 20,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(-0.8, 0.8)) { NaturalVelocity = new Vector2D(2.7, -4) }
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.BodyStates.Add(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneBodyFollowingPath()
        {
            var initialState = new State();

            var path = new Path
            {
                WayPoints = new List<Vector2D>
                {
                    new Vector2D(1, -1),
                    new Vector2D(-1, -1),
                    new Vector2D(1, 1),
                    new Vector2D(-1, 1)
                }
            };

            initialState.AddBodyState(
                new BodyStateEnemy(
                    new CircularBody(1, 0.125, 1, true),
                    new Vector2D(0, 0))
                {
                    Path = path,
                    Speed = 1,
                    NaturalVelocity = new Vector2D(1, 0)
                });

            var standardGravity = 0.0;
            var gravitationalConstant = 0.0;
            var handleBoundaryCollisions = false;
            var handleBodyCollisions = false;
            var coefficientOfFriction = 0.0;

            var scene = new Scene(
                "Auto: Body Following route",
                new Point2D(-2, -3),
                new Point2D(5, 3),
                initialState,
                standardGravity,
                gravitationalConstant,
                coefficientOfFriction,
                1,
                handleBoundaryCollisions,
                handleBodyCollisions,
                0.005);

            return scene;
        }

        private static Scene GenerateSceneBallInteraction1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1.03, -0.125)));

            var scene = new Scene("Interactive: Ball I", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddBoundary(new BoundaryPoint(new Vector2D(1, 0.4)));

            return scene;
        }

        private static Scene GenerateSceneBallInteraction2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1.03, -0.125)));

            var scene = new Scene("Interactive: Ball II", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddBoundary(new CircularBoundary(new Vector2D(1, 0.4), 0.1));

            return scene;
        }

        private static Scene GenerateSceneBallInteraction3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.1, 1, true), new Vector2D(-0.5, 0.4)));

            var scene = new Scene("Interactive: Ball III", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;
            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddBoundary(new LineSegment(new Vector2D(0, 0.4), new Vector2D(2, 0.4)));

            return scene;
        }

        private static Scene GenerateSceneBallInteraction4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125)));

            var scene = new Scene("Interactive: Ball IV", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;
            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);
            scene.AddRectangularBoundary(0, 0.5, 0.2, 0.7, false);
            scene.AddRectangularBoundary(1.5, 2, 0.2, 0.7, false);

            return scene;
        }

        private static Scene GenerateSceneBallInteraction5()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyStateClassic(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125)));

            var scene = new Scene("Interactive: Ball V", new Point2D(-1.4, -1.3), new Point2D(5, 3), initialState, 0, 0, 0, 1, true, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;
            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;

            scene.AddRectangularBoundary(-1, 3, -0.3, 1, false);

            var diamondCenter = new Vector2D(0, 0.5);
            var diamondRadius = 0.4;

            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(0, -diamondRadius),
                diamondCenter + new Vector2D(diamondRadius, 0)));
            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(diamondRadius, 0),
                diamondCenter + new Vector2D(0, diamondRadius)));
            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(0, diamondRadius),
                diamondCenter + new Vector2D(-diamondRadius, 0)));
            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(-diamondRadius, 0),
                diamondCenter + new Vector2D(0, -diamondRadius)));

            return scene;
        }
    }
}
