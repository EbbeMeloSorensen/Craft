using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Charts
{
    public class PieSliceDescriptionViewModel : ViewModelBase
    {
        public string _description;
        public double _width;
        public double _height;
        public double _positionX;
        public double _positionY;
        private Brush _brush;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged();
            }
        }

        public double PositionX
        {
            get { return _positionX; }
            set
            {
                _positionX = value;
                RaisePropertyChanged();
            }
        }

        public double PositionY
        {
            get { return _positionY; }
            set
            {
                _positionY = value;
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

        public PieSliceDescriptionViewModel(
            string description,
            double width,
            double height,
            double positionX,
            double positionY,
            Brush brush)
        {
            _description = description;
            _width = width;
            _height = height;
            _positionX = positionX;
            _positionY = positionY;
            _brush = brush;
        }
    }
}