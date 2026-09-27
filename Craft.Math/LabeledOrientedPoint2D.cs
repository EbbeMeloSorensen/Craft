namespace Craft.Math;

/// <summary>An oriented point with a text label.</summary>
public class LabeledOrientedPoint2D : OrientedPoint2D
{
    public string Text { get; }

    public LabeledOrientedPoint2D(double x, double y, double angleDegrees, string text)
        : base(x, y, angleDegrees)
    {
        Text = text ?? throw new System.ArgumentNullException(nameof(text));
    }
}
