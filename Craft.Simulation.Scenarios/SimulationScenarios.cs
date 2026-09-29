using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;

namespace Craft.Simulation.Scenarios;

/// <summary>Shared demo fixtures. Each factory creates an independent scene.</summary>
public static class SimulationScenarios
{
    public static Scene BouncingBall()
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
            deltaT,
            //SceneViewMode.FocusOnFirstBody);
            SceneViewMode.Stationary);

        scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

        scene.AddRectangularBoundary(-1, 3, -0.3, 2, false);

        scene.InitializeBoundaryDataStore();

        return scene;
    }

    public static Scene PoolTableWithTwoBalls()
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

    public static Scene BallAgainstLineEndpoint()
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
}
