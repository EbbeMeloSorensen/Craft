namespace Craft.Math;

/// <summary>A point with a text label.</summary>
public class LabeledPoint2D : Point2D
{
    public string Text { get; }

    public LabeledPoint2D(double x, double y, string text) : base(x, y)
    {
        Text = text ?? throw new System.ArgumentNullException(nameof(text));
    }
}
