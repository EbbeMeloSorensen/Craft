namespace Craft.Utils
{
    public class LineSegmentD
    {
        public PointD Point1 { get; }
        public PointD Point2 { get; }

        public LineSegmentD(
            PointD point1,
            PointD point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
}
