using Craft.Utils;
using GalaSoft.MvvmLight;
using System.Windows;
using System.Windows.Media;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class LineViewModel : ViewModelBase
    {
        private Point _point1InViewportCoordinates;
        private Point _point2InViewportCoordinates;

        public PointD Point1 { get; }
        public PointD Point2 { get; }
        public double Thickness { get; }
        public Brush Brush { get; }

        public Point Point1InViewportCoordinates
        {
            get => _point1InViewportCoordinates;
            private set
            {
                _point1InViewportCoordinates = value;
                RaisePropertyChanged();
            }
        }

        public Point Point2InViewportCoordinates
        {
            get => _point2InViewportCoordinates;
            private set
            {
                _point2InViewportCoordinates = value;
                RaisePropertyChanged();
            }
        }

        public LineViewModel(
            PointD point1,
            PointD point2,
            double thickness,
            Brush brush)
        {
            Point1 = point1;
            Point2 = point2;
            Thickness = thickness;
            Brush = brush;
        }

        public void UpdateViewportCoordinates(
            Size scaling,
            Point worldWindowUpperLeft)
        {
            Point1InViewportCoordinates = new Point(
                (Point1.X - worldWindowUpperLeft.X) * scaling.Width,
                (Point1.Y - worldWindowUpperLeft.Y) * scaling.Height);

            Point2InViewportCoordinates = new Point(
                (Point2.X - worldWindowUpperLeft.X) * scaling.Width,
                (Point2.Y - worldWindowUpperLeft.Y) * scaling.Height);
        }
    }
}