namespace Craft.UIElements.Geometry2D.Reborn;

public static class TimeTickEngine
{
    private static readonly ITimeStepStrategy[] Candidates =
    {
        new FixedStepStrategy(TimeSpan.FromSeconds(1)),
        //new FixedStepStrategy(TimeSpan.FromSeconds(2)),
        new FixedStepStrategy(TimeSpan.FromSeconds(5)),
        //new FixedStepStrategy(TimeSpan.FromSeconds(10)),
        new FixedStepStrategy(TimeSpan.FromSeconds(15)),
        //new FixedStepStrategy(TimeSpan.FromSeconds(20)),
        new FixedStepStrategy(TimeSpan.FromSeconds(30)),

        new FixedStepStrategy(TimeSpan.FromMinutes(1)),
        //new FixedStepStrategy(TimeSpan.FromMinutes(2)),
        new FixedStepStrategy(TimeSpan.FromMinutes(5)),
        //new FixedStepStrategy(TimeSpan.FromMinutes(10)),
        new FixedStepStrategy(TimeSpan.FromMinutes(15)),
        //new FixedStepStrategy(TimeSpan.FromMinutes(20)),
        new FixedStepStrategy(TimeSpan.FromMinutes(30)),

        new FixedStepStrategy(TimeSpan.FromHours(1)),
        //new FixedStepStrategy(TimeSpan.FromHours(2)),
        new FixedStepStrategy(TimeSpan.FromHours(3)),
        //new FixedStepStrategy(TimeSpan.FromHours(4)),
        new FixedStepStrategy(TimeSpan.FromHours(6)),
        //new FixedStepStrategy(TimeSpan.FromHours(8)),
        new FixedStepStrategy(TimeSpan.FromHours(12)),

        new FixedStepStrategy(TimeSpan.FromDays(1)),
        //new FixedStepStrategy(TimeSpan.FromDays(2)),
        new FixedStepStrategy(TimeSpan.FromDays(5)),

        new MonthStepStrategy(1),
        //new MonthStepStrategy(2),
        new MonthStepStrategy(3),
        //new MonthStepStrategy(4),
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

        //var dummy = TimeCoordinates.ToDateTime(current);

        var firstSemantic =
            GetNextMonthBoundary(startTicks);

        var firstDistancePx =
            WorldDistanceToViewportDistance(
                current,
                firstSemantic,
                startTicks,
                endTicks,
                viewportWidth);

        // Hvis en kandidatlinie er tæt på en semantisk grænse (fx månedsskifte), så snap til den i stedet for at vise den "regulære" kandidatlinie
        const double semanticSnapThresholdPx = 40;

        var semanticDominatesFirstTick =
            firstDistancePx < semanticSnapThresholdPx;

        if (semanticDominatesFirstTick)
        {
            current = firstSemantic;
        }

        while (current <= endTicks)
        {
            var x = ToViewportX(
                current,
                startTicks,
                endTicks,
                viewportWidth);

            if (x >= 0)
            {
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
            }

            // -------------------------------------------------
            // Compute next regular tick
            // -------------------------------------------------

            var regularNext = strategy.Next(current);

            // -------------------------------------------------
            // Compute next semantic boundary (we might use that instead)
            // -------------------------------------------------

            var semanticNext = GetNextMonthBoundary(current);

            var distancePx =
                WorldDistanceToViewportDistance(
                    regularNext,
                    semanticNext,
                    startTicks,
                    endTicks,
                    viewportWidth);

            var semanticIsNear =
                distancePx < semanticSnapThresholdPx;

            if (semanticIsNear)
            {
                current = semanticNext;
                continue;
            }

            current = regularNext;
        }

        var hasMajor = ticks.Any(t => t.Kind == TickKind.Major);

        if (hasMajor || ticks.Count <= 0) return ticks;

        // No major ticks present, so promote the first tick with a label to the anchor
        // (later, some ticks may have no labels)

        var anchorIndex = 0;

        while (ticks[anchorIndex].LabelLines.Count == 0 && anchorIndex < ticks.Count - 1)
        {
            anchorIndex++;
        }

        ticks[anchorIndex] =
            ticks[anchorIndex] with
            {
                Kind = TickKind.Anchor,
                LabelLines = strategy.FormatLabel(ticks[anchorIndex].WorldTicks, TickKind.Anchor)
            };

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

    private static long GetNextMonthBoundary(long ticks)
    {
        var dt = TimeCoordinates.ToDateTime(ticks);

        var nextMonth = new DateTime(
                dt.Year,
                dt.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc)
            .AddMonths(1);

        return TimeCoordinates.ToWorldTicks(nextMonth);
    }

    private static double WorldDistanceToViewportDistance(
        long a,
        long b,
        long startTicks,
        long endTicks,
        double viewportWidth)
    {
        double totalWorld = endTicks - startTicks;

        double worldDistance = System.Math.Abs(b - a);

        return worldDistance / totalWorld * viewportWidth;
    }
}