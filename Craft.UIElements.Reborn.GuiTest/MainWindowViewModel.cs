using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace Craft.UIElements.Reborn.GuiTest
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _requestedWwXMin;
        private string _requestedWwXMax;
        private string _requestedWwYMin;
        private string _requestedWwYMax;

        private string _requestedWwFocusX;
        private string _requestedWwFocusY;
        private string _requestedWwFocusRatioX;
        private string _requestedWwFocusRatioY;

        private string _focusShiftDamping;

        public string RequestedWW_XMin
        {
            get => _requestedWwXMin;
            set
            {
                _requestedWwXMin = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_XMax
        {
            get => _requestedWwXMax;
            set
            {
                _requestedWwXMax = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_YMin
        {
            get => _requestedWwYMin;
            set
            {
                _requestedWwYMin = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_YMax
        {
            get => _requestedWwYMax;
            set
            {
                _requestedWwYMax = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_FocusX
        {
            get => _requestedWwFocusX;
            set
            {
                _requestedWwFocusX = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_FocusY
        {
            get => _requestedWwFocusY;
            set
            {
                _requestedWwFocusY = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_FocusRatioX
        {
            get => _requestedWwFocusRatioX;
            set
            {
                _requestedWwFocusRatioX = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_FocusRatioY
        {
            get => _requestedWwFocusRatioY;
            set
            {
                _requestedWwFocusRatioY = value;
                OnPropertyChanged();
            }
        }

        public string FocusShiftDamping
        {
            get => _focusShiftDamping;
            set
            {
                _focusShiftDamping = value;

                if (double.TryParse(_focusShiftDamping, CultureInfo.InvariantCulture, out var focusShiftDamping))
                {
                    GeometryViewModel.FocusShiftDamping = focusShiftDamping;
                }

                OnPropertyChanged();
            }
        }

        public ICommand SetWorldWindowCommand { get; }
        public ICommand SetWorldFocusCommand { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            var centerOfHouse = new Point(200, 150);
            var worldWindowBoundsSize = new Size(2000, 2000);

            GeometryViewModel = new GeometryViewModel
            {
                WorldWindowBounds = new BoundingBox(
                    centerOfHouse.X - worldWindowBoundsSize.Width / 2,
                    centerOfHouse.X + worldWindowBoundsSize.Width / 2,
                    centerOfHouse.Y - worldWindowBoundsSize.Height / 2,
                    centerOfHouse.Y + worldWindowBoundsSize.Height / 2)
            };

            SetWorldWindowCommand = new RelayCommand(SetWorldWindow);
            SetWorldFocusCommand = new RelayCommand(SetWorldFocus);

            // Default values for the world window input fields
            //RequestedWW_XMin = "195.0";
            //RequestedWW_XMax = "405.0";
            ////RequestedWW_YMin = "195.0";
            //RequestedWW_YMin = "-5.0";
            //RequestedWW_YMax = "305.0";

            RequestedWW_XMin = "0";
            RequestedWW_XMax = "2000";
            RequestedWW_YMin = "0";
            RequestedWW_YMax = "2000";

            RequestedWW_FocusX = "200";
            RequestedWW_FocusY = "300";
            RequestedWW_FocusRatioX = "0.5";
            RequestedWW_FocusRatioY = "0.5";

            FocusShiftDamping = GeometryViewModel.FocusShiftDamping.ToString();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void SetWorldWindow()
        {
            if (double.TryParse(RequestedWW_XMin, CultureInfo.InvariantCulture, out var xMin) &&
                double.TryParse(RequestedWW_XMax, CultureInfo.InvariantCulture, out var xMax) &&
                double.TryParse(RequestedWW_YMin, CultureInfo.InvariantCulture, out var yMin) &&
                double.TryParse(RequestedWW_YMax, CultureInfo.InvariantCulture, out var yMax))
            {
                GeometryViewModel.RequestedWorldWindow = new BoundingBox(xMin, xMax, yMin, yMax);
            }
        }

        private void SetWorldFocus()
        {
            if (double.TryParse(RequestedWW_FocusX, CultureInfo.InvariantCulture, out var focusX) &&
                double.TryParse(RequestedWW_FocusY, CultureInfo.InvariantCulture, out var focusY) &&
                double.TryParse(RequestedWW_FocusRatioX, CultureInfo.InvariantCulture, out var ratioX) &&
                double.TryParse(RequestedWW_FocusRatioY, CultureInfo.InvariantCulture, out var ratioY))
            {
                GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
                {
                    ViewportRatio = new Size(ratioX, ratioY),
                    WorldPoint = new Point(focusX, focusY)
                };
            }
        }
    }
}
