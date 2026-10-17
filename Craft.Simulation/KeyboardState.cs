namespace Craft.Simulation
{
    /// <summary>Immutable input snapshot supplied by the host for one update.</summary>
    public sealed class KeyboardState
    {
        public bool LeftArrowDown { get; init; }
        public bool RightArrowDown { get; init; }
        public bool UpArrowDown { get; init; }
        public bool DownArrowDown { get; init; }
        public bool SpaceDown { get; init; }

    }
}
