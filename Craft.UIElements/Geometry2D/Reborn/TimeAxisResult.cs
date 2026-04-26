namespace Craft.UIElements.Geometry2D.Reborn;

public record TimeAxisResult(
    IReadOnlyList<TimeAxisTick> MinorTicks,
    IReadOnlyList<TimeAxisTick> MajorTicks,
    TimeAxisTick AnchorTick
);