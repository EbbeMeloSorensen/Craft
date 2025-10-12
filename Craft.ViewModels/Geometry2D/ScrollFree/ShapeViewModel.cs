using Craft.Utils;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public abstract class ShapeViewModel : ViewModelBase
    {
        private PointD _point;
        private double _width;
        private double _height;
        private string _text;

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

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }
    }
}
