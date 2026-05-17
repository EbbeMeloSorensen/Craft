using Craft.DataStructures.Geometry;
using Craft.UIElements.Geometry2D.Reborn;
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
    public class MainWindowViewModel : INotifyPropertyChanged, IFrameAware
    {
        private string _requestedWwXMin;
        private string _requestedWwXMax;
        private string _requestedWwYMin;
        private string _requestedWwYMax;
        private DateTime? _requestedStartDate;
        private DateTime? _requestedEndDate;

        private string _requestedWwFocusX;
        private string _requestedWwFocusY;
        private string _requestedWwFocusRatioX;
        private string _requestedWwFocusRatioY;
        private string _requestedWwScalingX;
        private string _requestedWwScalingY;

        private string _focusShiftDamping;
        private bool _continuallyMoveFocus;

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

        public DateTime? RequestedStartDate
        {
            get => _requestedStartDate;
            set
            {
                _requestedStartDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? RequestedEndDate
        {
            get => _requestedEndDate;
            set
            {
                _requestedEndDate = value;
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

        public string RequestedWW_ScalingX
        {
            get => _requestedWwScalingX;
            set
            {
                _requestedWwScalingX = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWW_ScalingY
        {
            get => _requestedWwScalingY;
            set
            {
                _requestedWwScalingY = value;
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

        public bool ContinuallyMoveFocus
        {
            get => _continuallyMoveFocus;
            set
            {
                _continuallyMoveFocus = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetWorldWindowCommand { get; }
        public ICommand SetTimeIntervalCommand { get; }
        public ICommand SetWorldFocusCommand { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            var centerOfHouse = new Point(200, 150);
            var worldWindowBoundsSize = new Size(100000, 100000);

            GeometryViewModel = new GeometryViewModel
            {
                //WorldWindowBounds = new BoundingBox(
                //    centerOfHouse.X - worldWindowBoundsSize.Width / 2,
                //    centerOfHouse.X + worldWindowBoundsSize.Width / 2,
                //    centerOfHouse.Y - worldWindowBoundsSize.Height / 2,
                //    centerOfHouse.Y + worldWindowBoundsSize.Height / 2),
                WorldWindowBounds = new BoundingBox(
                    double.MinValue,
                    double.MaxValue,
                    double.MinValue,
                    double.MaxValue),
                ShowCoordinateSystem = true,
                LockAspectRatio = true,
                DampFocusShifts = false,
                TimeAxisMode = false,
                FocusShiftDamping = 5.0
            };

            SetWorldWindowCommand = new RelayCommand(SetWorldWindow);
            SetTimeIntervalCommand = new RelayCommand(SetTimeInterval);
            SetWorldFocusCommand = new RelayCommand(SetWorldFocus);

            // Default values for the world window input fields
            //RequestedWW_XMin = "195.0";
            //RequestedWW_XMax = "405.0";
            ////RequestedWW_YMin = "195.0";
            //RequestedWW_YMin = "-5.0";
            //RequestedWW_YMax = "305.0";

            //RequestedWW_XMin = "0";
            //RequestedWW_XMax = "2000";
            //RequestedWW_YMin = "0";
            //RequestedWW_YMax = "2000";

            var ticksInAYear = TimeSpan.FromDays(365).Ticks;
            var ticksInAWeek = TimeSpan.FromDays(7).Ticks;
            var ticksInADay = TimeSpan.FromDays(1).Ticks;
            var ticksInAnHour = TimeSpan.FromHours(1).Ticks;

            //RequestedWW_XMin = (-ticksInAYear * 5).ToString();
            //RequestedWW_XMax = (ticksInAYear * 5).ToString();
            //RequestedWW_XMin = (ticksInADay * 26).ToString();
            //RequestedWW_XMax = (ticksInADay * 44).ToString();
            RequestedWW_XMin = (ticksInAYear * -975).ToString();
            RequestedWW_XMax = (ticksInAYear * 25).ToString();
            RequestedWW_YMin = "-100";
            RequestedWW_YMax = "100";

            RequestedStartDate = new DateTime(1975, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            RequestedEndDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            RequestedWW_FocusX = "200";
            RequestedWW_FocusY = "150";
            RequestedWW_FocusRatioX = "0.5";
            RequestedWW_FocusRatioY = "0.5";
            RequestedWW_ScalingX = "1";
            RequestedWW_ScalingY = "1";

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

        private void SetTimeInterval()
        {
            var xMin = TimeCoordinates.ToWorldTicks(RequestedStartDate.Value);
            var xMax = TimeCoordinates.ToWorldTicks(RequestedEndDate.Value);

            var yMin = GeometryViewModel.WorldWindow.MinY;
            var yMax = GeometryViewModel.WorldWindow.MaxY;

            GeometryViewModel.RequestedWorldWindow = new BoundingBox(xMin, xMax, yMin, yMax);
        }

        private void SetWorldFocus()
        {
            if (double.TryParse(RequestedWW_FocusX, CultureInfo.InvariantCulture, out var focusX) &&
                double.TryParse(RequestedWW_FocusY, CultureInfo.InvariantCulture, out var focusY) &&
                double.TryParse(RequestedWW_FocusRatioX, CultureInfo.InvariantCulture, out var ratioX) &&
                double.TryParse(RequestedWW_FocusRatioY, CultureInfo.InvariantCulture, out var ratioY) &&
                double.TryParse(RequestedWW_ScalingX, CultureInfo.InvariantCulture, out var scalingX) &&
                double.TryParse(RequestedWW_ScalingY, CultureInfo.InvariantCulture, out var scalingY))
            {
                GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
                {
                    ViewportRatio = new Size(ratioX, ratioY),
                    WorldPoint = new Point(focusX, focusY),
                    Scaling = new Size(scalingX, scalingY)
                };
            }
        }

        // Called for each frame
        public void OnFrame(
            TimeSpan time,
            double dt)
        {
            if (ContinuallyMoveFocus)
            {
                // Update simulation (like when it was a game - not doing that yet)
                //Update(deltaSeconds);

                // Update camera
                GeometryViewModel.RequestedWorldFocus = ComputeCamera(time);
            }
        }

        private WorldFocusRequest ComputeCamera(
            TimeSpan time)
        {
            var x = time.TotalSeconds * 50.0;
            var y = 150.0;

            var worldFocusRequest = new WorldFocusRequest
            {
                WorldPoint = new Point(x, y),
                ViewportRatio = new Size(0.5, 0.5)
            };

            return worldFocusRequest;
        }
    }
}
