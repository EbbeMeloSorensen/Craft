using Craft.DataStructures.Geometry;
using Craft.Math;
using Craft.Math.IO;
using Craft.UIElements.Geometry2D.Reborn;
using Craft.Utils.Linq;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace Craft.UIElements.Reborn.GuiTest
{
    public class MainWindowViewModel : INotifyPropertyChanged, IFrameAware
    {
        private IGeometryDataStore _geometryDataStore;
        private string _drawingLabel = string.Empty;

        public string DrawingLabel
        {
            get => _drawingLabel;
            set
            {
                _drawingLabel = value;
                OnPropertyChanged();
            }
        }

        private string _drawingLabelNumber = string.Empty;

        public string DrawingLabelNumber
        {
            get => _drawingLabelNumber;
            set
            {
                if (!IsValidLabelNumber(value))
                    throw new ArgumentException("Enter a non-negative integer or leave the number empty.", nameof(value));

                _drawingLabelNumber = value;
                OnPropertyChanged();
            }
        }

        public static bool IsValidLabelNumber(string value) =>
            value != null && value.All(character => character >= '0' && character <= '9');

        private string CompletedDrawingLabel => DrawingLabel + DrawingLabelNumber;

        private void IncrementDrawingLabelNumber()
        {
            if (DrawingLabelNumber.Length > 0)
                DrawingLabelNumber = (BigInteger.Parse(DrawingLabelNumber, CultureInfo.InvariantCulture) + BigInteger.One)
                    .ToString(CultureInfo.InvariantCulture);
        }

        private string _selectedObjectLabel = string.Empty;

        private object? EditableSelectedObject =>
            GeometryViewModel.CanvasMode == CanvasMode.Select &&
            GeometryViewModel.SelectedGeometricObjects.Count == 1
                ? GeometryViewModel.SelectedGeometricObjects[0]
                : null;

        public bool CanEditSelectedLabel => EditableSelectedObject is Point2D or LineSegment2D;

        public string SelectedObjectLabel
        {
            get => _selectedObjectLabel;
            set
            {
                _selectedObjectLabel = value;
                OnPropertyChanged();
                NotifyLabelDraftChanged();
            }
        }

        public bool IsSelectedLabelDirty => CanEditSelectedLabel &&
            (SelectedObjectLabel != GetObjectLabel(EditableSelectedObject) ||
             !SelectedObjectInformation.Select(item => item.Text)
                 .SequenceEqual(GetObjectLabels(EditableSelectedObject).Skip(1)));

        public bool CanAddSelectedInformation => CanEditSelectedLabel && !string.IsNullOrWhiteSpace(SelectedObjectLabel);

        public string SelectedLabelValidationMessage => !CanEditSelectedLabel ? string.Empty
            : SelectedObjectInformation.Count > 0 && string.IsNullOrWhiteSpace(SelectedObjectLabel)
                ? "Enter an identifier or remove all information rows."
                : SelectedObjectInformation.Any(item => string.IsNullOrWhiteSpace(item.Text))
                    ? "Fill in or remove blank information rows."
                    : string.Empty;

        public bool CanApplySelectedLabel => IsSelectedLabelDirty && SelectedLabelValidationMessage.Length == 0;

        private readonly HashSet<LabelInformationViewModel> _observedInformation = new();

        private void NotifyLabelDraftChanged()
        {
            OnPropertyChanged(nameof(IsSelectedLabelDirty));
            OnPropertyChanged(nameof(CanAddSelectedInformation));
            OnPropertyChanged(nameof(CanApplySelectedLabel));
            OnPropertyChanged(nameof(SelectedLabelValidationMessage));
            AddSelectedInformationCommand.RaiseCanExecuteChanged();
            ApplySelectedLabelCommand.RaiseCanExecuteChanged();
            CancelSelectedLabelCommand.RaiseCanExecuteChanged();
        }

        private void SelectedInformationChanged()
        {
            // Clear/reset events do not supply removed items, so track the subscribed rows.
            foreach (var item in _observedInformation.ToArray())
            {
                if (!SelectedObjectInformation.Contains(item))
                {
                    item.PropertyChanged -= InformationTextChanged;
                    _observedInformation.Remove(item);
                }
            }
            foreach (var item in SelectedObjectInformation)
            {
                if (_observedInformation.Add(item))
                    item.PropertyChanged += InformationTextChanged;
            }
            NotifyLabelDraftChanged();
            RemoveSelectedInformationCommand.RaiseCanExecuteChanged();
        }

        private void InformationTextChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LabelInformationViewModel.Text))
                NotifyLabelDraftChanged();
        }

        public RelayCommand ApplySelectedLabelCommand { get; }
        public RelayCommand CancelSelectedLabelCommand { get; }

        public ObservableCollection<LabelInformationViewModel> SelectedObjectInformation { get; } = new();
        public RelayCommand AddSelectedInformationCommand { get; }
        public RelayCommand<LabelInformationViewModel> RemoveSelectedInformationCommand { get; }

        private static IReadOnlyList<string> GetObjectLabels(object? geometry) =>
            geometry is ILabeledGeometry labeled ? labeled.Labels : Array.Empty<string>();

        private static string GetObjectLabel(object? geometry) =>
            geometry is ILabeledGeometry labeled ? labeled.Identifier : string.Empty;

        private void RefreshSelectedLabel()
        {
            SelectedObjectLabel = GetObjectLabel(EditableSelectedObject);
            SelectedObjectInformation.Clear();
            foreach (var text in GetObjectLabels(EditableSelectedObject).Skip(1))
                SelectedObjectInformation.Add(new LabelInformationViewModel(text));
            OnPropertyChanged(nameof(CanEditSelectedLabel));
            NotifyLabelDraftChanged();
            AddSelectedInformationCommand.RaiseCanExecuteChanged();
            RemoveSelectedInformationCommand.RaiseCanExecuteChanged();
        }

        private void ApplySelectedLabel()
        {
            if (!CanApplySelectedLabel)
                return;

            var original = EditableSelectedObject!;
            var label = string.IsNullOrWhiteSpace(SelectedObjectLabel) ? string.Empty : SelectedObjectLabel;
            var labels = new[] { label }.Concat(SelectedObjectInformation.Select(item => item.Text)).ToArray();
            var hasLabels = label.Length > 0 || SelectedObjectInformation.Count > 0;
            var originalLabels = GetObjectLabels(original);
            if (hasLabels ? labels.SequenceEqual(originalLabels) : originalLabels.Count == 0)
            {
                RefreshSelectedLabel();
                return;
            }

            // Labels are immutable. Replace the stored object while preserving its geometry.
            object replacement = original switch
            {
                OrientedPoint2D point => !hasLabels
                    ? new OrientedPoint2D(point.X, point.Y, point.AngleDegrees)
                    : new LabeledOrientedPoint2D(point.X, point.Y, point.AngleDegrees, labels),
                Point2D point => !hasLabels
                    ? new Point2D(point.X, point.Y)
                    : new LabeledPoint2D(point.X, point.Y, labels),
                LineSegment2D segment => !hasLabels
                    ? new LineSegment2D(segment.Point1, segment.Point2)
                    : new LabeledLineSegment2D(segment.Point1, segment.Point2, labels),
                _ => throw new InvalidOperationException("Unsupported geometry type.")
            };
            var bounds = replacement switch
            {
                Point2D point => point.ComputeBoundingBox(),
                LineSegment2D segment => segment.ComputeBoundingBox(),
                _ => throw new InvalidOperationException("Unsupported geometry type.")
            };

            _geometryDataStore.AddGeometricObject(replacement, bounds);
            _geometryDataStore.RemoveGeometricObjects(new[] { original });
            GeometryViewModel.SelectedGeometricObjects[0] = replacement;
            UpdateStaticGeometryLayer();
        }

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

        public IReadOnlyList<string> SnapSpacingPresets { get; } = new[] { "0.1", "0.25", "0.5", "1", "5", "10", "50" };

        public string GridSpacing
        {
            get => _gridSpacing;
            set
            {
                if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var gridSpacing) ||
                    !double.IsFinite(gridSpacing) || gridSpacing <= 0)
                    throw new ArgumentException("Enter a positive spacing, for example 1, 0.1 or 0.01 (use a decimal point).");

                _gridSpacing = value;
                GeometryViewModel.GridSpacing = gridSpacing;

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

            ApplySelectedLabelCommand = new RelayCommand(ApplySelectedLabel, () => CanApplySelectedLabel);
            CancelSelectedLabelCommand = new RelayCommand(RefreshSelectedLabel, () => IsSelectedLabelDirty);
            AddSelectedInformationCommand = new RelayCommand(
                () => { if (CanAddSelectedInformation) SelectedObjectInformation.Add(new LabelInformationViewModel()); }, () => CanAddSelectedInformation);
            RemoveSelectedInformationCommand = new RelayCommand<LabelInformationViewModel>(
                item => SelectedObjectInformation.Remove(item),
                item => CanEditSelectedLabel && item != null && SelectedObjectInformation.Contains(item));
            SelectedObjectInformation.CollectionChanged += (_, _) => SelectedInformationChanged();
            GeometryViewModel.SelectedGeometricObjects.CollectionChanged += (_, _) => RefreshSelectedLabel();
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
            GridSpacing = GeometryViewModel.GridSpacing.ToString(CultureInfo.InvariantCulture);
        }

        private void GeometryViewModel_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GeometryViewModel.CanvasMode))
            {
                RefreshSelectedLabel();
            }
            else if (e.PropertyName == nameof(GeometryViewModel.WorldWindowExpanded))
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

                var geometricObjects = GetGeometryCandidates(bbox);

                var clickedPosition = new Point2D(
                    GeometryViewModel.ClickedWorldPosition.Value.X,
                    GeometryViewModel.ClickedWorldPosition.Value.Y);

                object closestGeometricObject = null;
                var squaredDistanceToClosestGeometricObject = double.NaN;

                foreach (var geometricObject in geometricObjects)
                {
                    switch (geometricObject)
                    {
                        case OrientedPoint2D orientedPoint:
                            var squaredDistanceToArrow = GetSelectionShaft(orientedPoint).SquaredDistanceTo(clickedPosition);
                            if (closestGeometricObject == null || squaredDistanceToArrow < squaredDistanceToClosestGeometricObject)
                            {
                                closestGeometricObject = orientedPoint;
                                squaredDistanceToClosestGeometricObject = squaredDistanceToArrow;
                            }
                            break;

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

                var geometricObjects = GetGeometryCandidates(GeometryViewModel.SelectionWindow);

                foreach (var geometricObject in geometricObjects)
                {
                    switch (geometricObject)
                    {
                        case OrientedPoint2D orientedPoint:
                            if (GeometryViewModel.SelectionWindow.Encloses(GetSelectionShaft(orientedPoint).ComputeBoundingBox()) &&
                                !GeometryViewModel.SelectedGeometricObjects.Contains(orientedPoint))
                                GeometryViewModel.SelectedGeometricObjects.Add(orientedPoint);
                            break;

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
            else if (e.PropertyName == nameof(GeometryViewModel.DrawnOrientedPoint))
            {
                var arrow = GeometryViewModel.DrawnOrientedPoint;
                if (arrow != null)
                {
                    var label = CompletedDrawingLabel;
                    if (!string.IsNullOrWhiteSpace(label))
                        arrow = new LabeledOrientedPoint2D(arrow.X, arrow.Y, arrow.AngleDegrees, label);
                    _geometryDataStore.AddGeometricObject(arrow, arrow.ComputeBoundingBox());
                    IncrementDrawingLabelNumber();
                    UpdateStaticGeometryLayer();
                }
            }
            else if (e.PropertyName == nameof(GeometryViewModel.DrawingStrokePoints))
            {
                if (GeometryViewModel.DrawingStrokePoints == null || !GeometryViewModel.DrawingStrokePoints.Any())
                {
                    return;
                }

                var label = CompletedDrawingLabel;
                if (GeometryViewModel.DrawingStrokePoints.Skip(1).Any())
                {
                    // The user has finished a poly line
                    GeometryViewModel.DrawingStrokePoints.AdjacentPairs().ToList().ForEach(_ =>
                    {
                        var line = new LineSegment2D(
                            new Point2D(_.Item1.X, _.Item1.Y),
                            new Point2D(_.Item2.X, _.Item2.Y));

                        if (!string.IsNullOrWhiteSpace(label))
                            line = new LabeledLineSegment2D(line.Point1, line.Point2, label);
                        _geometryDataStore.AddGeometricObject(line, line.ComputeBoundingBox());
                    });
                }
                else
                {
                    // The user has finished a point
                    var temp = GeometryViewModel.DrawingStrokePoints.First();
                    Point2D point = string.IsNullOrWhiteSpace(label)
                        ? new Point2D(temp.X, temp.Y)
                        : new LabeledPoint2D(temp.X, temp.Y, label);
                    _geometryDataStore.AddGeometricObject(point, point.ComputeBoundingBox());
                }

                IncrementDrawingLabelNumber();
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

            var json = GeometryFile.Serialize(_geometryDataStore.GetAll());

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
                var geometricObjects = GeometryFile.Deserialize(jsonData);

                foreach (var geometricObject in geometricObjects)
                {
                    var bbox = geometricObject switch
                    {
                        Point2D point => point.ComputeBoundingBox(),
                        LineSegment2D segment => segment.ComputeBoundingBox(),
                        _ => throw new InvalidDataException("Unsupported geometry type.")
                    };
                    _geometryDataStore.AddGeometricObject(geometricObject, bbox);
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

        // Arrows have viewport-sized bounds, so their stored world bounds cannot cull them.
        private IEnumerable<object> GetGeometryCandidates(BoundingBox window)
            => _geometryDataStore.GetIntersecting(window).Cast<object>()
                .Where(item => item is not OrientedPoint2D)
                .Concat(_geometryDataStore.GetAll().Cast<object>().OfType<OrientedPoint2D>());

        private LineSegment2D GetSelectionShaft(OrientedPoint2D point)
            => point.GetWorldShaft(GeometryViewModel.ViewState.Scaling.Width,
                GeometryViewModel.ViewState.Scaling.Height);
        private void UpdateStaticGeometryLayer()
        {
            var geometricObjects = GetGeometryCandidates(GeometryViewModel.WorldWindowExpanded);

            GeometryViewModel.ClearLayer(false);

            GeometryViewModel.AddStaticGeometryLayer(
                geometricObjects);
        }
    }
}
