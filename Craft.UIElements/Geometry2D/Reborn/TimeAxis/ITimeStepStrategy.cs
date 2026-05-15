namespace Craft.UIElements.Geometry2D.Reborn;

public interface ITimeStepStrategy
{
    long Align(long ticks);

    long Next(long ticks);

    double ApproximateDurationSeconds { get; }

    bool IsMajorTick(long ticks);

    IReadOnlyList<string> FormatLabel(long ticks, TickKind kind);
}