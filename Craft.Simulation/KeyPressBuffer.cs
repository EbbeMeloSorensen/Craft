using Craft.Simulation.Engine;

namespace Craft.Simulation;

/// <summary>
/// UI-thread queue of discrete presses. A press is retained until a simulation
/// state is consumed; repeated presses are delivered on separate updates.
/// This does not track which keys are currently held.
/// </summary>
public sealed class KeyPressBuffer
{
    private readonly Queue<KeyboardKey> _presses = new();

    public void Enqueue(KeyboardKey key) => _presses.Enqueue(key);

    public KeyboardState Peek() => new KeyboardState
    {
        LeftArrowDown = _presses.TryPeek(out var key) && key == KeyboardKey.LeftArrow,
        RightArrowDown = _presses.TryPeek(out key) && key == KeyboardKey.RightArrow,
        UpArrowDown = _presses.TryPeek(out key) && key == KeyboardKey.UpArrow,
        DownArrowDown = _presses.TryPeek(out key) && key == KeyboardKey.DownArrow,
        SpaceDown = _presses.TryPeek(out key) && key == KeyboardKey.Space
    };

    public void Consume() => _presses.TryDequeue(out _);

    public void Clear() => _presses.Clear();
}
