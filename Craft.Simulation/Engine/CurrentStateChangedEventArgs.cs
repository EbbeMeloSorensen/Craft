namespace Craft.Simulation.Engine;

public class CurrentStateChangedEventArgs : EventArgs
{
    public readonly State State;

    public CurrentStateChangedEventArgs(
        State state)
    {
        State = state;
    }
}