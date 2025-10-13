using Craft.Math;

namespace Craft.GeometryEditor3D.ViewModel
{
    public class PointViewModel
    {
        private static double _diameter = 5;

        public static double Diameter
        {
            get => _diameter;
            set
            {
                if (value.Equals(_diameter)) return;
                _diameter = value;
            }
        }

        public Point2D Point { get; }

        public PointViewModel(Point2D point)
        {
            Point = point;
        }
    }
}
