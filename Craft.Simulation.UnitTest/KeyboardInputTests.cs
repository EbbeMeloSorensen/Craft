using Craft.Logging;
using Craft.Simulation.Engine;
using Craft.Simulation.Scenarios;
using Xunit;

namespace Craft.Simulation.UnitTest;

public class KeyboardInputTests
{
    [Fact]
    public void HeldKeyIsAvailableOnFirstUpdateAndDoesNotStickAfterRelease()
    {
        var scene = SimulationScenarios.BouncingBall();
        var observed = new List<bool>();
        scene.InteractionCallBack = (held, presses, mouse, collisions, state) =>
        {
            observed.Add(held.RightArrowDown);
            return false;
        };
        var core = new EngineCore(new DummyLogger()) { Scene = scene };
        core.SpawnNewThread();
        try
        {
            // No key-down event is sent to the engine before the initial update.
            core.TryGetState(0, new KeyboardState { RightArrowDown = true }, new KeyboardState(), null, out _);
            core.TryGetState(0, new KeyboardState(), new KeyboardState(), null, out _);
            Assert.Equal(new[] { true, false }, observed);
        }
        finally
        {
            core.Reset();
        }
    }

    [Fact]
    public void QuickPressSurvivesReleaseAndUnsuccessfulUpdates()
    {
        var buffer = new KeyPressBuffer();
        buffer.Enqueue(KeyboardKey.Space);
        var engine = new Engine.Engine(new DummyLogger());
        // A paused engine cannot consume input, so the host retains the press.
        Assert.False(engine.UpdateModel(new KeyboardState(), buffer.Peek()));
        Assert.True(buffer.Peek().SpaceDown);

        var scene = SimulationScenarios.BouncingBall();
        bool? heldSpace = null;
        bool? pressedSpace = null;
        scene.InteractionCallBack = (held, presses, mouse, collisions, state) =>
        {
            heldSpace = held.SpaceDown;
            pressedSpace = presses.SpaceDown;
            return false;
        };
        engine.EngineCore.Scene = scene;
        engine.EngineCore.SpawnNewThread();
        try
        {
            var state = engine.EngineCore.TryGetState(0, new KeyboardState(), buffer.Peek(), null, out _);
            Assert.NotNull(state);
            buffer.Consume();
            Assert.Equal(false, heldSpace);
            Assert.Equal(true, pressedSpace);
            Assert.False(buffer.Peek().SpaceDown);
        }
        finally
        {
            engine.ResetEngine();
        }
    }

    [Fact]
    public void RepeatedPressesKeepTheirOrderAndAreConsumedIndividually()
    {
        var buffer = new KeyPressBuffer();
        buffer.Enqueue(KeyboardKey.Space);
        buffer.Enqueue(KeyboardKey.LeftArrow);
        buffer.Enqueue(KeyboardKey.Space);
        Assert.True(buffer.Peek().SpaceDown);
        Assert.True(buffer.Peek().SpaceDown); // Reading does not consume.
        buffer.Consume();
        Assert.True(buffer.Peek().LeftArrowDown);
        buffer.Consume();
        Assert.True(buffer.Peek().SpaceDown);
        buffer.Consume();
        Assert.False(buffer.Peek().SpaceDown);
    }

    [Fact]
    public void ClearingPendingPressesDiscardsActionsFromPreviousFocusOrScene()
    {
        var buffer = new KeyPressBuffer();
        buffer.Enqueue(KeyboardKey.Space);
        buffer.Enqueue(KeyboardKey.RightArrow);
        buffer.Clear();
        Assert.False(buffer.Peek().SpaceDown);
        Assert.False(buffer.Peek().RightArrowDown);
        buffer.Consume(); // Empty queues can be acknowledged safely.
    }
}
