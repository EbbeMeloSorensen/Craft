namespace Craft.UIElements.Geometry2D.Reborn;

public class MonthStepStrategy : ITimeStepStrategy
{
    private readonly int _months;

    public MonthStepStrategy(int months)
    {
        _months = months;
    }

    public double ApproximateDurationSeconds =>
        TimeSpan.FromDays(30 * _months).TotalSeconds;

    public long Align(long ticks)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        var aligned = new DateTime(
            dt.Year,
            dt.Month,
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
            dt.AddMonths(_months));
    }

    public bool IsMajorTick(long ticks)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        return dt.Month == 1;
    }
}