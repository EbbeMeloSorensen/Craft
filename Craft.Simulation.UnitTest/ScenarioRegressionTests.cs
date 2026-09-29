using Craft.Logging;
using Craft.Math;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.Simulation.Engine;
using Craft.Simulation.Scenarios;
using Xunit;

namespace Craft.Simulation.UnitTest;

public class ScenarioRegressionTests
{
    [Fact]
    public void BouncingBall_BeforeContact_FollowsFreeFall()
    {
        var scene = SimulationScenarios.BouncingBall();
        var result = Advance(scene, scene.InitialState.Clone(), 100);
        var ball = Assert.Single(result.State.BodyStates);

        Near(1.2, ball.Position.X);
        Near(-0.125 + 0.5 * 9.82 * 0.1 * 0.1, ball.Position.Y);
        Near(2, ball.Velocity.X);
        Near(0.982, ball.Velocity.Y);
        Assert.Empty(result.BoundaryCollisions);
        Assert.Empty(result.BodyCollisions);
    }

    [Fact]
    public void BouncingBall_AfterFloorContact_ReflectsAndContinuesMoving()
    {
        var scene = SimulationScenarios.BouncingBall();
        var result = Advance(scene, scene.InitialState.Clone(), 700);
        var ball = Assert.Single(result.State.BodyStates);
        // Floor y = 2, radius = .125; the center falls from -.125 to 1.875.
        var impactTime = System.Math.Sqrt(2 * 2.0 / scene.StandardGravity);
        var impactSpeed = scene.StandardGravity * impactTime;
        var remaining = 0.7 - impactTime;

        Near(2.4, ball.Position.X);
        Near(1.875 - impactSpeed * remaining + 0.5 * scene.StandardGravity * remaining * remaining,
            ball.Position.Y, 1e-4);
        Near(2, ball.Velocity.X);
        Near(-impactSpeed + scene.StandardGravity * remaining, ball.Velocity.Y, 1e-4);
        Assert.Single(result.BoundaryCollisions);
        Assert.Empty(result.BodyCollisions);
    }

    [Fact]
    public void TwoBalls_ObliqueElasticCollision_PreservesMomentumAndEnergy()
    {
        var scene = SimulationScenarios.PoolTableWithTwoBalls();
        var result = Advance(scene, scene.InitialState.Clone(), 420);
        Assert.Equal(2, result.State.BodyStates.Count);
        var first = result.State.BodyStates.Single(b => b.Body.Id == 1);
        var second = result.State.BodyStates.Single(b => b.Body.Id == 2);
        // Equal masses; transfer the normal component of velocity at contact.
        var offset = System.Math.Sqrt(0.25 * 0.25 - 0.1 * 0.1);
        var impactTime = (1 - offset) / 2;
        var remaining = 0.42 - impactTime;
        var transferredY = 2 * (offset / 0.25) * (0.1 / 0.25);

        Near(0.32, first.Velocity.X);
        Near(-transferredY, first.Velocity.Y);
        Near(1.68, second.Velocity.X);
        Near(transferredY, second.Velocity.Y);
        Near(2 - offset + 0.32 * remaining, first.Position.X);
        Near(-transferredY * remaining, first.Position.Y);
        Near(2 + 1.68 * remaining, second.Position.X);
        Near(0.1 + transferredY * remaining, second.Position.Y);
        Near(2, first.Velocity.X + second.Velocity.X);
        Near(0, first.Velocity.Y + second.Velocity.Y);
        Near(2, result.State.CalculateTotalEnergy(0));
        var collision = Assert.Single(result.BodyCollisions);
        Assert.Equal(new[] { 1, 2 }, new[] { collision.Body1.Id, collision.Body2.Id }.OrderBy(id => id));
        Assert.Empty(result.BoundaryCollisions);
    }

    [Theory]
    [InlineData(0.002, 125)]
    [InlineData(0.025, 10)]
    public void LineEndpoint_HeadOnApproach_StopsAtContact(double deltaT, int steps)
    {
        var scene = SimulationScenarios.BallAgainstLineEndpoint();
        var state = scene.InitialState.Clone();
        // Supply one rightward movement, without repeated GUI input callbacks.
        Assert.IsType<BodyStateClassic>(Assert.Single(state.BodyStates)).ArtificialVelocity = new Vector2D(2, 0);
        var result = Advance(scene, state, steps, deltaT);
        var ball = Assert.Single(result.State.BodyStates);

        Near(-0.1, ball.Position.X);
        Near(0.4, ball.Position.Y);
        Near(0, ball.Velocity.X);
        Near(0, ball.Velocity.Y);
        var collision = Assert.Single(result.BoundaryCollisions);
        var segment = Assert.IsType<LineSegment>(collision.Boundary);
        Near(0, segment.Point1.X);
        Near(0.4, segment.Point1.Y);
        Assert.Empty(result.BodyCollisions);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Factories_CreateIndependentMutableScenes(int scenario)
    {
        Func<Scene>[] factories = [SimulationScenarios.BouncingBall,
            SimulationScenarios.PoolTableWithTwoBalls, SimulationScenarios.BallAgainstLineEndpoint];
        var first = factories[scenario]();
        var second = factories[scenario]();

        Assert.NotSame(first, second);
        Assert.NotSame(first.InitialState, second.InitialState);
        Assert.NotSame(first.InitialState.BodyStates[0], second.InitialState.BodyStates[0]);
        Assert.NotSame(first.InitialState.BodyStates[0].Body, second.InitialState.BodyStates[0].Body);
        Assert.NotSame(first.InitialState.BodyStates[0].Position, second.InitialState.BodyStates[0].Position);
        Assert.NotSame(first.Boundaries[0], second.Boundaries[0]);
        first.InitialState.BodyStates.Clear();
        first.Boundaries.Clear();
        Assert.NotEmpty(second.InitialState.BodyStates);
        Assert.NotEmpty(second.Boundaries);
    }

    private static (State State, List<BoundaryCollisionReport> BoundaryCollisions,
        List<BodyCollisionReport> BodyCollisions) Advance(Scene scene, State state, int steps, double? deltaT = null)
    {
        var boundaryCollisions = new List<BoundaryCollisionReport>();
        var bodyCollisions = new List<BodyCollisionReport>();
        var logger = new DummyLogger();
        for (var step = 0; step < steps; step++)
        {
            state = Calculator.PropagateState(scene, state, deltaT ?? scene.DeltaT, logger,
                out var boundaries, out var bodies);
            boundaryCollisions.AddRange(boundaries);
            bodyCollisions.AddRange(bodies);
            Assert.All(state.BodyStates, body =>
            {
                Assert.True(double.IsFinite(body.Position.X) && double.IsFinite(body.Position.Y),
                    $"{scene.Name}, step {step + 1}: non-finite position for body {body.Body.Id}");
                Assert.True(double.IsFinite(body.Velocity.X) && double.IsFinite(body.Velocity.Y),
                    $"{scene.Name}, step {step + 1}: non-finite velocity for body {body.Body.Id}");
            });
        }

        return (state, boundaryCollisions, bodyCollisions);
    }

    private static void Near(double expected, double actual, double tolerance = 1e-7)
    {
        Assert.True(double.IsFinite(actual) && System.Math.Abs(expected - actual) <= tolerance,
            $"Expected {expected:R} ± {tolerance:R}, actual {actual:R}");
    }
}
