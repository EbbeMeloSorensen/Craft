namespace Craft.UIElements.Geometry2D.Reborn;

public static class TimeTickEngine
{
    private static readonly ITimeStepStrategy[] Candidates =
    {
        new FixedStepStrategy(TimeSpan.FromSeconds(1)),
        new FixedStepStrategy(TimeSpan.FromSeconds(2)),
        new FixedStepStrategy(TimeSpan.FromSeconds(5)),
        new FixedStepStrategy(TimeSpan.FromSeconds(10)),
        new FixedStepStrategy(TimeSpan.FromSeconds(15)),
        new FixedStepStrategy(TimeSpan.FromSeconds(20)),
        new FixedStepStrategy(TimeSpan.FromSeconds(30)),

        new FixedStepStrategy(TimeSpan.FromMinutes(1)),
        new FixedStepStrategy(TimeSpan.FromMinutes(2)),
        new FixedStepStrategy(TimeSpan.FromMinutes(5)),
        new FixedStepStrategy(TimeSpan.FromMinutes(10)),
        new FixedStepStrategy(TimeSpan.FromMinutes(15)),
        new FixedStepStrategy(TimeSpan.FromMinutes(20)),
        new FixedStepStrategy(TimeSpan.FromMinutes(30)),

        new FixedStepStrategy(TimeSpan.FromHours(1)),
        new FixedStepStrategy(TimeSpan.FromHours(2)),
        new FixedStepStrategy(TimeSpan.FromHours(3)),
        new FixedStepStrategy(TimeSpan.FromHours(4)),
        new FixedStepStrategy(TimeSpan.FromHours(6)),
        new FixedStepStrategy(TimeSpan.FromHours(8)),
        new FixedStepStrategy(TimeSpan.FromHours(12)),

        new FixedStepStrategy(TimeSpan.FromDays(1)),
        new FixedStepStrategy(TimeSpan.FromDays(2)),
        new FixedStepStrategy(TimeSpan.FromDays(5)),

        new MonthStepStrategy(1),
        new MonthStepStrategy(2),
        new MonthStepStrategy(3),
        new MonthStepStrategy(4),
        new MonthStepStrategy(6),

        new YearStepStrategy(1),
        new YearStepStrategy(2),
        new YearStepStrategy(5),
        new YearStepStrategy(10),
        new YearStepStrategy(20),
        new YearStepStrategy(50),
        new YearStepStrategy(100),
    };

    public static IReadOnlyList<Tick> Generate(
        long startTicks,
        long endTicks,
        double viewportWidth,
        double targetSpacing)
    {
        var strategy = ChooseStrategy(
            startTicks,
            endTicks,
            viewportWidth,
            targetSpacing);

        var ticks = new List<Tick>();

        var current = strategy.Align(startTicks);

        while (current <= endTicks)
        {
            var x = ToViewportX(
                current,
                startTicks,
                endTicks,
                viewportWidth);

            var kind = strategy.IsMajorTick(current)
                ? TickKind.Major
                : TickKind.Minor;

            var labelLines =
                strategy.FormatLabel(current, kind);

            ticks.Add(new Tick(
                x,
                current,
                kind,
                labelLines));

            current = strategy.Next(current);
        }

        return ticks;
    }

    private static ITimeStepStrategy ChooseStrategy(
        long startTicks,
        long endTicks,
        double viewportWidth,
        double targetSpacing)
    {
        var totalSeconds =
            TimeSpan.FromTicks(endTicks - startTicks)
                    .TotalSeconds;

        var bestError = double.MaxValue;
        var best = Candidates.Last();

        foreach (var candidate in Candidates)
        {
            var tickCount =
                totalSeconds /
                candidate.ApproximateDurationSeconds;

            var spacing =
                viewportWidth / tickCount;

            var error =
                System.Math.Abs(spacing - targetSpacing);

            if (error > bestError) continue;

            bestError = error;
            best = candidate;
        }

        return best;
    }

    private static double ToViewportX(
        long tick,
        long startTicks,
        long endTicks,
        double width)
    {
        double total = endTicks - startTicks;
        double offset = tick - startTicks;

        return offset / total * width;
    }
}