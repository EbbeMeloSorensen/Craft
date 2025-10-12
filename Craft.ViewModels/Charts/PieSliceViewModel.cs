using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Charts
{
    public class PieSliceViewModel : ViewModelBase
    {
        private double _radius;
        private Point _centerPoint;
        private Point _topPoint;
        private Point _arcStartPoint;
        private Point _tagPoint;
        private string _tag;
        private Size _size;
        private bool _angleBiggerThan180Degrees;
        private double _startAngle;
        private Brush _brush;

        public double Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                RaisePropertyChanged();
            }
        }

        public Point CenterPoint
        {
            get { return _centerPoint; }
            set
            {
                _centerPoint = value;
                RaisePropertyChanged();
            }
        }

        public Point TopPoint
        {
            get { return _topPoint; }
            set
            {
                _topPoint = value;
                RaisePropertyChanged();
            }
        }

        public Point ArcStartPoint
        {
            get { return _arcStartPoint; }
            set
            {
                _arcStartPoint = value;
                RaisePropertyChanged();
            }
        }

        public Point TagPoint
        {
            get { return _tagPoint; }
            set
            {
                _tagPoint = value;
                RaisePropertyChanged();
            }
        }

        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                RaisePropertyChanged();
            }
        }

        public Size Size
        {
            get { return _size; }
            set
            {
                _size = value;
                RaisePropertyChanged();
            }
        }

        public bool AngleBiggerThan180Degrees
        {
            get { return _angleBiggerThan180Degrees; }
            set
            {
                _angleBiggerThan180Degrees = value;
                RaisePropertyChanged();
            }
        }

        public double StartAngle
        {
            get { return _startAngle; }
            set
            {
                _startAngle = value;
                RaisePropertyChanged();
            }
        }

        public Brush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                RaisePropertyChanged();
            }
        }

        public PieSliceViewModel(
            double diameter,
            double angle,
            double startAngle,
            Brush brush,
            string tag)
        {
            Radius = diameter / 2;
            CenterPoint = new Point(Radius, Radius);
            TopPoint = new Point(Radius, 0);
            Size = new Size(Radius, Radius);
            AngleBiggerThan180Degrees = angle > 180;
            StartAngle = startAngle;
            Brush = brush;
            Tag = tag;

            var angleInRadians = System.Math.PI * angle / 180;
            _arcStartPoint = new Point(
                Radius + Radius * System.Math.Sin(angleInRadians), 
                Radius - Radius * System.Math.Cos(angleInRadians));

            var startAngleInRadians = System.Math.PI * StartAngle / 180;
            var angleForTagInRadians = startAngleInRadians + angleInRadians / 2;
            var distanceFromCenterToTagPoint = Radius * 0.75;

            _tagPoint = new Point(
                Radius + distanceFromCenterToTagPoint * System.Math.Sin(angleForTagInRadians), 
                Radius - distanceFromCenterToTagPoint * System.Math.Cos(angleForTagInRadians));
        }
    }
}