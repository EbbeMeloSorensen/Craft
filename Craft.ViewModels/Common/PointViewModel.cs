using System;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Common
{
    public class PointViewModel : ViewModelBase
    {
        private int _index;
        private PointD _point;
        private double _left;
        private double _top;
        private double _diameter;
        private string _label;
        private Brush _brush;
        private RelayCommand _clickedCommand;

        public PointD Point
        {
            get { return _point; }
            set
            {
                _point = value;
                UpdateTopLeft();
            }
        }

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;
                RaisePropertyChanged();
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged();
            }
        }

        public Brush Brush
        {
            get => _brush;
            set
            {
                _brush = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ClickedCommand
        {
            get
            {
                return _clickedCommand ?? (_clickedCommand = new RelayCommand(Clicked));
            }
        }

        public event EventHandler<ElementClickedEventArgs> ElementClicked;

        public PointViewModel(
            PointD point,
            int index,
            double diameter)
        {
            Point = point;
            _index = index;
            _diameter = diameter;

            UpdateTopLeft();
        }

        private void Clicked()
        {
            OnElementClicked();
        }

        private void OnElementClicked()
        {
            var handler = ElementClicked;

            if (handler != null)
            {
                handler(this, new ElementClickedEventArgs(_index));
            }
        }

        private void UpdateTopLeft()
        {
            Left = Point.X - _diameter / 2;
            Top = Point.Y - _diameter / 2;
        }
    }
}
