namespace Craft.Math
{
    /// <summary>A world-anchored arrow with a snapped viewport direction and fixed viewport length.</summary>
    public class Arrow2D : LineSegment2D
    {
        public const double ViewportLength = 100;
        public double AngleDegrees => SnapAngle(System.Math.Atan2(
            -(Point2.Y - Point1.Y), Point2.X - Point1.X) * 180 / System.Math.PI);

        public Arrow2D(Point2D point1, Point2D point2) : base(point1, point2)
        {
        }

        public static double SnapAngle(double degrees)
        {
            var normalized = (degrees % 360 + 360) % 360;
            return System.Math.Round(normalized / 5, System.MidpointRounding.AwayFromZero) * 5 % 360;
        }

        public static Arrow2D FromAngle(Point2D origin, double degrees)
        {
            var radians = SnapAngle(degrees) * System.Math.PI / 180;
            return new Arrow2D(origin, new Point2D(
                origin.X + System.Math.Cos(radians), origin.Y - System.Math.Sin(radians)));
        }

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
            return new LineSegment2D(Point1, new Point2D(
                Point1.X + offset.X / scaleX, Point1.Y + offset.Y / scaleY));
        }
    }
}
