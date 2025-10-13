using System.Windows;
using System.Windows.Media;
using Craft.Math;
using GalaSoft.MvvmLight;

namespace Craft.Algorithms.GuiTest.Common
{
    public class Triangle2DViewModel : ViewModelBase
    {
        private PointCollection _points;

        public PointCollection Points
        {
            get { return _points; }
            set
            {
                _points = value;
                RaisePropertyChanged();
            }
        }

        public Triangle2DViewModel(Triangle2D triangle)
        {
            Points = new PointCollection
            {
                new Point(triangle.Point1.X, triangle.Point1.Y),
                new Point(triangle.Point2.X, triangle.Point2.Y),
                new Point(triangle.Point3.X, triangle.Point3.Y)
            };
        }
    }
}