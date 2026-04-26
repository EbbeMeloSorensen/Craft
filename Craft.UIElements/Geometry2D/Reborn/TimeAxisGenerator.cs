namespace Craft.UIElements.Geometry2D.Reborn;

public static class TimeAxisGenerator
{
    static readonly DateTime Epoch = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Candidate steps in TICKS (not TimeSpan to avoid conversions)
    static readonly long[] Steps =
    {
        TimeSpan.FromSeconds(1).Ticks,
        TimeSpan.FromSeconds(5).Ticks,
        TimeSpan.FromSeconds(10).Ticks,
        TimeSpan.FromSeconds(30).Ticks,
        TimeSpan.FromMinutes(1).Ticks,
        TimeSpan.FromMinutes(5).Ticks,
        TimeSpan.FromMinutes(10).Ticks,
        TimeSpan.FromMinutes(30).Ticks,
        TimeSpan.FromHours(1).Ticks
    };

    public static TimeAxisResult Generate(
        DateTime start,
        DateTime end,
        double viewportWidth,
        double minPx,
        double maxPx)
    {
        long startTicks = ToWorldTicks(start);
        long endTicks = ToWorldTicks(end);

        double ticksPerPixel = (double)(endTicks - startTicks) / viewportWidth;

        long step = ChooseStep(ticksPerPixel, minPx, maxPx);

        var tickTimes = GenerateTicks(startTicks, endTicks, step);

        var minor = new List<TimeAxisTick>();
        var major = new List<TimeAxisTick>();

        long? prev = null;
        long? firstMajor = null;

        foreach (var t in tickTimes)
        {
            bool isMajor = IsMajor(t, prev);

            if (isMajor && firstMajor == null)
                firstMajor = t;

            prev = t;
        }

        long anchor = firstMajor ?? tickTimes[0];

        prev = null;

        foreach (var t in tickTimes)
        {
            bool isMajor = IsMajor(t, prev);
            bool isAnchor = t == anchor;

            var dt = ToDateTime(t);

            string label = Format(dt, prev.HasValue ? ToDateTime(prev.Value) : null, isAnchor, isMajor);

            double x = ToViewportX(t, startTicks, endTicks, viewportWidth);

            var tick = new TimeAxisTick(x, label, isMajor);

            if (isMajor) major.Add(tick);
            else minor.Add(tick);

            prev = t;
        }

        var anchorTick = major.Concat(minor).First(t => t.X == ToViewportX(anchor, startTicks, endTicks, viewportWidth));

        return new TimeAxisResult(minor, major, anchorTick);
    }

    // --- Step selection ---

    static long ChooseStep(double ticksPerPixel, double minPx, double maxPx)
    {
        foreach (var step in Steps)
        {
            double px = step / ticksPerPixel;

            if (px >= minPx && px <= maxPx)
                return step;
        }

        return Steps.Last();
    }

    // --- Tick generation (DRIFT-FREE) ---

    static List<long> GenerateTicks(long start, long end, long step)
    {
        var ticks = new List<long>();

        long first = AlignUp(start, step);

        for (long t = first; t <= end; t += step)
        {
            ticks.Add(t);
        }

        return ticks;
    }

    static long AlignUp(long value, long step)
    {
        long remainder = value % step;
        return remainder == 0 ? value : value + (step - remainder);
    }

    // --- Classification ---

    static bool IsMajor(long current, long? previous)
    {
        if (previous == null)
            return true;

        var c = ToDateTime(current);
        var p = ToDateTime(previous.Value);

        return c.Year != p.Year;
    }

    // --- Formatting ---

    static string Format(DateTime curr, DateTime? prev, bool isAnchor, bool isMajor)
    {
        if (isAnchor)
            return curr.ToString("yyyy-MM-dd HH:mm:ss");

        if (isMajor)
            return curr.ToString("yyyy-MM-dd");

        if (prev.HasValue && curr.Day != prev.Value.Day)
            return curr.ToString("dd MMM");

        return curr.ToString("HH:mm:ss");
    }

    // --- Coordinate transform (only here we use double) ---

    static double ToViewportX(long t, long start, long end, double width)
    {
        double total = end - start;
        double offset = t - start;

        return (offset / total) * width;
    }

    // --- Conversion helpers ---

    static long ToWorldTicks(DateTime t) => (t - Epoch).Ticks;

    static DateTime ToDateTime(long ticks) => Epoch.AddTicks(ticks);
}