namespace Craft.UIElements.Geometry2D.Reborn
{
    public class FrameEventArgs : EventArgs
    {
        public TimeSpan Time { get; }
        public double DeltaSeconds { get; }

        public FrameEventArgs(
            TimeSpan time,
            double deltaSeconds)
        {
            Time = time;
            DeltaSeconds = deltaSeconds;
        }
    }
}
