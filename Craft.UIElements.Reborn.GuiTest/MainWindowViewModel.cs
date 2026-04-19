using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace Craft.UIElements.Reborn.GuiTest
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Radius and center of circular motion
        private const double Radius = 200;
        private static readonly Point Center = new Point(100, 100);

        private readonly DispatcherTimer _timer;
        private double _angle;

        private string _requestedWwXMin;
        private string _requestedWwXMax;
        private string _requestedWwYMin;
        private string _requestedWwYMax;

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

        public ICommand SetWorldWindowCommand { get; }
        public ICommand SetWorldFocusCommand { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            GeometryViewModel = new GeometryViewModel();

            SetWorldWindowCommand = new RelayCommand(SetWorldWindow);
            SetWorldFocusCommand = new RelayCommand(SetWorldFocus);

            // Default values for the world window input fields
            RequestedWW_XMin = "-200.0";
            RequestedWW_XMax = "200.0";
            RequestedWW_YMin = "-200.0";
            RequestedWW_YMax = "200.0";

            // Timer: ~60 FPS
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(32)
                //Interval = TimeSpan.FromMilliseconds(16)
                //Interval = TimeSpan.FromMilliseconds(1)
            };

            // Uncomment to enable continuous rotation of the world focus
            //_timer.Tick += OnTick;
            //_timer.Start();
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
            // Todo: Get it from the gui

            GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
            {
                ViewportRatio = new Size(1.0, 1.0),
                WorldPoint = new Point(200.0, 300.0)
            };
        }

        private void OnTick(object sender, EventArgs e)
        {
            _angle += 0.05; // speed of rotation

            var x = Center.X + Radius * System.Math.Cos(_angle);
            var y = Center.Y + Radius * System.Math.Sin(_angle);

            GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
            {
                ViewportRatio = new Size(0.5, 0.5),
                WorldPoint = new Point(x, y)
            };
        }
    }
}
