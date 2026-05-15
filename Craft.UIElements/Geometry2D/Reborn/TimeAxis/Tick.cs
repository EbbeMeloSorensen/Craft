namespace Craft.UIElements.Geometry2D.Reborn;

// X: The viewport coordinate
// WorldTicks: The world coordinate
public record Tick(
    double X,
    long WorldTicks,
    TickKind Kind,
    IReadOnlyList<string> LabelLines);
