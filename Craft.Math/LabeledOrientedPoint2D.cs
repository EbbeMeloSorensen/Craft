namespace Craft.Math;

/// <summary>An oriented point with an identifier and ordered information strings.</summary>
public class LabeledOrientedPoint2D : OrientedPoint2D, ILabeledGeometry
{
    public IReadOnlyList<string> Labels { get; }
    public string Identifier => Labels[0];

    public LabeledOrientedPoint2D(double x, double y, double angleDegrees, string identifier)
        : this(x, y, angleDegrees, new[] { identifier })
    {
    }

    public LabeledOrientedPoint2D(double x, double y, double angleDegrees, IEnumerable<string> labels) : base(x, y, angleDegrees)
    {
        Labels = GeometryLabelStrings.Copy(labels);
    }
}
