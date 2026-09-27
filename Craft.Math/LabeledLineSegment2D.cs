namespace Craft.Math;

/// <summary>A line segment with a text label.</summary>
public class LabeledLineSegment2D : LineSegment2D
{
    public string Text { get; }

    public LabeledLineSegment2D(Point2D point1, Point2D point2, string text)
        : base(point1, point2)
    {
        Text = text ?? throw new System.ArgumentNullException(nameof(text));
    }
}
