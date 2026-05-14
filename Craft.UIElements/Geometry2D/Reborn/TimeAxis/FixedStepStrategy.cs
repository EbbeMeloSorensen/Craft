namespace Craft.UIElements.Geometry2D.Reborn;

public class FixedStepStrategy : ITimeStepStrategy
{
    private readonly long _stepTicks;

    public FixedStepStrategy(TimeSpan step)
    {
        _stepTicks = step.Ticks;
    }

    public double ApproximateDurationSeconds =>
        TimeSpan.FromTicks(_stepTicks).TotalSeconds;

    public long Align(long ticks)
    {
        return ticks - (ticks % _stepTicks);
    }

    public long Next(long ticks)
    {
        return ticks + _stepTicks;
    }
}