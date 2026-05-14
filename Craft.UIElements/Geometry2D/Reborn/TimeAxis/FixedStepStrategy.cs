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

    public bool IsMajorTick(long ticks)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        if (_stepTicks < TimeSpan.TicksPerMinute)
        {
            return dt.Second == 0;
        }

        if (_stepTicks < TimeSpan.TicksPerHour)
        {
            return dt.Minute == 0 &&
                   dt.Second == 0;
        }

        if (_stepTicks < TimeSpan.TicksPerDay)
        {
            return dt.Hour == 0 &&
                   dt.Minute == 0 &&
                   dt.Second == 0;
        }

        return dt.Day == 1 &&
               dt.Hour == 0 &&
               dt.Minute == 0 &&
               dt.Second == 0;
    }
}