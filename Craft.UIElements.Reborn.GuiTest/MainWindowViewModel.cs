using Craft.DataStructures.Geometry;
using Craft.IO.Utils;
using Craft.Math;
using Craft.UIElements.Geometry2D.Reborn;
using Craft.Utils.Linq;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace Craft.UIElements.Reborn.GuiTest
{
    public class MainWindowViewModel : INotifyPropertyChanged, IFrameAware
    {
        private IGeometryDataStore _geometryDataStore;

        private string _requestedWwBoundsXMin;
        private string _requestedWwBoundsXMax;
        private string _requestedWwBoundsYMin;
        private string _requestedWwBoundsYMax;
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
        private string _gridSpacing;
        private bool _continuallyMoveFocus;

        public string RequestedWWBounds_XMin
        {
            get => _requestedWwBoundsXMin;
            set
            {
                _requestedWwBoundsXMin = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWWBounds_XMax
        {
            get => _requestedWwBoundsXMax;
            set
            {
                _requestedWwBoundsXMax = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWWBounds_YMin
        {
            get => _requestedWwBoundsYMin;
            set
            {
                _requestedWwBoundsYMin = value;
                OnPropertyChanged();
            }
        }

        public string RequestedWWBounds_YMax
        {
            get => _requestedWwBoundsYMax;
            set
            {
                _requestedWwBoundsYMax = value;
                OnPropertyChanged();
            }
        }

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

        public string GridSpacing
        {
            get => _gridSpacing;
            set
            {
                _gridSpacing = value;

                if (double.TryParse(_gridSpacing, CultureInfo.InvariantCulture, out var gridSpacing))
                {
                    GeometryViewModel.GridSpacing = gridSpacing;
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

        public ICommand SetWorldWindowBoundsCommand { get; }
        public ICommand SetWorldWindowCommand { get; }
        public ICommand SetTimeIntervalCommand { get; }
        public ICommand SetWorldFocusCommand { get; }
        public ICommand SaveGeometryCommand { get; }
        public ICommand LoadGeometryCommand { get; }
        public ICommand ChangeCanvasModeCommand { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            //var geometryDataSource = new EmptyDataSource();
            //var geometryDataSource = new SimpleGeometryDataSource();
            //var geometryDataSource = new FunctionCurveDataSource();
            //_geometryDataSource = new MxCifQuadTreeGeometryDataSource(
            //    new BoundingBox(-2000, 2000, -2000, 2000), 8);

            //_geometryDataSource = new TimeStampDataSource();
            //_geometryDataSource = new TemperatureDataSource();

            _geometryDataStore = new MxCifQuadTreeGeometryDataStore(
                new BoundingBox(-2000, 2000, -2000, 2000), 8);

            GeometryViewModel = new GeometryViewModel()
            {
                ShowCoordinateSystem = true,
                LockAspectRatio = true,
                DampFocusShifts = false,
                TimeAxisMode = false,
                FocusShiftDamping = 5.0,
                GridSpacing = 50.0
            };

            GeometryViewModel.PropertyChanged += GeometryViewModel_PropertyChanged;

            SetWorldWindowBoundsCommand = new RelayCommand(SetWorldWindowBounds);
            SetWorldWindowCommand = new RelayCommand(SetWorldWindow);
            SetTimeIntervalCommand = new RelayCommand(SetTimeInterval);
            SetWorldFocusCommand = new RelayCommand(SetWorldFocus);
            SaveGeometryCommand = new RelayCommand(SaveGeometry);
            LoadGeometryCommand = new RelayCommand(LoadGeometry);
            ChangeCanvasModeCommand = new RelayCommand<object>(ChangeCanvasMode);

            // Default values for the world window bounds input fields
            RequestedWWBounds_XMin = "-300";
            RequestedWWBounds_XMax = "700";
            RequestedWWBounds_YMin = "-300";
            RequestedWWBounds_YMax = "700";

            // Default values for the world window input fields
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

            // Default values for the date range input fields
            RequestedStartDate = new DateTime(1975, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            RequestedEndDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Default values for the date focus input fields
            RequestedWW_FocusX = "200";
            RequestedWW_FocusY = "150";
            RequestedWW_FocusRatioX = "0.5";
            RequestedWW_FocusRatioY = "0.5";
            RequestedWW_ScalingX = "1";
            RequestedWW_ScalingY = "1";

            FocusShiftDamping = GeometryViewModel.FocusShiftDamping.ToString();
            GridSpacing = GeometryViewModel.GridSpacing.ToString();
        }

        private void GeometryViewModel_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GeometryViewModel.WorldWindowExpanded))
            {
                UpdateStaticGeometryLayer();
            }
            else if (e.PropertyName == nameof(GeometryViewModel.ClickedWorldPosition))
            {
                if (GeometryViewModel.ClickedWorldPosition == null)
                {
                    return;
                }

                // Make a bounding box that takes the current magnification into account
                var x = GeometryViewModel.ClickedWorldPosition.Value.X;
                var y = GeometryViewModel.ClickedWorldPosition.Value.Y;
                var sX = GeometryViewModel.ViewState.Scaling.Width;
                var sY = GeometryViewModel.ViewState.Scaling.Height;

                var bbHalfWidth = 4 / System.Math.Min(sX, sY);

                var bbox = new BoundingBox(
                    x - bbHalfWidth,
                    x + bbHalfWidth,
                    y - bbHalfWidth,
                    y + bbHalfWidth);

                var geometricObjects = _geometryDataStore.GetIntersecting(bbox);

                var clickedPosition = new Point2D(
                    GeometryViewModel.ClickedWorldPosition.Value.X,
                    GeometryViewModel.ClickedWorldPosition.Value.Y);

                object closestGeometricObject = null;
                var squaredDistanceToClosestGeometricObject = double.NaN;

                foreach (var geometricObject in geometricObjects)
                {
                    switch (geometricObject)
                    {
                        case Point2D point2D:
                            var squaredDistanceToPoint = point2D.SquaredDistanceTo(clickedPosition);

                            if (closestGeometricObject == null ||
                                squaredDistanceToPoint < squaredDistanceToClosestGeometricObject)
                            {
                                closestGeometricObject = geometricObject;
                                squaredDistanceToClosestGeometricObject = squaredDistanceToPoint;
                            }

                            break;

                        case LineSegment2D lineSegment2D:
                            var squaredDistanceToLine = lineSegment2D.SquaredDistanceTo(clickedPosition);

                            if (closestGeometricObject == null ||
                                squaredDistanceToLine < squaredDistanceToClosestGeometricObject)
                            {
                                closestGeometricObject = geometricObject;
                                squaredDistanceToClosestGeometricObject = squaredDistanceToLine;
                            }

                            break;

                        default:
                            throw new InvalidDataException("unsupported geometric object type");
                    }
                }

                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    // No modifier keys pressed, so clear any existing selection
                    GeometryViewModel.SelectedGeometricObjects.Clear();
                }

                if (closestGeometricObject != null && squaredDistanceToClosestGeometricObject < bbHalfWidth * bbHalfWidth)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        if (GeometryViewModel.SelectedGeometricObjects.Contains(closestGeometricObject))
                        {
                            GeometryViewModel.SelectedGeometricObjects.Remove(closestGeometricObject);
                        }
                        else
                        {
                            GeometryViewModel.SelectedGeometricObjects.Add(closestGeometricObject);
                        }
                    }
                    else
                    {
                        if (!GeometryViewModel.SelectedGeometricObjects.Contains(closestGeometricObject))
                        {
                            GeometryViewModel.SelectedGeometricObjects.Add(closestGeometricObject);
                        }
                    }
                }
            }
            else if (e.PropertyName == nameof(GeometryViewModel.SelectionWindow))
            {
                if (GeometryViewModel.SelectionWindow == null)
                {
                    return;
                }

                if (!Keyboard.IsKeyDown(Key.LeftShift))
                {
                    // Shift key is not pressed, so clear any existing selection
                    GeometryViewModel.SelectedGeometricObjects.Clear();
                }

                var geometricObjects = _geometryDataStore.GetIntersecting(GeometryViewModel.SelectionWindow);

                foreach (var geometricObject in geometricObjects)
                {
                    switch (geometricObject)
                    {
                        case Point2D point2D:

                            if (GeometryViewModel.SelectionWindow.Encloses(point2D.ComputeBoundingBox()))
                            {
                                if (!GeometryViewModel.SelectedGeometricObjects.Contains(point2D))
                                {
                                    GeometryViewModel.SelectedGeometricObjects.Add(point2D);
                                }
                            }

                            break;

                        case LineSegment2D lineSegment2D:

                            if (GeometryViewModel.SelectionWindow.Encloses(lineSegment2D.ComputeBoundingBox()))
                            {
                                if (!GeometryViewModel.SelectedGeometricObjects.Contains(lineSegment2D))
                                {
                                    GeometryViewModel.SelectedGeometricObjects.Add(lineSegment2D);
                                }
                            }

                            break;

                        default:
                            throw new InvalidDataException("unsupported geometric object type");
                    }
                }
            }
            else if (e.PropertyName == nameof(GeometryViewModel.DrawingStrokePoints))
            {
                if (GeometryViewModel.DrawingStrokePoints == null)
                {
                    return;
                }

                if (GeometryViewModel.DrawingStrokePoints.Skip(1).Any())
                {
                    // The user has finished a poly line
                    GeometryViewModel.DrawingStrokePoints.AdjacentPairs().ToList().ForEach(_ =>
                    {
                        var line = new LineSegment2D(
                            new Point2D(_.Item1.X, _.Item1.Y),
                            new Point2D(_.Item2.X, _.Item2.Y));

                        _geometryDataStore.AddGeometricObject(line, line.ComputeBoundingBox());
                    });
                }
                else
                {
                    // The user has finished a point
                    var temp = GeometryViewModel.DrawingStrokePoints.First();
                    var point = new Point2D(temp.X, temp.Y);
                    _geometryDataStore.AddGeometricObject(point, point.ComputeBoundingBox());
                }

                UpdateStaticGeometryLayer();
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

        public void OnLoaded()
        {
            //GeometryViewModel.RequestedWorldWindow = new BoundingBox(-100, 100, -100, 100);

            GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
            {
                WorldPoint = new Point(0, 0),
                ViewportRatio = new Size(0.5, 0.5),
            };
        }

        public void HandleKeyEvent(
            Key key)
        {
            switch (key)
            {
                case Key.Delete:

                    if (GeometryViewModel.SelectedGeometricObjects.Any())
                    {
                        _geometryDataStore.RemoveGeometricObjects(
                            GeometryViewModel.SelectedGeometricObjects);

                        GeometryViewModel.SelectedGeometricObjects.Clear();

                        UpdateStaticGeometryLayer();
                    }

                    break;
            }
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void SetWorldWindowBounds()
        {
            if (double.TryParse(RequestedWWBounds_XMin, CultureInfo.InvariantCulture, out var xMin) &&
                double.TryParse(RequestedWWBounds_XMax, CultureInfo.InvariantCulture, out var xMax) &&
                double.TryParse(RequestedWWBounds_YMin, CultureInfo.InvariantCulture, out var yMin) &&
                double.TryParse(RequestedWWBounds_YMax, CultureInfo.InvariantCulture, out var yMax))
            {
                GeometryViewModel.WorldWindowBounds = new BoundingBox(xMin, xMax, yMin, yMax);
            }
        }

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

        private void SaveGeometry()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Json Files(*.json)|*.json"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            var lines = new List<LineSegment2D>();

            foreach (var spatialObject in _geometryDataStore.GetAll())
            {
                switch (spatialObject)
                {
                    case LineSegment2D lineSegment2D:
                        lines.Add(lineSegment2D);
                        break;
                }
            }

            var json = JsonConvert.SerializeObject(lines, Formatting.Indented, new DoubleJsonConverter());

            using (var streamWriter = new StreamWriter(dialog.FileName))
            {
                streamWriter.WriteLine(json);
            }
        }

        private void LoadGeometry()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Json Files(*.json)|*.json"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            using (var r = new StreamReader(dialog.FileName))
            {
                var jsonData = r.ReadToEnd();
                var deserializedData = JsonConvert.DeserializeObject<List<LineSegment2D>>(jsonData);

                foreach (var lineSegment2D in deserializedData)
                {
                    var bbox = lineSegment2D.ComputeBoundingBox();

                    _geometryDataStore.AddGeometricObject(lineSegment2D, bbox);
                }

                UpdateStaticGeometryLayer();
            }
        }

        private void ChangeCanvasMode(
            object parameter)
        {
            if (Enum.TryParse((string)parameter, out CanvasMode canvasMode))
            {
                GeometryViewModel.CanvasMode = canvasMode;
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

        private void UpdateStaticGeometryLayer()
        {
            var geometricObjects = _geometryDataStore.GetIntersecting(
                GeometryViewModel.WorldWindowExpanded);

            GeometryViewModel.ClearLayer(false);

            GeometryViewModel.AddStaticGeometryLayer(
                geometricObjects);
        }
    }
}
