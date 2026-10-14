using Craft.Logging;
using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Xunit;

namespace Craft.Simulation.UnitTest;

public class DoorCollisionBroadPhaseTests
{
    private static BodyDoor Door(int id, double x, double width = 2) =>
        new(id, 1, false, false, null) { Point1 = new(x, 0), Point2 = new(x, width) };

    [Fact]
    public void ManyDistantDoors_OnlyNearbyDoorIsACandidate()
    {
        var doors = Enumerable.Range(0, 1000).Select(i => Door(100 + i, i * 10)).ToArray();
        var circle = new BodyState(new CircularBody(1, .16, 1, false), new Vector2D(-.2, 1));
        var states = doors.Select(d => (BodyState)new BodyStateDoor(d, true, 0))
            .Append(circle).ToDictionary(s => s.Clone(), s => s);
        var pairs = new DoorCollisionBroadPhase(doors).GetCandidates(states).ToArray();
        var pair = Assert.Single(pairs);
        Assert.Same(circle.Body, pair.First.Body);
        Assert.Same(doors[0], pair.Second.Body);
    }

    [Fact]
    public void SweptBounds_IncludeDoorBetweenStartAndEnd()
    {
        var door = Door(100, 0);
        var before = new BodyState(new CircularBody(1, .05, 1, false), new Vector2D(-5, 1));
        var after = before.Clone();
        after.Position = new Vector2D(5, 1);
        var doorState = new BodyStateDoor(door, true, 0);
        var states = new Dictionary<BodyState, BodyState> { [after] = before, [doorState.Clone()] = doorState };
        Assert.Single(new DoorCollisionBroadPhase(new[] { door }).GetCandidates(states));
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(false, 0)]
    [InlineData(true, 100)]
    [InlineData(false, 100)]
    public void CollisionResults_MatchAllPairs(bool clockwise, double opening)
    {
        var door = Door(100, 0);
        var body = new CircularBody(1, .16, 1, false, false);
        var position = opening == 0 ? new Vector2D(-.17, 1) : new Vector2D(clockwise ? -.5 : .5, -.17);
        var velocity = opening == 0 ? new Vector2D(1, 0) : new Vector2D(0, 1);
        var state = new State(new BodyState[] {
            new BodyState(body, position) { NaturalVelocity = velocity },
            new BodyStateDoor(door, clockwise, opening) });
        var scene = new Scene("doors", new Point2D(-10, -10), new Point2D(10, 10), state,
            standardGravity: 0, gravitationalConstant: 0, handleBoundaryCollisions: false, deltaT: .02);
        scene.CheckForCollisionBetweenBodiesCallback = (a, b) => a is BodyDoor || b is BodyDoor;
        scene.CollisionBetweenTwoBodiesOccuredCallBack = (a, b) => OutcomeOfCollisionBetweenTwoBodies.Block;
        var expected = Calculator.PropagateState(scene, state.Clone(), .02, new DummyLogger(), out _, out var expectedReports);
        scene.BodyCollisionCandidates = new DoorCollisionBroadPhase(new[] { door }).GetCandidates;
        var actual = Calculator.PropagateState(scene, state.Clone(), .02, new DummyLogger(), out _, out var actualReports);
        Assert.NotEmpty(expectedReports);
        Assert.Equal(expectedReports.Count, actualReports.Count);
        Assert.Equal(expected.BodyStates[0].Position.X, actual.BodyStates[0].Position.X, 10);
        Assert.Equal(expected.BodyStates[0].Position.Y, actual.BodyStates[0].Position.Y, 10);
    }

    [Fact]
    public void Engine_DoesNotEnumerateDoorDoorPairsWhenBroadPhaseIsEnabled()
    {
        var doors = Enumerable.Range(0, 1000).Select(i => Door(100 + i, i * 10)).ToArray();
        var state = new State(doors.Select(d => (BodyState)new BodyStateDoor(d, true, 0)));
        state.BodyStates.Insert(0, new BodyState(new CircularBody(1, .16, 1, false, false), new Vector2D(-.5, 1)));
        var scene = new Scene("many doors", new Point2D(-10, -10), new Point2D(10010, 10), state,
            standardGravity: 0, gravitationalConstant: 0, handleBoundaryCollisions: false);
        var examinedPairs = 0;
        scene.CheckForCollisionBetweenBodiesCallback = (a, b) =>
        {
            examinedPairs++;
            return (a is BodyDoor) != (b is BodyDoor);
        };
        Calculator.PropagateState(scene, state.Clone(), .001, new DummyLogger(), out _, out _);
        Assert.Equal(500500, examinedPairs);
        examinedPairs = 0;
        scene.BodyCollisionCandidates = new DoorCollisionBroadPhase(doors).GetCandidates;
        Calculator.PropagateState(scene, state.Clone(), .001, new DummyLogger(), out _, out _);
        Assert.Equal(1, examinedPairs);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NarrowOpenDoor_UsesEngineEndpointExtent(bool clockwise)
    {
        var door = Door(100, 0, .25);
        var circle = new BodyState(new CircularBody(1, .05, 1, false),
            new Vector2D(clockwise ? -.95 : .95, 0));
        var doorState = new BodyStateDoor(door, clockwise, 100);
        var states = new[] { circle, doorState }.ToDictionary(s => s.Clone(), s => s);
        Assert.Single(new DoorCollisionBroadPhase(new[] { door }).GetCandidates(states));
    }

    [Fact]
    public void EmptyScene_HasNoCandidates()
    {
        Assert.Empty(new DoorCollisionBroadPhase(Array.Empty<BodyDoor>())
            .GetCandidates(new Dictionary<BodyState, BodyState>()));
    }
}
