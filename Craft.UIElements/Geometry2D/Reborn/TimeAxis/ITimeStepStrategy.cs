namespace Craft.UIElements.Geometry2D.Reborn;

public interface ITimeStepStrategy
{
    long Align(long ticks);

    long Next(long ticks);

    double ApproximateDurationSeconds { get; }
}