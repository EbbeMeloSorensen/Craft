namespace Craft.Math;

/// <summary>A point with an identifier and ordered information strings.</summary>
public class LabeledPoint2D : Point2D, ILabeledGeometry
{
    public IReadOnlyList<string> Labels { get; }
    public string Identifier => Labels[0];

    public LabeledPoint2D(double x, double y, string identifier)
        : this(x, y, new[] { identifier })
    {
    }

    public LabeledPoint2D(double x, double y, IEnumerable<string> labels) : base(x, y)
    {
        Labels = GeometryLabelStrings.Copy(labels);
    }
}
