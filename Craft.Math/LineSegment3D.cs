namespace Craft.Math
{
    public class LineSegment3D
    {
        public Point3D Point1 { get; }
        public Point3D Point2 { get; }

        public LineSegment3D(Point3D point1, Point3D point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
}