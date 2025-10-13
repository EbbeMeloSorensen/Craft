using Craft.Math;

namespace Craft.GeometryEditor3D.ViewModel
{
    public class LineViewModel
    {
        public Point2D Point1 { get; }
        public Point2D Point2 { get; }

        public LineViewModel(Point2D point1, Point2D point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public LineViewModel(LineSegment2D line)
        {
            Point1 = line.Point1;
            Point2 = line.Point2;
        }
    }
}
