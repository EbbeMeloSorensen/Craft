using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public enum ROIAlignment
    {
        Center,
        TopLeft
    }

    public delegate void UpdateModelCallBack();

    public class GeometryEditorViewModel : ViewModelBase
    {
        public int _yAxisFactor;
        private double? _initialScalingX;
        private double? _initialScalingY;
        private Point? _initialWorldWindowFocus;
        private Size? _initialWorldWindowSize;
        private bool _fitAspectRatio;
        private string _imagePath;
        protected Size _viewPortSize;
        protected Size _worldWindowSize;
        private Point _mousePositionViewport;
        private double? _worldWindowUpperLeftOverride_X;
        private double? _worldWindowUpperLeftOverride_Y;
        protected Point _worldWindowUpperLeft;
        private Size _scaling;
        private Matrix _transformationMatrix;
        private double _translationX;
        private double _translationXMin;
        private double _translationXMax;
        private double _translationYMin;
        private double _translationYMax;
        private double _translationY;
        private Brush _defaultBrush;
        private Brush _backgroundBrush;
        private Dictionary<int, ShapeViewModel> _shapeViewModelMap;
        private bool _xAxisLocked;
        private bool _yAxisLocked;
        private bool _aspectRatioLocked;
        private bool _xScalingLocked;
        private bool _yScalingLocked;
        private ROIAlignment _roiAlignment = ROIAlignment.TopLeft;
        private Point _worldWindowUpperLeftLimit;
        private Point _worldWindowBottomRightLimit;
        private bool _selectRegionPossible;
        private bool _selectedRegionLimitedVertically;
        private bool _selectedRegionWindowVisible;

        public RectangleViewModel SelectedRegionWindow { get; }

        public bool SelectedRegionWindowVisible
        {
            get => _selectedRegionWindowVisible;
            set
            {
                _selectedRegionWindowVisible = value;
                RaisePropertyChanged();
            }
        }

        public ObservableObject<Point?> MousePositionWorld { get; }
        public ObservableObject<BoundingBox?> SelectedRegion { get; }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                RaisePropertyChanged();
            }
        }

        public Size ViewPortSize
        {
            get { return _viewPortSize; }
            set
            {
                if (!(value.Width > 0) ||
                    !(value.Height > 0)) return;

                _viewPortSize = value;
                UpdateWorldWindowSize();
                RaisePropertyChanged();

                OnWorldWindowMajorUpdateOccured();
            }
        }

        public Size WorldWindowSize
        {
            get { return _worldWindowSize; }
            private set
            {
                _worldWindowSize = value;
                RaisePropertyChanged();
            }
        }

        public Point MousePositionViewport
        {
            get { return _mousePositionViewport; }
            // The setter is called by the view
            set
            {
                _mousePositionViewport = value;

                RaisePropertyChanged();

                MousePositionWorld.Object = new Point(
                    ConvertViewPortXCoordinateToWorldXCoordinate(_mousePositionViewport.X),
                    ConvertViewPortYCoordinateToWorldYCoordinate(_mousePositionViewport.Y));
            }
        }

        public Point WorldWindowUpperLeftLimit
        {
            get => _worldWindowUpperLeftLimit;
            set
            {
                _worldWindowUpperLeftLimit = value;
                UpdateWorldWindowPositionOverrides();
                EnsureWorldWindowIsWithinLimits();
                UpdateTransformationMatrix();
            }
        }

        public Point WorldWindowBottomRightLimit 
        {
            get => _worldWindowBottomRightLimit; 
            set
            {
                _worldWindowBottomRightLimit = value;
                UpdateWorldWindowPositionOverrides();
                EnsureWorldWindowIsWithinLimits();
                UpdateTransformationMatrix();
            }
        }

        public Point WorldWindowUpperLeft
        {
            get
            {
                if (_worldWindowUpperLeftOverride_X.HasValue)
                {
                    if (_worldWindowUpperLeftOverride_Y.HasValue)
                    {
                        return new Point(
                            _worldWindowUpperLeftOverride_X.Value,
                            _worldWindowUpperLeftOverride_Y.Value);
                    }

                    return new Point(
                        _worldWindowUpperLeftOverride_X.Value,
                        _worldWindowUpperLeft.Y);
                }
                else if (_worldWindowUpperLeftOverride_Y.HasValue)
                {
                    return new Point(
                        _worldWindowUpperLeft.X,
                        _worldWindowUpperLeftOverride_Y.Value);
                }
                
                return _worldWindowUpperLeft;
            }
            set
            {
                _worldWindowUpperLeft = value;

                EnsureWorldWindowIsWithinLimits();
                UpdateTransformationMatrix();
                RaisePropertyChanged();
            }
        }

        public Size Scaling
        {
            get
            {
                return _scaling;
            }
            set
            {
                _scaling = value;

                // Set the Size of the World Window, i.e. not its position. The World Window size only depends on magnification and viewport size
                UpdateWorldWindowSize();

                if (!MousePositionWorld.Object.HasValue)
                {
                    FocusInCenterOfViewPort();
                }

                // Set the position of the World Window. Notice that this also affects the Transformation Matrix
                WorldWindowUpperLeft = new Point(
                    MousePositionWorld.Object.Value.X - _mousePositionViewport.X * _worldWindowSize.Width / _viewPortSize.Width,
                    MousePositionWorld.Object.Value.Y - _mousePositionViewport.Y * _worldWindowSize.Height / _viewPortSize.Height);

                UpdatePoints();
                RaisePropertyChanged();
            }
        }

        public Matrix TransformationMatrix
        {
            get { return _transformationMatrix; }
            private set
            {
                _transformationMatrix = value;
                RaisePropertyChanged();
            }
        }

        // Used exclusively for positioning polylines
        public double TranslationX
        {
            get { return _translationX; }
            set
            {
                if (!XAxisLocked)
                {
                    if (value < _translationXMin)
                    {
                        _translationX = _translationXMin;
                    }
                    else if (value > _translationXMax)
                    {
                        _translationX = _translationXMax;
                    }
                    else
                    {
                        _translationX = value;
                    }

                    RaisePropertyChanged();
                }
            }
        }

        // Used exclusively for positioning polylines
        public double TranslationY
        {
            get { return _translationY; }
            set
            {
                if (!YAxisLocked)
                {
                    if (value < _translationYMin)
                    {
                        _translationY = _translationYMin;
                    }
                    else if (value > _translationYMax)
                    {
                        _translationY = _translationYMax;
                    }
                    else
                    {
                        _translationY = value;
                    }

                    RaisePropertyChanged();
                }
            }
        }

        public bool XAxisLocked
        {
            get { return _xAxisLocked; }
            set
            {
                _xAxisLocked = value;
                RaisePropertyChanged();
            }
        }

        public bool YAxisLocked
        {
            get { return _yAxisLocked; }
            set
            {
                _yAxisLocked = value;
                RaisePropertyChanged();
            }
        }

        public bool AspectRatioLocked
        {
            get { return _aspectRatioLocked; }
            set
            {
                _aspectRatioLocked = value;
                RaisePropertyChanged();
            }
        }

        public bool XScalingLocked
        {
            get { return _xScalingLocked; }
            set
            {
                _xScalingLocked = value;
                RaisePropertyChanged();
            }
        }

        public bool YScalingLocked
        {
            get { return _yScalingLocked; }
            set
            {
                _yScalingLocked = value;
                RaisePropertyChanged();
            }
        }

        public bool SelectRegionPossible
        {
            get => _selectRegionPossible;
            set
            {
                _selectRegionPossible = value;
                RaisePropertyChanged();
            }
        }

        public bool SelectedRegionLimitedVertically
        {
            get => _selectedRegionLimitedVertically;
            set
            {
                _selectedRegionLimitedVertically = value;
                RaisePropertyChanged();
            }
        }

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                _backgroundBrush = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PolygonViewModel> PolygonViewModels { get; }

        public ObservableCollection<PolylineViewModel> PolylineViewModels { get; }

        public ObservableCollection<PointViewModel> PointViewModels { get; }

        public ObservableCollection<ShapeViewModel> ShapeViewModels { get; }

        public ObservableCollection<LineViewModel> LineViewModels { get; }

        public ObservableCollection<LabelViewModel> LabelViewModels { get; }

        public IEnumerable<int> AllShapeIds
        {
            get { return _shapeViewModelMap.Keys; }
        }

        public Point WorldWindowFocus
        {
            get => new(
                WorldWindowUpperLeft.X + WorldWindowSize.Width / 2,
                WorldWindowUpperLeft.Y + WorldWindowSize.Height / 2);
            set =>
                WorldWindowUpperLeft = new Point(
                    value.X - WorldWindowSize.Width / 2, 
                    value.Y - WorldWindowSize.Height / 2);
        }

        public event EventHandler<WorldWindowUpdatedEventArgs> WorldWindowUpdateOccured;
        public event EventHandler<WorldWindowUpdatedEventArgs> WorldWindowMajorUpdateOccured;
        public event EventHandler<MouseEventArgs> MouseClickOccured;

        // A callback delegate passed to the view model will be kept and
        // invoked each time a frame refresh is needed
        public UpdateModelCallBack UpdateModelCallBack { get; set; }

        public GeometryEditorViewModel(
            int yAxisFactor = 1)
        {
            _yAxisFactor = yAxisFactor;
            _initialScalingX = 1;
            _initialScalingY = 1;

            _backgroundBrush = new SolidColorBrush(Colors.WhiteSmoke);
            _defaultBrush = new SolidColorBrush(Colors.Black);
            _shapeViewModelMap = new Dictionary<int, ShapeViewModel>();

            WorldWindowUpperLeftLimit = new Point(double.MinValue, double.MinValue);
            WorldWindowBottomRightLimit = new Point(double.MaxValue, double.MaxValue);

            PolygonViewModels = new ObservableCollection<PolygonViewModel>();
            PolylineViewModels = new ObservableCollection<PolylineViewModel>();
            PointViewModels = new ObservableCollection<PointViewModel>();
            ShapeViewModels = new ObservableCollection<ShapeViewModel>();
            LineViewModels = new ObservableCollection<LineViewModel>();
            LabelViewModels = new ObservableCollection<LabelViewModel>();

            MousePositionWorld = new ObservableObject<Point?> { Object = null };
            SelectedRegion = new ObservableObject<BoundingBox?> { Object = null };

            SelectedRegion.PropertyChanged += (s, e) =>
            {
                var bb = SelectedRegion.Object;

                if (bb == null)
                {
                    SelectedRegionWindowVisible = false;
                }
                else
                {
                    SelectedRegionWindow.Point = new PointD(
                        bb.Left + bb.Width / 2,
                        bb.Top + bb.Height / 2);

                    SelectedRegionWindow.Width = bb.Width;
                    SelectedRegionWindow.Height = bb.Height;
                }
            };

            PropertyChanged += GeometryEditorViewModel_PropertyChanged;

            // I første omngang placeres den i vinduet og gøres lidt mindre end det (senere skal det afhænge af brugerinteraktion)
            SelectedRegionWindow = new RectangleViewModel();
            SelectedRegionWindowVisible = false;
        }

        private void GeometryEditorViewModel_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ViewPortSize":
                    if (WorldWindowSize.Width < double.Epsilon &&
                        WorldWindowSize.Height < double.Epsilon)
                    {
                        // Her initialiserer vi World Window og skalering for første gang
                        InitializeScalingAndWorldWindow();
                    }
                        
                    //MarginBottomOffset = ViewPortSize.Height - MarginBottom;

                    break;
            }
        }

        public void InitializeWorldWindow(
            Point focus)
        {
            _initialScalingX = 1.0;
            _initialScalingY = 1.0;
            _initialWorldWindowFocus = focus;
            _initialWorldWindowSize = null;
            _fitAspectRatio = false;

            InitializeScalingAndWorldWindow();
        }

        public void InitializeWorldWindow(
            Size scaling)
        {
            _initialScalingX = scaling.Width;
            _initialScalingY = scaling.Height;
            _initialWorldWindowFocus = null;
            _initialWorldWindowSize = null;
            _fitAspectRatio = false;

            InitializeScalingAndWorldWindow();
        }

        public void InitializeWorldWindow(
            Size scaling,
            Point focus)
        {
            _initialScalingX = scaling.Width;
            _initialScalingY = scaling.Height;
            _initialWorldWindowFocus = focus;
            _initialWorldWindowSize = null;
            _fitAspectRatio = false;

            InitializeScalingAndWorldWindow();
        }

        public void InitializeWorldWindow(
            Point focus,
            Size size,
            bool fitAspectRatio)
        {
            _initialScalingX = null;
            _initialScalingY = null;
            _initialWorldWindowFocus = focus;
            _initialWorldWindowSize = size;
            _fitAspectRatio = fitAspectRatio;

            InitializeScalingAndWorldWindow();
        }

        // Kaldes:
        // - Når ViewPorten initialiseres, dvs første gang den sættes og altså ikke når den ændres
        // - Hvis hosten kalder en af de 4 InitializeWorldWindow metoder til placering af World Window.
        // Bemærk, at det f.eks giver fin mening at kalde denne method umiddelbart efter at have kaldt
        // GeometryEditorViewModel-klassens constructor, også selv om Viewporten på dette tidspunkt
        // ikke er initialiseret endnu. I den situation er sigtet blot at initialisere de variable,
        // der vil skulle bruges, når metoden kaldes igen i forbindelse med at viewporten initialiseres.

        private void InitializeScalingAndWorldWindow()
        {
            if (ViewPortSize.Width < double.Epsilon)
            {
                // Det giver kun mening, hvis viewporten er initialiseret
                return;
            }

            if (_yAxisFactor == 1)
            {
                // Dette er passende for et ALMINDELIGT view

                if (_initialScalingX.HasValue &&
                    _initialScalingY.HasValue)
                {
                    Scaling = new Size(_initialScalingX.Value, _initialScalingY.Value);

                    if (_initialWorldWindowFocus.HasValue)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        WorldWindowUpperLeft = new Point(0, 0);
                    }
                }
                else if (_initialWorldWindowFocus.HasValue &&
                         _initialWorldWindowSize.HasValue)
                {
                    if (_fitAspectRatio)
                    {
                        // Her skal World Window fylde hele viewporten ud
                        throw new NotImplementedException();

                        //Scaling = new Size(
                        //    ViewPortSize.Width / (_initialWorldWindowSize.Value.Width),
                        //    ViewPortSize.Height / _initialWorldWindowSize.Value.Height);

                        //WorldWindowUpperLeft = new Point(
                        //    _initialWorldWindowFocus.Value.X - _initialWorldWindowSize.Value.Width / 2,
                        //    _initialWorldWindowSize.Value.Height / 2 - _initialWorldWindowFocus.Value.Y - ViewPortSize.Height / Scaling.Height);
                    }
                    else
                    {
                        if (ViewPortSize.Width / ViewPortSize.Height <=
                            _initialWorldWindowSize.Value.Width / _initialWorldWindowSize.Value.Height)
                        {
                            // Dette er hvis World Window skal afgrænses af Viewportens VENSTRE og HØJRE side

                            var scaling = ViewPortSize.Width / _initialWorldWindowSize.Value.Width;
                            Scaling = new Size(scaling, scaling);

                            WorldWindowUpperLeft = new Point(
                                _initialWorldWindowFocus.Value.X - _initialWorldWindowSize.Value.Width / 2,
                                _initialWorldWindowFocus.Value.Y - WorldWindowSize.Height / 2);
                        }
                        else
                        {
                            // Dette er hvis World Window skal afgrænses af Viewportens NEDERSTE OG ØVERSTE side

                            var scaling = ViewPortSize.Height / _initialWorldWindowSize.Value.Height;
                            Scaling = new Size(scaling, scaling);

                            WorldWindowUpperLeft = new Point(
                                _initialWorldWindowFocus.Value.X - WorldWindowSize.Width / 2,
                                _initialWorldWindowFocus.Value.Y - _initialWorldWindowSize.Value.Height / 2);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid specification of World Window");
                }
            }
            else
            {
                // Dette er passende for et MATEMATISK view

                if (_initialScalingX.HasValue &&
                    _initialScalingY.HasValue)
                {
                    Scaling = new Size(_initialScalingX.Value, _initialScalingY.Value);

                    if (_initialWorldWindowFocus.HasValue)
                    {
                        WorldWindowUpperLeft = new Point(
                            _initialWorldWindowFocus.Value.X - WorldWindowSize.Width / 2,
                            -ViewPortSize.Height / Scaling.Height - _initialWorldWindowFocus.Value.Y + WorldWindowSize.Height / 2);
                    }
                    else
                    {
                        WorldWindowUpperLeft = new Point(0, -ViewPortSize.Height / Scaling.Height);
                    }
                }
                else if (_initialWorldWindowFocus.HasValue &&
                         _initialWorldWindowSize.HasValue)
                {
                    if (_fitAspectRatio)
                    {
                        // Her skal World Window fylde hele viewporten ud

                        Scaling = new Size(
                            ViewPortSize.Width / (_initialWorldWindowSize.Value.Width),
                            ViewPortSize.Height / _initialWorldWindowSize.Value.Height);
                        
                        // Her kan man f.eks. sætte Scaling.Height til 1, hvis man gerne vil have det sådan

                        WorldWindowUpperLeft = new Point(
                            _initialWorldWindowFocus.Value.X - _initialWorldWindowSize.Value.Width / 2,
                            _initialWorldWindowSize.Value.Height / 2 - _initialWorldWindowFocus.Value.Y - ViewPortSize.Height / Scaling.Height);
                    }
                    else
                    {
                        if (ViewPortSize.Width / ViewPortSize.Height <=
                            _initialWorldWindowSize.Value.Width / _initialWorldWindowSize.Value.Height)
                        {
                            // Dette er hvis World Window skal afgrænses af Viewportens VENSTRE og HØJRE side

                            var scaling = ViewPortSize.Width / _initialWorldWindowSize.Value.Width;
                            Scaling = new Size(scaling, scaling);

                            WorldWindowUpperLeft = new Point(
                                _initialWorldWindowFocus.Value.X - _initialWorldWindowSize.Value.Width / 2,
                                -_initialWorldWindowFocus.Value.Y - WorldWindowSize.Height / 2);
                        }
                        else
                        {
                            // Dette er hvis World Window skal afgrænses af Viewportens NEDERSTE OG ØVERSTE side

                            var scaling = ViewPortSize.Height / _initialWorldWindowSize.Value.Height;
                            Scaling = new Size(scaling, scaling);

                            WorldWindowUpperLeft = new Point(
                                _initialWorldWindowFocus.Value.X - WorldWindowSize.Width / 2,
                                _initialWorldWindowSize.Value.Height / 2 - _initialWorldWindowFocus.Value.Y - ViewPortSize.Height / scaling);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid specification of World Window");
                }
            }

            UpdatePoints();
            OnWorldWindowMajorUpdateOccured();
        }

        public void UpdatePoints()
        {
            TranslationX = 0;
            TranslationY = 0;

            _translationXMax = (WorldWindowUpperLeft.X - WorldWindowUpperLeftLimit.X) * Scaling.Width;
            _translationXMin = (WorldWindowUpperLeft.X + WorldWindowSize.Width - WorldWindowBottomRightLimit.X) * Scaling.Width;
            _translationXMax = Math.Max(0, _translationXMax);
            _translationXMin = Math.Min(0, _translationXMin);

            _translationYMax = (WorldWindowUpperLeft.Y - WorldWindowUpperLeftLimit.Y) * Scaling.Height;
            _translationYMin = (WorldWindowUpperLeft.Y + WorldWindowSize.Height - WorldWindowBottomRightLimit.Y) * Scaling.Height;
            _translationYMax = Math.Max(0, _translationYMax);
            _translationYMin = Math.Min(0, _translationYMin);

            foreach (var polylineViewModel in PolylineViewModels)
            {
                polylineViewModel.UpdateViewportCoordinates(Scaling, WorldWindowUpperLeft);
            }

            foreach (var lineViewModel in LineViewModels)
            {
                lineViewModel.UpdateViewportCoordinates(Scaling, WorldWindowUpperLeft);
            }
        }

        // For zooming in and out
        public void ChangeScaling(
            double factor)
        {
            FocusInCenterOfViewPort();
            Scaling = new Size(
                Scaling.Width * factor,
                Scaling.Height * factor);
        }

        public void AddPoint(
            PointD point,
            double diameter,
            Brush brush = null)
        {
            PointViewModels.Add(new PointViewModel(new PointD(point.X, _yAxisFactor * point.Y), diameter, brush ?? _defaultBrush));
        }

        public void AddShape(
            int id,
            ShapeViewModel shapeViewModel)
        {
            shapeViewModel.Point = new PointD(
                shapeViewModel.Point.X, 
                shapeViewModel.Point.Y * _yAxisFactor);

            _shapeViewModelMap[id] = shapeViewModel;
            ShapeViewModels.Add(shapeViewModel);
        }

        public void RemoveShape(
            int id)
        {
            var shapeViewModel = _shapeViewModelMap[id];
            ShapeViewModels.Remove(shapeViewModel);
            _shapeViewModelMap.Remove(id);
        }

        public void AddLine(
            PointD point1,
            PointD point2,
            double thickness,
            Brush brush)
        {
            LineViewModels.Add(new LineViewModel(
                new PointD(point1.X, _yAxisFactor * point1.Y), 
                new PointD(point2.X, _yAxisFactor * point2.Y), 
                thickness, brush));
        }

        public void AddPolygon(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            PolygonViewModels.Add(new PolygonViewModel(
                points.Select(p => new PointD(p.X, _yAxisFactor * p.Y)), thickness, brush));
        }

        public void AddPolyline(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            var polyLineViewModel = new PolylineViewModel(
                points.Select(p => new PointD(p.X, _yAxisFactor * p.Y)), thickness, brush);

            polyLineViewModel.UpdateViewportCoordinates(Scaling, WorldWindowUpperLeft);
            PolylineViewModels.Add(polyLineViewModel);
        }

        // Argumenter
        // point: angiver positionen af labelens centrum
        // width og height: angiver, hvor stor den skal være
        // shift: en forskydning af centeret, så det ikke nødvendigvis er labellens centrum men f.eks. dens øverste venstre hjørne, der positioneres
        public void AddLabel(
            string text,
            PointD point,
            double width,
            double height,
            PointD shift,
            double opacity,
            double? fixedViewPortXCoordinate = null,
            double? fixedViewPortYCoordinate = null)
        {
            LabelViewModels.Add(new LabelViewModel
            {
                Text = text,
                Point = new PointD(point.X, _yAxisFactor * point.Y), 
                Width = width, 
                Height = height,
                Shift = shift,
                Opacity = opacity,
                FixedViewPortXCoordinate = fixedViewPortXCoordinate,
                FixedViewPortYCoordinate = fixedViewPortYCoordinate
            });
        }

        public void ClearPolygons()
        {
            PolygonViewModels.Clear();
        }

        public void ClearPolylines()
        {
            PolylineViewModels.Clear();
        }

        public void ClearPoints()
        {
            PointViewModels.Clear();
        }

        public void ClearShapes()
        {
            ShapeViewModels.Clear();
            _shapeViewModelMap.Clear();
        }

        public void ClearLines()
        {
            LineViewModels.Clear();
        }

        public void ClearLabels()
        {
            LabelViewModels.Clear();
        }

        public void FocusInCenterOfViewPort()
        {
            MousePositionViewport = new Point(
                ViewPortSize.Width / 2,
                ViewPortSize.Height / 2);
        }

        public void SetFocusForWorldWindow(
            PointD focus)
        {
            WorldWindowUpperLeft = new Point(
                focus.X - WorldWindowSize.Width / 2,
                focus.Y - WorldWindowSize.Height / 2);
        }

        public void AdjustWorldWindowSoPointLiesInCentralSquare(
            PointD point,
            double maxOffsetXFraction,
            double maxOffsetYFraction,
            double correctionXFraction = 0.5,
            double correctionYFraction = 0.5)
        {
            var correctionX = WorldWindowSize.Width * (correctionXFraction - 0.5);
            var correctionY = WorldWindowSize.Height * (correctionYFraction - 0.5);

            // What is the distance between the current focus and the point
            var currentFocus = new PointD(
                _worldWindowUpperLeft.X + _worldWindowSize.Width / 2 + correctionX,
                _worldWindowUpperLeft.Y + _worldWindowSize.Height / 2 + correctionY);

            var dx = point.X - currentFocus.X;
            var dy = point.Y - currentFocus.Y;

            var shiftX = 0.0;
            var shiftY = 0.0;
            var maxDx = WorldWindowSize.Width * maxOffsetXFraction;
            var maxDy = WorldWindowSize.Height * maxOffsetYFraction;

            if (dx > maxDx)
            {
                shiftX = dx - maxDx;
            }
            else if (dx < -maxDx)
            {
                shiftX = maxDx + dx;
            }

            if (dy > maxDy)
            {
                shiftY = dy - maxDy;
            }
            else if (dy < -maxDy)
            {
                shiftY = maxDy + dy;
            }

            if (Math.Abs(shiftX) > 0.001 ||
                Math.Abs(shiftY) > 0.001)
            {
                WorldWindowUpperLeft = new Point(
                    WorldWindowUpperLeft.X + shiftX,
                    WorldWindowUpperLeft.Y + shiftY);
            }
        }

        public bool ContainsShape(
            int id)
        {
            return _shapeViewModelMap.ContainsKey(id);
        }

        public ShapeViewModel TryGetShape(
            int id)
        {
            _shapeViewModelMap.TryGetValue(id, out var result);
            return result;
        }

        // This method is called from the View class when it handles the CompositionTarget.Rendering event
        public void UpdateModel()
        {
            UpdateModelCallBack?.Invoke();
        }

        protected void UpdateTransformationMatrix()
        {
            TransformationMatrix = new Matrix(
                _scaling.Width,
                0,
                0,
                _scaling.Height,
                -WorldWindowUpperLeft.X * _scaling.Width,
                -WorldWindowUpperLeft.Y * _scaling.Height);
        }

        protected void UpdateWorldWindowSize()
        {
            if (_scaling.Width <= 0 ||
                _scaling.Height <= 0) return;

            WorldWindowSize = new Size(
                _viewPortSize.Width / _scaling.Width,
                _viewPortSize.Height / _scaling.Height);

            UpdateWorldWindowPositionOverrides();

            WorldWindowUpperLeft = WorldWindowUpperLeft; // For at trigge, at den notificerer viewet
        }

        // This method is called from the View class
        public void OnWorldWindowUpdateOccured()
        {
            var handler = WorldWindowUpdateOccured;

            if (handler != null)
            {
                handler(this, new WorldWindowUpdatedEventArgs(
                    WorldWindowUpperLeft,
                    WorldWindowSize));
            }
        }

        // This method is called from the View class
        public void OnWorldWindowMajorUpdateOccured()
        {
            UpdatePoints();

            var handler = WorldWindowMajorUpdateOccured;

            if (handler != null)
            {
                handler(this, new WorldWindowUpdatedEventArgs(
                    WorldWindowUpperLeft,
                    WorldWindowSize));
            }
        }

        // This method is called from the View class
        public void OnMouseClickOccured(
            Point mousePositionViewport)
        {
            var handler = MouseClickOccured;

            if (handler != null)
            {
                var mousePositionWorld = new Point(
                    ConvertViewPortXCoordinateToWorldXCoordinate(mousePositionViewport.X),
                    ConvertViewPortYCoordinateToWorldYCoordinate(mousePositionViewport.Y));

                handler(this, new MouseEventArgs(mousePositionWorld));
            }
        }

        public double ConvertViewPortXCoordinateToWorldXCoordinate(
            double viewPortXCoordinate)
        {
            return _worldWindowUpperLeft.X + viewPortXCoordinate / _scaling.Width;
        }

        public double ConvertViewPortYCoordinateToWorldYCoordinate(
            double viewPortYCoordinate)
        {
            return _worldWindowUpperLeft.Y + viewPortYCoordinate / _scaling.Height;
        }

        public double ConvertWorldXCoordinateToViewPortXCoordinate(
            double worldXCoordinate)
        {
            return (worldXCoordinate - _worldWindowUpperLeft.X) * _scaling.Width;
        }

        public double ConvertWorldYCoordinateToViewPortYCoordinate(
            double worldYCoordinate)
        {

            return (worldYCoordinate - _worldWindowUpperLeft.Y) * _scaling.Height;
        }

        private void EnsureWorldWindowIsWithinLimits()
        {
            // Push the World Window to the left if it exceeds the limit to the right
            if (_worldWindowUpperLeft.X + WorldWindowSize.Width > WorldWindowBottomRightLimit.X)
            {
                _worldWindowUpperLeft.X = WorldWindowBottomRightLimit.X - WorldWindowSize.Width;
            }

            // Push the World Window up if it exceeds the bottom limit
            if (_worldWindowUpperLeft.Y + WorldWindowSize.Height > WorldWindowBottomRightLimit.Y)
            {
                _worldWindowUpperLeft.Y = WorldWindowBottomRightLimit.Y - WorldWindowSize.Height;
            }

            // Push the World Window to the right if it exceeds the limit to the left
            _worldWindowUpperLeft.X = Math.Max(_worldWindowUpperLeft.X, WorldWindowUpperLeftLimit.X);

            // Push the World Window down if it exceeds the bottom limit
            _worldWindowUpperLeft.Y = Math.Max(_worldWindowUpperLeft.Y, WorldWindowUpperLeftLimit.Y);
        }

        private void UpdateWorldWindowPositionOverrides()
        {
            if (WorldWindowSize.Width > WorldWindowBottomRightLimit.X - WorldWindowUpperLeftLimit.X)
            {
                switch (_roiAlignment)
                {
                    case ROIAlignment.Center:
                        {
                            var dx = (WorldWindowSize.Width - (WorldWindowBottomRightLimit.X - WorldWindowUpperLeftLimit.X)) / 2;
                            _worldWindowUpperLeftOverride_X = WorldWindowUpperLeftLimit.X - dx;
                            break;
                        }
                    case ROIAlignment.TopLeft:
                        {
                            _worldWindowUpperLeftOverride_X = WorldWindowUpperLeftLimit.X;
                            break;
                        }
                }
            }
            else
            {
                _worldWindowUpperLeftOverride_X = null;
            }

            if (WorldWindowSize.Height > WorldWindowBottomRightLimit.Y - WorldWindowUpperLeftLimit.Y)
            {
                switch (_roiAlignment)
                {
                    case ROIAlignment.Center:
                        {
                            var dy = (WorldWindowSize.Height - (WorldWindowBottomRightLimit.Y - WorldWindowUpperLeftLimit.Y)) / 2;
                            _worldWindowUpperLeftOverride_Y = WorldWindowUpperLeftLimit.Y - dy;
                            break;
                        }
                    case ROIAlignment.TopLeft:
                        {
                            _worldWindowUpperLeftOverride_Y = WorldWindowUpperLeftLimit.Y;
                            break;
                        }
                }
            }
            else
            {
                _worldWindowUpperLeftOverride_Y = null;
            }
        }
    }
}