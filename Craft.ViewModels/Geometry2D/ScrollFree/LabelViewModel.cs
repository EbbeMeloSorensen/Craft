using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class LabelViewModel : ViewModelBase
    {
        private PointD _point;
        private double _width;
        private double _height;
        private PointD _shift;
        private string _text;
        private double _opacity;
        public double? _fixedViewPortXCoordinate;
        public double? _fixedViewPortYCoordinate;

        public PointD Point
        {
            get => _point;
            set
            {
                _point = value;
                RaisePropertyChanged();
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                RaisePropertyChanged();
            }
        }

        public PointD Shift
        {
            get => _shift;
            set
            {
                _shift = value;
                RaisePropertyChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }

        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                RaisePropertyChanged();
            }
        }

        public double? FixedViewPortXCoordinate
        {
            get => _fixedViewPortXCoordinate;
            set
            {
                _fixedViewPortXCoordinate = value;
                RaisePropertyChanged();
            }
        }

        public double? FixedViewPortYCoordinate
        {
            get => _fixedViewPortYCoordinate;
            set
            {
                _fixedViewPortYCoordinate = value;
                RaisePropertyChanged();
            }
        }
    }
}
