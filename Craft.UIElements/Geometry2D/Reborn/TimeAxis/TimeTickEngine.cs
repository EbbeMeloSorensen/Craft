namespace Craft.UIElements.Geometry2D.Reborn;

public static class TimeTickEngine
{
    private static readonly ITimeStepStrategy[] Candidates =
    {
        new FixedStepStrategy(TimeSpan.FromSeconds(1)),
        new FixedStepStrategy(TimeSpan.FromSeconds(5)),
        new FixedStepStrategy(TimeSpan.FromSeconds(10)),
        new FixedStepStrategy(TimeSpan.FromSeconds(30)),

        new FixedStepStrategy(TimeSpan.FromMinutes(1)),
        new FixedStepStrategy(TimeSpan.FromMinutes(5)),
        new FixedStepStrategy(TimeSpan.FromMinutes(15)),

        new FixedStepStrategy(TimeSpan.FromHours(1)),
        new FixedStepStrategy(TimeSpan.FromHours(6)),

        new FixedStepStrategy(TimeSpan.FromDays(1)),

        new MonthStepStrategy(1),
        new MonthStepStrategy(3),

        new YearStepStrategy(1)
    };

    public static IReadOnlyList<Tick> Generate(
        long startTicks,
        long endTicks,
        double viewportWidth,
        double minPixelSpacing,
        double maxPixelSpacing)
    {
        var strategy = ChooseStrategy(
            startTicks,
            endTicks,
            viewportWidth,
            minPixelSpacing,
            maxPixelSpacing);

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

            var label =
                strategy.FormatLabel(current, kind);

            ticks.Add(new Tick(
                x,
                current,
                kind,
                label));

            current = strategy.Next(current);
        }

        return ticks;
    }

    private static ITimeStepStrategy ChooseStrategy(
        long startTicks,
        long endTicks,
        double viewportWidth,
        double minPixelSpacing,
        double maxPixelSpacing)
    {
        var totalSeconds =
            TimeSpan.FromTicks(endTicks - startTicks)
                    .TotalSeconds;

        foreach (var candidate in Candidates)
        {
            var tickCount =
                totalSeconds /
                candidate.ApproximateDurationSeconds;

            var spacing =
                viewportWidth / tickCount;

            if (spacing >= minPixelSpacing &&
                spacing <= maxPixelSpacing)
            {
                return candidate;
            }
        }

        return Candidates.Last();
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