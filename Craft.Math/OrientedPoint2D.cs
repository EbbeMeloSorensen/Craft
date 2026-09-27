namespace Craft.Math;

/// <summary>A point with a polar orientation in degrees (0 right, 90 up).</summary>
public class OrientedPoint2D : Point2D
{
    public const double ViewportLength = 100;
    public double AngleDegrees { get; }

    public OrientedPoint2D(double x, double y, double angleDegrees) : base(x, y)
    {
        AngleDegrees = SnapAngle(angleDegrees);
    }

    public static double SnapAngle(double degrees)
    {
        var normalized = (degrees % 360 + 360) % 360;
        return System.Math.Round(normalized / 5, System.MidpointRounding.AwayFromZero) * 5 % 360;
    }

    public static OrientedPoint2D FromAngle(Point2D origin, double degrees)
        => new OrientedPoint2D(origin.X, origin.Y, degrees);

    public Vector2D ViewportOffset
    {
        get
        {
            var radians = AngleDegrees * System.Math.PI / 180;
            return new Vector2D(ViewportLength * System.Math.Cos(radians),
                -ViewportLength * System.Math.Sin(radians));
        }
    }

    public LineSegment2D GetWorldShaft(double scaleX, double scaleY)
    {
        var offset = ViewportOffset;
        return new LineSegment2D(this, new Point2D(X + offset.X / scaleX, Y + offset.Y / scaleY));
    }
}
