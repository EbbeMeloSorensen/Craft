namespace Craft.Math
{
    public class Triangle2D
    {
        public Point2D Point1 { get; }
        public Point2D Point2 { get; }
        public Point2D Point3 { get; }

        public Triangle2D(
            Point2D point1,
            Point2D point2,
            Point2D point3)
        {
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
        }
    }
}
