namespace Craft.Math
{
    public class LineSegment2D
    {
        public Point2D Point1 { get; }
        public Point2D Point2 { get; }

        public LineSegment2D(
            Point2D point1, 
            Point2D point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
}