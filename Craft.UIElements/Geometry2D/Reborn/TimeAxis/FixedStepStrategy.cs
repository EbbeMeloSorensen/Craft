using System.ComponentModel;

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
        var dt = TimeCoordinates.ToDateTime(ticks);

        // Only apply month-relative alignment
        // for day-based steps

        if (_stepTicks >= TimeSpan.TicksPerDay)
        {
            var monthStart = new DateTime(
                dt.Year,
                dt.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            var daysSinceMonthStart =
                (dt.Date - monthStart).Days;

            var stepDays =
                (int)TimeSpan
                    .FromTicks(_stepTicks)
                    .TotalDays;

            var alignedDays =
                (daysSinceMonthStart / stepDays)
                * stepDays;

            var aligned =
                monthStart.AddDays(alignedDays);

            return TimeCoordinates.ToWorldTicks(aligned);
        }

        // fallback for smaller units

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

    public IReadOnlyList<string> FormatLabel(
        long ticks,
        TickKind kind)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        if (_stepTicks < TimeSpan.TicksPerMinute)
        {
            return kind switch
            {
                TickKind.Minor => new[] { dt.ToString("ss") },
                TickKind.Major => new[] { dt.ToString("HH:mm:ss"), dt.ToString("yyyy-MM-dd") },
                TickKind.Anchor => new[] { dt.ToString("HH:mm::ss"), dt.ToString("yyyy-MM-dd") },
                _ => throw new InvalidEnumArgumentException()
            };
        }

        if (_stepTicks < TimeSpan.TicksPerHour)
        {
            return kind switch
            {
                TickKind.Minor => new[] { dt.ToString("mm") },
                TickKind.Major => new[] { dt.ToString("HH:mm"), dt.ToString("yyyy-MM-dd") },
                TickKind.Anchor => new[] { dt.ToString("HH:mm"), dt.ToString("yyyy-MM-dd") },
                _ => throw new InvalidEnumArgumentException()
            };
        }

        if (_stepTicks < TimeSpan.TicksPerDay)
        {
            return kind switch
            {
                TickKind.Minor => new[] { dt.ToString("HH:mm") },
                TickKind.Major => new[] { dt.ToString("HH:mm"), dt.ToString("yyyy-MM-dd") },
                TickKind.Anchor => new[] { dt.ToString("HH:mm"), dt.ToString("yyyy-MM-dd") },
                _ => throw new InvalidEnumArgumentException()
            };
        }

        return kind switch
        {
            TickKind.Minor => new[] {dt.ToString("dd")},
            TickKind.Major => new[] {dt.ToString("dd"), dt.ToString("MMM yyyy")},
            TickKind.Anchor => new[] {dt.ToString("dd"), dt.ToString("MMM yyyy")},
            _ => throw new InvalidEnumArgumentException()
        };
    }
}