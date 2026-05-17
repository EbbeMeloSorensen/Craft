using System.ComponentModel;

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
            ((dt.Month - 1) / _months) * _months + 1,
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

    public IReadOnlyList<string> FormatLabel(
        long ticks,
        TickKind kind)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        return kind switch
        {
            TickKind.Minor => new[] { dt.ToString("MMM") },
            TickKind.Major => new[] { dt.ToString("MMM"), dt.ToString("yyyy") },
            TickKind.Anchor => new[] { dt.ToString("MMM"), dt.ToString("yyyy") },
            _ => throw new InvalidEnumArgumentException()
        };
    }
}