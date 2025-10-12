using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.Scrolling
{
    public class ImageEditorViewModel : ViewModelBase
    {
        private double _imageWidth;
        private double _imageHeight;
        private PointD _scrollableOffset;
        private PointD _scrollOffset;
        private double _magnification;
        //private PointD _mouseWorldPosition;

        public double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                if (value.Equals(_imageWidth)) return;
                _imageWidth = value;
                RaisePropertyChanged();
            }
        }

        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                if (value.Equals(_imageHeight)) return;
                _imageHeight = value;
                RaisePropertyChanged();
            }
        }

        public PointD ScrollableOffset
        {
            get { return _scrollableOffset; }
            set
            {
                if (value.Equals(_scrollableOffset)) return;
                _scrollableOffset = value;
                RaisePropertyChanged();
            }
        }

        public PointD ScrollOffset
        {
            get { return _scrollOffset; }
            set
            {
                if (value.Equals(_scrollOffset)) return;
                _scrollOffset = value;
                RaisePropertyChanged();
            }
        }

        public double Magnification
        {
            get { return _magnification; }
            set
            {
                if (value.Equals(_magnification)) return;
                _magnification = value;
                RaisePropertyChanged();
            }
        }

        //public PointD MouseWorldPosition
        //{
        //    get { return _mouseWorldPosition; }
        //    set
        //    {
        //        if (value.Equals(_mouseWorldPosition)) return;
        //        _mouseWorldPosition = value;
        //        RaisePropertyChanged();
        //    }
        //}

        public ObservableObject<PointD?> MousePositionWorld { get; }

        public ImageEditorViewModel() : this(0, 0)
        {
        }

        public ImageEditorViewModel(
            int initialImageWidth,
            int initialImageHeight)
        {
            ImageWidth = initialImageWidth;
            ImageHeight = initialImageHeight;
            ScrollOffset = new PointD { X = 0, Y = 0 };
            ScrollOffset = new PointD { X = 0, Y = 0 };
            Magnification = 1;
            MousePositionWorld = new ObservableObject<PointD?>();
        }
    }
}
