namespace Craft.Math;

/// <summary>A line segment with an identifier and ordered information strings.</summary>
public class LabeledLineSegment2D : LineSegment2D, ILabeledGeometry
{
    public IReadOnlyList<string> Labels { get; }
    public string Identifier => Labels[0];

    public LabeledLineSegment2D(Point2D point1, Point2D point2, string identifier)
        : this(point1, point2, new[] { identifier })
    {
    }

    public LabeledLineSegment2D(Point2D point1, Point2D point2, IEnumerable<string> labels) : base(point1, point2)
    {
        Labels = GeometryLabelStrings.Copy(labels);
    }
}
