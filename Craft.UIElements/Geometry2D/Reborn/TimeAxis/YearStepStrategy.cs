namespace Craft.UIElements.Geometry2D.Reborn;

public class YearStepStrategy : ITimeStepStrategy
{
    private readonly int _years;

    public YearStepStrategy(int years)
    {
        _years = years;
    }

    public double ApproximateDurationSeconds =>
        TimeSpan.FromDays(365 * _years).TotalSeconds;

    public long Align(long ticks)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        var aligned = new DateTime(
            (dt.Year / _years) * _years,
            1,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc);

        return TimeCoordinates.ToWorldTicks(aligned);
    }

    public long Next(long ticks)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        return TimeCoordinates.ToWorldTicks(
            dt.AddYears(_years));
    }

    public bool IsMajorTick(long ticks)
    {
        return true;
    }

    public IReadOnlyList<string> FormatLabel(
        long ticks,
        TickKind kind)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        return new[]
        {
            dt.ToString("yyyy")
        };
    }
}