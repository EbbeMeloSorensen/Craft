using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Craft.Utils.Linq;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;

namespace Craft.UIElements.Geometry2D.Reborn
{
    public class GeometryCanvas : FrameworkElement
    {
        private const double _zoomingFactor = 1.2;
        private WorldWindowLimiter _worldWindowLimiter;

        private bool _isRenderingSubscribed;
        private bool _isPanning;
        private bool _isDrawing;
        private Point _mouseDownPosition;
        private Point _panStartWorldOrigin;
        private TimeSpan _lastTime;
        private BoundingBox _current;
        private BoundingBox _target;
        private BoundingBox? _potentialSelectionWindow;
        private List<Point> _drawingStrokePoints;
        private Point? _nextPotentialDrawingStrokePoint;

        private Brush _selectionWindowBrush = new SolidColorBrush(Color.FromArgb(38, 0, 120, 215));
        private Pen _selectionWindowPen = new Pen(new SolidColorBrush(Color.FromRgb(0, 120, 215)), 1);

        // =============================
        // Items (your geometries)
        // =============================

        public IEnumerable<object> SelectedGeometricObjects
        {
            get => (IEnumerable<object>)GetValue(SelectedGeometricObjectsProperty);
            set => SetValue(SelectedGeometricObjectsProperty, value);
        }

        public static readonly DependencyProperty SelectedGeometricObjectsProperty =
            DependencyProperty.Register(
                nameof(SelectedGeometricObjects),
                typeof(IEnumerable<object>),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null, OnSelectedGeometricObjectsChanged));

        public IEnumerable<GeometryLayer> GeometryLayers
        {
            get => (IEnumerable<GeometryLayer>)GetValue(GeometryLayersProperty);
            set => SetValue(GeometryLayersProperty, value);
        }

        public static readonly DependencyProperty GeometryLayersProperty =
            DependencyProperty.Register(
                nameof(GeometryLayers),
                typeof(IEnumerable<GeometryLayer>),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null, OnGeometryLayersChanged));

        // Dette er elementets TILSTAND, som indeholder al nødvendig information for at kunne beregne world vindue og transformere world til viewport koordinater.
        public ViewState ViewState
        {
            get => (ViewState)GetValue(ViewStateProperty);
            set => SetValue(ViewStateProperty, value);
        }

        public static readonly DependencyProperty ViewStateProperty =
            DependencyProperty.Register(
                nameof(ViewState),
                typeof(ViewState),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    new ViewState(new Point(0, 0), new Size(1, 1)), FrameworkPropertyMetadataOptions.AffectsRender));

        public Point? CursorWorldPosition
        {
            get => (Point)GetValue(CursorWorldPositionProperty);
            set => SetValue(CursorWorldPositionProperty, value);
        }

        public static readonly DependencyProperty CursorWorldPositionProperty =
            DependencyProperty.Register(
                nameof(CursorWorldPosition),
                typeof(Point?),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null));

        public BoundingBox SelectionWindow
        {
            get => (BoundingBox)GetValue(SelectionWindowProperty);
            set => SetValue(SelectionWindowProperty, value);
        }

        public static readonly DependencyProperty SelectionWindowProperty =
            DependencyProperty.Register(
                nameof(SelectionWindow),
                typeof(BoundingBox),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null));

        public Point? ClickedWorldPosition
        {
            get => (Point?)GetValue(ClickedWorldPositionProperty);
            set => SetValue(ClickedWorldPositionProperty, value);
        }

        public static readonly DependencyProperty ClickedWorldPositionProperty =
            DependencyProperty.Register(
                nameof(ClickedWorldPosition),
                typeof(Point?),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null));

        public bool LockAspectRatio
        {
            get => (bool)GetValue(LockAspectRatioProperty);
            set => SetValue(LockAspectRatioProperty, value);
        }

        public static readonly DependencyProperty LockAspectRatioProperty =
            DependencyProperty.Register(
                nameof(LockAspectRatio),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false));

        public bool LockXAxis
        {
            get => (bool)GetValue(LockXAxisProperty);
            set => SetValue(LockXAxisProperty, value);
        }

        public static readonly DependencyProperty LockXAxisProperty =
            DependencyProperty.Register(
                nameof(LockXAxis),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false));

        public bool LockYAxis
        {
            get => (bool)GetValue(LockYAxisProperty);
            set => SetValue(LockYAxisProperty, value);
        }

        public static readonly DependencyProperty LockYAxisProperty =
            DependencyProperty.Register(
                nameof(LockYAxis),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false));

        public bool DampFocusShifts
        {
            get => (bool)GetValue(DampFocusShiftsProperty);
            set => SetValue(DampFocusShiftsProperty, value);
        }

        public static readonly DependencyProperty DampFocusShiftsProperty =
            DependencyProperty.Register(
                nameof(DampFocusShifts),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false));

        public double FocusShiftDamping
        {
            get => (double)GetValue(FocusShiftDampingProperty);
            set => SetValue(FocusShiftDampingProperty, value);
        }

        public static readonly DependencyProperty FocusShiftDampingProperty =
            DependencyProperty.Register(
                nameof(FocusShiftDamping),
                typeof(double),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(5.0));

        // Dette er WorldWindow, som er afledt af ViewState og som bruges til at kommunikere world vinduets position og størrelse til omverdenen
        // Elementet her modtager IKKE data gennem denne property
        public BoundingBox WorldWindow
        {
            get => (BoundingBox)GetValue(WorldWindowProperty);
            set => SetValue(WorldWindowProperty, value);
        }

        public static readonly DependencyProperty WorldWindowProperty =
            DependencyProperty.Register(
                nameof(WorldWindow),
                typeof(BoundingBox),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    default(BoundingBox),
                    OnWorldWindowChanged));

        public BoundingBox WorldWindowExpanded
        {
            get => (BoundingBox)GetValue(WorldWindowExpandedProperty);
            set => SetValue(WorldWindowExpandedProperty, value);
        }

        public static readonly DependencyProperty WorldWindowExpandedProperty =
            DependencyProperty.Register(
                nameof(WorldWindowExpanded),
                typeof(BoundingBox),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    default(BoundingBox)));

        // Denne bruges til at kommunikere udefra kommende requests om at ændre world vinduet
        // Dvs elementet her MODTAGER DATA udefra gennem denne property
        public BoundingBox RequestedWorldWindow
        {
            get => (BoundingBox)GetValue(RequestedWorldWindowProperty);
            set => SetValue(RequestedWorldWindowProperty, value);
        }

        public static readonly DependencyProperty RequestedWorldWindowProperty =
            DependencyProperty.Register(
                nameof(RequestedWorldWindow),
                typeof(BoundingBox),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    default(BoundingBox),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnRequestedWorldWindowChanged));

        public WorldFocusRequest RequestedWorldFocus
        {
            get => (WorldFocusRequest)GetValue(RequestedWorldFocusProperty);
            set => SetValue(RequestedWorldFocusProperty, value);
        }

        public static readonly DependencyProperty RequestedWorldFocusProperty =
            DependencyProperty.Register(
                nameof(RequestedWorldFocus),
                typeof(WorldFocusRequest),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    default(WorldFocusRequest),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnRequestedWorldFocusChanged));

        // Denne bruges til at kommunikere udefra kommende requests om at ændre grænser for world vinduet
        // Dvs elementet her MODTAGER DATA udefra gennem denne property
        public BoundingBox WorldWindowBounds
        {
            get => (BoundingBox)GetValue(WorldWindowBoundsProperty);
            set => SetValue(WorldWindowBoundsProperty, value);
        }

        public static readonly DependencyProperty WorldWindowBoundsProperty =
            DependencyProperty.Register(
                nameof(WorldWindowBounds),
                typeof(BoundingBox),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    default(BoundingBox),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnWorldWindowBoundsChanged));

        public bool DebugMode
        {
            get => (bool)GetValue(DebugModeProperty);
            set => SetValue(DebugModeProperty, value);
        }

        public static readonly DependencyProperty DebugModeProperty =
            DependencyProperty.Register(
                nameof(DebugMode),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool SnapToGrid
        {
            get => (bool)GetValue(SnapToGridProperty);
            set => SetValue(SnapToGridProperty, value);
        }

        public static readonly DependencyProperty SnapToGridProperty =
            DependencyProperty.Register(
                nameof(SnapToGrid),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false));

        public double GridSpacing
        {
            get => (double)GetValue(GridSpacingProperty);
            set => SetValue(GridSpacingProperty, value);
        }

        public static readonly DependencyProperty GridSpacingProperty =
            DependencyProperty.Register(
                nameof(GridSpacing),
                typeof(double),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(50.0));

        public bool ShowGrid
        {
            get => (bool)GetValue(ShowGridProperty);
            set => SetValue(ShowGridProperty, value);
        }

        public static readonly DependencyProperty ShowGridProperty =
            DependencyProperty.Register(
                nameof(ShowGrid),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowCoordinateSystem
        {
            get => (bool)GetValue(ShowCoordinateSystemProperty);
            set => SetValue(ShowCoordinateSystemProperty, value);
        }

        public static readonly DependencyProperty ShowCoordinateSystemProperty =
            DependencyProperty.Register(
                nameof(ShowCoordinateSystem),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool TimeAxisMode
        {
            get => (bool)GetValue(TimeAxisModeProperty);
            set => SetValue(TimeAxisModeProperty, value);
        }

        public static readonly DependencyProperty TimeAxisModeProperty =
            DependencyProperty.Register(
                nameof(TimeAxisMode),
                typeof(bool),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public CanvasMode CanvasMode
        {
            get => (CanvasMode)GetValue(CanvasModeProperty);
            set => SetValue(CanvasModeProperty, value);
        }

        public static readonly DependencyProperty CanvasModeProperty =
            DependencyProperty.Register(
                nameof(CanvasMode),
                typeof(CanvasMode),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    default(CanvasMode),
                    OnCanvasModeChanged));

        public List<Point> DrawingStrokePoints
        {
            get => (List<Point>)GetValue(DrawingStrokePointsProperty);
            set => SetValue(DrawingStrokePointsProperty, value);
        }

        public static readonly DependencyProperty DrawingStrokePointsProperty =
            DependencyProperty.Register(
                nameof(DrawingStrokePoints),
                typeof(List<Point>),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null));

        public event EventHandler<FrameEventArgs> FrameRendering;

        public GeometryCanvas()
        {
            _drawingStrokePoints = new List<Point>();

            Loaded += GeometryCanvas_Loaded;
            Unloaded += GeometryCanvas_Unloaded;

            CompositionTarget.Rendering += OnRendering;
            Unloaded += (s, e) => CompositionTarget.Rendering -= OnRendering;
        }

        private void GeometryCanvas_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            if (_isRenderingSubscribed)
            {
                return;
            }

            CompositionTarget.Rendering += OnRendering;

            _isRenderingSubscribed = true;
        }

        private void GeometryCanvas_Unloaded(
            object sender,
            RoutedEventArgs e)
        {
            if (!_isRenderingSubscribed)
            {
                return;
            }

            CompositionTarget.Rendering -= OnRendering;

            _isRenderingSubscribed = false;
        }

        // =============================
        // Handle collection changes
        // =============================

        private static void OnGeometryLayersChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= canvas.OnCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += canvas.OnCollectionChanged;

            canvas.InvalidateVisual();
        }

        private static void OnSelectedGeometricObjectsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= canvas.OnCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += canvas.OnCollectionChanged;

            canvas.InvalidateVisual();
        }

        private void OnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }

        // =============================
        // Rendering
        // =============================
        protected override void OnRender(
            DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (GeometryLayers == null)
            {
                return;
            }

            var worldWindow = ComputeWorldWindow();
            var worldToViewportTransform = CreateWorldToViewportTransform(worldWindow, RenderSize);

            if (DebugMode)
            {
                dc.PushTransform(new MatrixTransform(worldToViewportTransform));

                var debugTransform = CreateDebugShrinkTransform(worldWindow, 0.4);
                dc.PushTransform(debugTransform);

                var worldPen = new Pen(Brushes.LimeGreen, 2);
                var expandedPen = new Pen(Brushes.OrangeRed, 2);
                var boundsPen = new Pen(Brushes.DarkMagenta, 2);

                expandedPen.Freeze();

                var worldRect = ToRect(worldWindow);
                dc.DrawRectangle(null, worldPen, worldRect);

                var expandedRect = ToRect(WorldWindowExpanded);
                dc.DrawRectangle(null, expandedPen, expandedRect);

                var boundsRect = ToRect(WorldWindowBounds);
                dc.DrawRectangle(null, boundsPen, boundsRect);

                DrawGeometriesDebug(dc);

                dc.Pop();
                dc.Pop();
            }
            else
            {
                if (TimeAxisMode)
                {
                    var startTicks = (long)WorldWindow.MinX;
                    var endTicks = (long)WorldWindow.MaxX;

                    var ticks = TimeTickEngine.Generate(
                        startTicks,
                        endTicks,
                        ActualWidth,
                        120);

                    var minorGridLinePen = new Pen(Brushes.LightGray, 1);
                    var majorGridLinePen = new Pen(Brushes.LightGray, 2);

                    var typeface = new Typeface("Segoe UI");
                    double fontSize = 10;
                    double margin = 4;
                    var yScreen = ActualHeight - margin;

                    foreach (var tick in ticks)
                    {
                        if (ShowGrid)
                        {
                            var gridLinePen = tick.Kind == TickKind.Major
                                ? majorGridLinePen
                                : minorGridLinePen;

                            dc.DrawLine(
                                gridLinePen,
                                new Point(tick.X, 0),
                                new Point(tick.X, ActualHeight));
                        }

                        if (ShowCoordinateSystem)
                        {
                            for (var i = 0; i < tick.LabelLines.Count; i++)
                            {
                                var text = new FormattedText(
                                    tick.LabelLines[i],
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    FlowDirection.LeftToRight,
                                    typeface,
                                    fontSize,
                                    Brushes.Black,
                                    1.0);

                                var x = tick.X - text.Width / 2;
                                var y = yScreen - text.Height * (tick.LabelLines.Count - i);

                                if (tick.Kind != TickKind.Minor)
                                {
                                    if (x < 0)
                                    {
                                        x = 0;
                                    }
                                    else if (x + text.Width > ActualWidth)
                                    {
                                        x = ActualWidth - text.Width;
                                    }
                                }

                                // Center text under grid line
                                dc.DrawText(
                                    text,
                                    new Point(x, y));
                            }
                        }
                    }

                    if (ShowGrid)
                    {
                        DrawHorizontalGridLines(dc);
                    }

                    if (ShowCoordinateSystem)
                    {
                        DrawHorizontalGridLabels(dc);
                    }
                }
                else
                {
                    if (ShowGrid)
                    {
                        DrawHorizontalGridLines(dc);
                        DrawVerticalGridLines(dc);
                    }

                    if (ShowCoordinateSystem)
                    {
                        DrawAxes(dc, true, true);
                        DrawAxisTicks(dc, true, true);
                        DrawHorizontalGridLabels(dc);
                        DrawVerticalGridLabels(dc);
                    }
                }

                DrawGeometries(dc, worldWindow, worldToViewportTransform);
            }
        }

        private void DrawGeometries(
            DrawingContext dc,
            BoundingBox worldWindow,
            Matrix worldToViewportTransform)
        {
            // Notice that we use worldWindow and worldToViewportTransform here,
            // since we want full control over the pen thickness in screen pixels, regardless of zoom level.
            // If we used a Transform on the DrawingContext instead, the pen thickness would also be scaled,
            // which is not what we want.

            var drawingPen = new Pen(Brushes.IndianRed, 2); // always 2 pixels
            var selectedPen = new Pen(Brushes.Blue, 3); // always 3 pixels
            var drawingBrush = Brushes.IndianRed;
            //pen.Freeze(); // What does this do? ChatGpt talked about it

            foreach (var geometryLayer in GeometryLayers)
            {
                foreach (var geometricObject in geometryLayer.GeometricObjects)
                {
                    switch (geometricObject)
                    {
                        case Math.LineSegment2D lineSegment:
                            var p1 = worldToViewportTransform.Transform(new Point(lineSegment.Point1.X, lineSegment.Point1.Y));
                            var p2 = worldToViewportTransform.Transform(new Point(lineSegment.Point2.X, lineSegment.Point2.Y));

                            var pen1 = SelectedGeometricObjects.Contains(lineSegment)
                                ? selectedPen
                                : drawingPen;

                            dc.DrawLine(pen1, p1, p2);
                            break;

                        case Math.Point2D point:
                            var p = worldToViewportTransform.Transform(new Point(point.X, point.Y));
                            dc.DrawEllipse(drawingBrush, null, p, 3, 3);
                            break;

                        case Math.Circle2D circle:
                            var c = worldToViewportTransform.Transform(new Point(circle.Center.X, circle.Center.Y));
                            var radiusX = ViewState.Scaling.Width * circle.Radius;
                            var radiusY = ViewState.Scaling.Height * circle.Radius;
                            dc.DrawEllipse(drawingBrush, null, c, radiusX, radiusY);
                            break;

                        case VerticalLineModel verticalLine:
                            var screenX = (verticalLine.X - worldWindow.MinX) * ViewState.Scaling.Width;

                            dc.DrawLine(
                                drawingPen,
                                new Point(screenX, 0),
                                new Point(screenX, ActualHeight));
                            break;

                        case HorizontalLineModel horizontalLine:
                            var screenY = (horizontalLine.Y - worldWindow.MinY) * ViewState.Scaling.Height;

                            dc.DrawLine(
                                drawingPen,
                                new Point(0, screenY),
                                new Point(ActualWidth, screenY));
                            break;

                        case PolyLineModel polyLineModel:

                            var sg = new StreamGeometry();

                            using (var ctx = sg.Open())
                            {
                                ctx.BeginFigure(
                                    worldToViewportTransform.Transform(polyLineModel.Points.First()),
                                    isFilled: false,
                                    isClosed: false);

                                ctx.PolyLineTo(
                                    polyLineModel.Points.Skip(1)
                                        .Select(point => worldToViewportTransform.Transform(point))
                                        .ToList(),
                                    isStroked: true,
                                    isSmoothJoin: false);
                            }

                            sg.Freeze();

                            var pen2 = SelectedGeometricObjects.Contains(polyLineModel)
                                ? selectedPen
                                : drawingPen;

                            dc.DrawGeometry(null, pen2, sg);
                            break;
                    }
                }
            }

            if (_isDrawing)
            {
                var drawingStrokePointsWorld = _drawingStrokePoints
                    .Select(p => worldToViewportTransform.Transform(new Point(p.X, p.Y)))
                    .ToList();

                if (_nextPotentialDrawingStrokePoint.HasValue)
                {
                    drawingStrokePointsWorld.Add(worldToViewportTransform.Transform(new Point(
                        _nextPotentialDrawingStrokePoint.Value.X,
                        _nextPotentialDrawingStrokePoint.Value.Y)));
                }

                drawingStrokePointsWorld.AdjacentPairs().ToList().ForEach(_ =>
                {
                    dc.DrawLine(drawingPen, _.Item1, _.Item2);
                });
            }
            else
            {
                if (_potentialSelectionWindow != null)
                {
                    var selectionWindowRect = new Rect(new Point(
                            _potentialSelectionWindow.MinX,
                            _potentialSelectionWindow.MinY),
                        new Size(
                            _potentialSelectionWindow.Width,
                            _potentialSelectionWindow.Height));

                    dc.DrawRectangle(_selectionWindowBrush, _selectionWindowPen, selectionWindowRect);
                }
            }
        }

        private void DrawGeometriesDebug(
            DrawingContext dc)
        {
            var drawingPen = new Pen(Brushes.IndianRed, 2); // always 2 pixels
            var drawingBrush = Brushes.IndianRed;
            //pen.Freeze(); // What does this do? ChatGpt talked about it

            foreach (var geometryLayer in GeometryLayers)
            {
                foreach (var geometricObject in geometryLayer.GeometricObjects)
                {
                    switch (geometricObject)
                    {
                        case Math.LineSegment2D lineSegment:
                            dc.DrawLine(
                                drawingPen,
                                new Point(lineSegment.Point1.X, lineSegment.Point1.Y),
                                new Point(lineSegment.Point2.X, lineSegment.Point2.Y));
                            break;

                        case Math.Point2D point:
                            dc.DrawEllipse(drawingBrush, null, new Point(point.X, point.Y), 3, 3);
                            break;

                        case PolyLineModel polyLineModel:

                            var sg = new StreamGeometry();

                            using (var ctx = sg.Open())
                            {
                                ctx.BeginFigure(
                                    polyLineModel.Points.First(),
                                    isFilled: false,
                                    isClosed: false);

                                ctx.PolyLineTo(
                                    polyLineModel.Points.Skip(1).ToList(),
                                    isStroked: true,
                                    isSmoothJoin: false);
                            }

                            sg.Freeze();
                            dc.DrawGeometry(null, drawingPen, sg);
                            break;

                        case Math.Circle2D circle:
                            dc.DrawEllipse(drawingBrush, null,
                                new Point(circle.Center.X, circle.Center.Y),
                                circle.Radius, circle.Radius);
                            break;
                    }
                }
            }
        }

        protected override void OnRenderSizeChanged(
            SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (_worldWindowLimiter == null)
            {
                return;
            }

            var worldWindow = ComputeWorldWindow();

            var proposedWorldWindow = new BoundingBox(
                worldWindow.MinX,
                worldWindow.MinX + sizeInfo.NewSize.Width / ViewState.Scaling.Width,
                worldWindow.MinY,
                worldWindow.MinY + sizeInfo.NewSize.Height / ViewState.Scaling.Height);

            UpdateViewState(proposedWorldWindow);
            InvalidateVisual();
        }

        protected override void OnMouseDown(
            MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (_target != null)
            {
                // The window is sliding towards a target, but the user intercepts that by clicking
                _target = null;
                _current = null;
            }

            _mouseDownPosition = e.GetPosition(this);

            switch (CanvasMode)
            {
                case CanvasMode.Select:

                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        // Start panning
                        Mouse.OverrideCursor = Cursors.Hand;
                        _isPanning = true;
                        _panStartWorldOrigin = ViewState.WorldOrigin;
                    }

                    CaptureMouse();

                    break;

                case CanvasMode.Draw:

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        var transform = CreateViewportToWorldTransform(WorldWindow, RenderSize);
                        var selectedWorldPoint = transform.Transform(_mouseDownPosition);

                        if (SnapToGrid)
                        {
                            selectedWorldPoint = SnapPointToGrid(selectedWorldPoint);
                        }

                        // Add a point to the current drawing stroke
                        _drawingStrokePoints.Add(selectedWorldPoint);

                        switch (e.ClickCount)
                        {
                            case 1:

                                if (!_isDrawing)
                                {
                                    _isDrawing = true;
                                    CaptureMouse();
                                }
                                
                                break;

                            case 2:

                                SetCurrentValue(DrawingStrokePointsProperty, _drawingStrokePoints);
                                _drawingStrokePoints.Clear();
                                _isDrawing = false;
                                ReleaseMouseCapture();
                                InvalidateVisual();
                                break;
                        }
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    {
                        // Start panning
                        Mouse.OverrideCursor = Cursors.Hand;
                        _isPanning = true;
                        _panStartWorldOrigin = ViewState.WorldOrigin;
                        CaptureMouse();
                    }

                    break;
            }
        }

        protected override void OnMouseMove(
            MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePos = e.GetPosition(this);

            if (_isPanning)
            {
                var deltaViewport = _mouseDownPosition - mousePos;

                var deltaWorld = new Vector(
                    deltaViewport.X / ViewState.Scaling.Width,
                    deltaViewport.Y / ViewState.Scaling.Height);

                var worldWindow = ComputeWorldWindow();

                var proposedWorldWindow = new BoundingBox(
                    _panStartWorldOrigin.X + deltaWorld.X,
                    _panStartWorldOrigin.X + deltaWorld.X + worldWindow.Width,
                    _panStartWorldOrigin.Y + deltaWorld.Y,
                    _panStartWorldOrigin.Y + deltaWorld.Y + worldWindow.Height);

                UpdateViewState(proposedWorldWindow);
            }
            else
            {
                // Show the cursor position
                if (IsMouseOver)
                {
                    var scaleX = ViewState.Scaling.Width;
                    var scaleY = ViewState.Scaling.Height;
                    var worldX = ViewState.WorldOrigin.X + mousePos.X / scaleX;
                    var worldY = ViewState.WorldOrigin.Y + mousePos.Y / scaleY;
                    CursorWorldPosition = new Point(worldX, worldY);
                }
                else
                {
                    CursorWorldPosition = null;
                }

                switch (CanvasMode)
                {
                    case CanvasMode.Select:

                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            var deltaViewport = _mouseDownPosition - mousePos;

                            if (System.Math.Abs(deltaViewport.X) > 1 || System.Math.Abs(deltaViewport.Y) > 1)
                            {
                                _potentialSelectionWindow = new BoundingBox(
                                    System.Math.Min(_mouseDownPosition.X, mousePos.X),
                                    System.Math.Max(_mouseDownPosition.X, mousePos.X),
                                    System.Math.Min(_mouseDownPosition.Y, mousePos.Y),
                                    System.Math.Max(_mouseDownPosition.Y, mousePos.Y));

                                InvalidateVisual();
                            }
                        }

                        break;

                    case CanvasMode.Draw:

                        if (_isDrawing)
                        {
                            var transform = CreateViewportToWorldTransform(WorldWindow, RenderSize);
                            _nextPotentialDrawingStrokePoint = transform.Transform(mousePos);

                            if (SnapToGrid && _nextPotentialDrawingStrokePoint.HasValue)
                            {
                                _nextPotentialDrawingStrokePoint = SnapPointToGrid(
                                    _nextPotentialDrawingStrokePoint.Value);
                            }

                            InvalidateVisual();
                        }

                        break;
                }
            }
        }

        protected override void OnMouseUp(
            MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isPanning)
            {
                _isPanning = false;
            }
            else
            {
                switch (CanvasMode)
                {
                    case CanvasMode.Select:

                        if (e.LeftButton == MouseButtonState.Released)
                        {
                            var mouseUpPosition = e.GetPosition(this);
                            var transform = CreateViewportToWorldTransform(WorldWindow, RenderSize);
                            var delta = mouseUpPosition - _mouseDownPosition;

                            if (System.Math.Abs(delta.X) > 1 || System.Math.Abs(delta.Y) > 1)
                            {
                                var point1 = transform.Transform(new Point(
                                    _potentialSelectionWindow.MinX,
                                    _potentialSelectionWindow.MinY));

                                var point2 = transform.Transform(new Point(
                                    _potentialSelectionWindow.MaxX,
                                    _potentialSelectionWindow.MaxY));

                                SetCurrentValue(
                                    SelectionWindowProperty,
                                    new BoundingBox(point1.X, point2.X, point1.Y, point2.Y));

                                ReleaseMouseCapture();
                            }
                            else
                            {
                                ClickedWorldPosition = transform.Transform(_mouseDownPosition);
                            }

                            _potentialSelectionWindow = null;
                            InvalidateVisual();
                        }

                        break;

                    case CanvasMode.Draw:
                        break;
                }
            }

            ShowDefaultCursorForMode();
            ReleaseMouseCapture();
        }

        protected override void OnMouseLeave(
            MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!_isPanning)
            {
                CursorWorldPosition = null;
            }
        }

        protected override void OnMouseWheel(
            MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            if (_target != null)
            {
                _target = null;
                _current = null;
            }

            var mousePos = e.GetPosition(this);
            var worldWindow = ComputeWorldWindow();
            
            // 1. World point under cursor BEFORE zoom
            var transform = CreateViewportToWorldTransform(worldWindow, RenderSize);
            var worldBefore = transform.Transform(mousePos);

            // 2. Determine new zoom level
            var steps = e.Delta / 120; // standard wheel notch

            var ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
            var alt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;

            var proposedScalingWidth = ViewState.Scaling.Width;
            var proposedScalingHeight = ViewState.Scaling.Height;

            // Determine new desired zoom level
            if (LockAspectRatio || !ctrl && !alt)
            {
                proposedScalingWidth *= System.Math.Pow(_zoomingFactor, steps);
                proposedScalingHeight *= System.Math.Pow(_zoomingFactor, steps);
            }
            else if (ctrl)
            {
                // X only
                proposedScalingWidth *= System.Math.Pow(_zoomingFactor, steps);
            }
            else if (alt)
            {
                // Y only
                proposedScalingHeight *= System.Math.Pow(_zoomingFactor, steps);
            }

            var scaling = new Size(
                proposedScalingWidth,
                proposedScalingHeight);

            // 3. Compute new origin so cursor stays fixed
            var proposedOriginX = worldBefore.X - mousePos.X / scaling.Width;
            var proposedOriginY = worldBefore.Y - mousePos.Y / scaling.Height;

            // 4. Apply
            var proposedWorldWindow = new BoundingBox(
                proposedOriginX,
                proposedOriginX + ActualWidth / scaling.Width,
                proposedOriginY,
                proposedOriginY + ActualHeight / scaling.Height);

            UpdateViewState(proposedWorldWindow);
        }

        private BoundingBox ComputeWorldWindow()
        {
            // This is BY DEFINITION the world window, i.e it is derived by the parameters: origin, scaling, and viewport size
            var worldWindow = new BoundingBox(
                ViewState.WorldOrigin.X,
                ViewState.WorldOrigin.X + ActualWidth / ViewState.Scaling.Width,
                ViewState.WorldOrigin.Y,
                ViewState.WorldOrigin.Y + ActualHeight / ViewState.Scaling.Height);

            return worldWindow;
        }

        // =============================
        // Transform
        // =============================
        private Matrix CreateViewportToWorldTransform(
            BoundingBox worldWindow, 
            Size viewport)
        {
            var scaleX = worldWindow.Width / viewport.Width;
            var scaleY = worldWindow.Height / viewport.Height;

            var m = Matrix.Identity;

            m.Scale(scaleX, scaleY);
            m.Translate(worldWindow.MinX, worldWindow.MinY);

            return m;
        }

        private Matrix CreateWorldToViewportTransform(
            BoundingBox worldWindow,
            Size viewport)
        {
            var scaleX = viewport.Width / worldWindow.Width;
            var scaleY = viewport.Height / worldWindow.Height;

            var m = Matrix.Identity;

            m.Translate(-worldWindow.MinX, -worldWindow.MinY);
            m.Scale(scaleX, scaleY);

            return m;
        }

        private void UpdateViewState(
            BoundingBox proposedWorldWindow)
        {
            if (_worldWindowLimiter == null)
            {
                return;
            }

            var worldWindow = ComputeWorldWindow();

            if (LockXAxis)
            {
                proposedWorldWindow = new BoundingBox(
                    worldWindow.MinX,
                    worldWindow.MaxX,
                    proposedWorldWindow.MinY,
                    proposedWorldWindow.MaxY);
            }

            if (LockYAxis)
            {
                proposedWorldWindow = new BoundingBox(
                    proposedWorldWindow.MinX,
                    proposedWorldWindow.MaxX,
                    worldWindow.MinY,
                    worldWindow.MaxY);
            }

            var newWorldWindow = _worldWindowLimiter.Limit(proposedWorldWindow);

            SetCurrentValue(WorldWindowProperty, newWorldWindow);

            var newScalingX = ActualWidth / newWorldWindow.Width;
            var newScalingY = ActualHeight / newWorldWindow.Height;

            ViewState = new ViewState(
                new Point(
                    newWorldWindow.MinX,
                    newWorldWindow.MinY),
                new Size(
                    newScalingX,
                    newScalingY
                ));
        }

        private void DrawHorizontalGridLines(
            DrawingContext dc)
        {
            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            var world = ComputeWorldWindow();
            var pen = new Pen(Brushes.LightGray, 1);

            var scaleY = ViewState.Scaling.Height;
            var stepY = GetNiceStep(scaleY);
            for (var y = System.Math.Floor(world.MinY / stepY) * stepY; y < world.MaxY; y += stepY)
            {
                var screenY = (y - world.MinY) * scaleY;

                dc.DrawLine(
                    pen,
                    new Point(0, screenY),
                    new Point(ActualWidth, screenY));
            }
        }

        private void DrawVerticalGridLines(
            DrawingContext dc)
        {
            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            var world = ComputeWorldWindow();
            var pen = new Pen(Brushes.LightGray, 1);

            var scaleX = ViewState.Scaling.Width;
            var stepX = GetNiceStep(scaleX);
            for (var x = System.Math.Floor(world.MinX / stepX) * stepX; x < world.MaxX; x += stepX)
            {
                var screenX = (x - world.MinX) * scaleX;

                dc.DrawLine(
                    pen,
                    new Point(screenX, 0),
                    new Point(screenX, ActualHeight));
            }
        }

        private void DrawAxes(
            DrawingContext dc,
            bool horizontalAxis,
            bool verticalAxis)
        {
            var pen = new Pen(Brushes.Black, 2);
            var world = ComputeWorldWindow();

            if (verticalAxis && world.MinX <= 0 && world.MaxX >= 0)
            {
                var screenX = WorldToScreenX(0);

                dc.DrawLine(
                    pen,
                    new Point(screenX, 0),
                    new Point(screenX, ActualHeight));
            }

            if (horizontalAxis && world.MinY <= 0 && world.MaxY >= 0)
            {
                var screenY = WorldToScreenY(0);

                dc.DrawLine(
                    pen,
                    new Point(0, screenY),
                    new Point(ActualWidth, screenY));
            }
        }

        private void DrawAxisTicks(
            DrawingContext dc,
            bool horizontalAxisTicks,
            bool verticalAxisTicks)
        {
            var scaleX = ViewState.Scaling.Width;
            var scaleY = ViewState.Scaling.Height;
            var stepX = GetNiceStep(scaleX);
            var stepY = GetNiceStep(scaleY);
            var world = ComputeWorldWindow();
            var pen = new Pen(Brushes.Black, 1);
            var tickSize = 5;

            if (horizontalAxisTicks && world.MinY <= 0 && world.MaxY >= 0)
            {
                var y = WorldToScreenY(0);

                for (var x = System.Math.Floor(world.MinX / stepX) * stepX; x < world.MaxX; x += stepX)
                {
                    var sx = WorldToScreenX(x);

                    dc.DrawLine(
                        pen,
                        new Point(sx, y - tickSize),
                        new Point(sx, y + tickSize));
                }
            }

            if (verticalAxisTicks && world.MinX <= 0 && world.MaxX >= 0)
            {
                var x = WorldToScreenX(0);

                for (var y = System.Math.Floor(world.MinY / stepY) * stepY; y < world.MaxY; y += stepY)
                {
                    var sy = WorldToScreenY(y);

                    dc.DrawLine(
                        pen,
                        new Point(x - tickSize, sy),
                        new Point(x + tickSize, sy));
                }
            }
        }

        private void DrawHorizontalGridLabels(
            DrawingContext dc)
        {
            var scaleY = ViewState.Scaling.Height;
            var stepY = GetNiceStep(scaleY);
            var world = ComputeWorldWindow();
            var typeface = new Typeface("Segoe UI");
            double fontSize = 10;
            double margin = 4;

            var xScreen = margin;

            for (var y = System.Math.Floor(world.MinY / stepY) * stepY; y < world.MaxY; y += stepY)
            {
                var sy = WorldToScreenY(y);

                if (sy < 0 || sy > ActualHeight)
                    continue;

                var text = System.Math.Abs(y) > 9000
                    ? $"{y / 1:0.###e0}"
                    : $"{y / 1:G}";

                var formattedText = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black,
                    1.0);

                dc.DrawText(
                    formattedText,
                    new Point(xScreen, sy - formattedText.Height / 2));
            }
        }

        private void DrawVerticalGridLabels(
            DrawingContext dc)
        {
            var scaleX = ViewState.Scaling.Width;
            var stepX = GetNiceStep(scaleX);
            var world = ComputeWorldWindow();
            var typeface = new Typeface("Segoe UI");
            double fontSize = 10;
            double margin = 4;

            var yScreen = ActualHeight - margin;

            for (var x = System.Math.Floor(world.MinX / stepX) * stepX; x < world.MaxX; x += stepX)
            {
                var sx = WorldToScreenX(x);

                // Skip if outside screen (safety)
                if (sx < 0 || sx > ActualWidth)
                    continue;

                var text = System.Math.Abs(x) > 9000
                    ? $"{x / 1:0.###e0}"
                    : $"{x / 1:G}";

                var formattedText = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black,
                    1.0);

                // Center text under grid line
                dc.DrawText(
                    formattedText,
                    new Point(sx - formattedText.Width / 2, yScreen - formattedText.Height));
            }
        }

        private double GetNiceStep(
            double scaling)
        {
            var targetPixels = 100; // desired spacing

            var rawStep = targetPixels / scaling;

            var magnitude = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(rawStep)));
            var normalized = rawStep / magnitude;

            double nice;

            if (normalized < 1.5) nice = 1;
            else if (normalized < 3) nice = 2;
            else if (normalized < 7) nice = 5;
            else nice = 10;

            return nice * magnitude;
        }

        private double WorldToScreenX(
            double worldX)
        {
            return (worldX - ViewState.WorldOrigin.X) * ViewState.Scaling.Width;
        }

        private double WorldToScreenY(
            double worldY)
        {
            return (worldY - ViewState.WorldOrigin.Y) * ViewState.Scaling.Height;
        }

        private Transform CreateDebugShrinkTransform(
            BoundingBox worldWindow,
            double scale)
        {
            var cx = (worldWindow.MinX + worldWindow.MaxX) / 2.0;
            var cy = (worldWindow.MinY + worldWindow.MaxY) / 2.0;

            var group = new TransformGroup();

            group.Children.Add(new TranslateTransform(-cx, -cy));
            group.Children.Add(new ScaleTransform(scale, scale));
            group.Children.Add(new TranslateTransform(cx, cy));

            return group;
        }

        private Rect ToRect(
            BoundingBox box)
        {
            return new Rect(
                new Point(box.MinX, box.MinY),
                new Point(box.MaxX, box.MaxY));
        }

        private static void OnWorldWindowChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;
            canvas.OnWorldWindowChanged((BoundingBox)e.NewValue);
        }

        private static void OnRequestedWorldWindowChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;
            canvas.OnRequestedWorldWindowChanged((BoundingBox)e.NewValue);
        }

        private static void OnRequestedWorldFocusChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;
            canvas.OnRequestedWorldFocusChanged((WorldFocusRequest)e.NewValue);
        }

        private static void OnWorldWindowBoundsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;
            canvas.OnWorldWindowBoundsChanged((BoundingBox)e.NewValue);
        }

        private void OnWorldWindowChanged(
            BoundingBox? worldWindow)
        {
            if (worldWindow == null)
            {
                return;
            }

            if (WorldWindowExpanded == null ||
                !WorldWindowExpanded.Contains(worldWindow) ||
                WorldWindowExpanded.Width / worldWindow.Width > 2.0)
            {
                var expandedWorldWindow = worldWindow.Expand(1.2);
                expandedWorldWindow = _worldWindowLimiter.Limit(expandedWorldWindow);
                SetCurrentValue(WorldWindowExpandedProperty, expandedWorldWindow);
            }
        }

        private void OnRequestedWorldWindowChanged(
            BoundingBox? requestedWorldWindow)
        {
            if (requestedWorldWindow == null)
            {
                return;
            }

            var proposedWorldWindow = _worldWindowLimiter.Limit(requestedWorldWindow);

            if (LockAspectRatio)
            {
                // Make sure to preserve aspect ratio while also ensuring that the entire proposed world window is visible
                var aspectRatioViewport = ActualWidth / ActualHeight;
                var aspectRatioProposed = proposedWorldWindow.Width / proposedWorldWindow.Height;
                var minXAdjusted = 0.0;
                var minYAdjusted = 0.0;

                if (aspectRatioProposed > aspectRatioViewport)
                {
                    // Increase height of proposed world window
                    var heightAdjusted = proposedWorldWindow.Width / aspectRatioViewport;
                    minYAdjusted = proposedWorldWindow.CenterY - heightAdjusted / 2;

                    proposedWorldWindow = new BoundingBox(
                        proposedWorldWindow.MinX,
                        proposedWorldWindow.MaxX,
                        minYAdjusted,
                        minYAdjusted + heightAdjusted);
                }
                else if (aspectRatioProposed < aspectRatioViewport)
                {
                    // Increase width of proposed world window
                    var widthAdjusted = proposedWorldWindow.Height * aspectRatioViewport;
                    minXAdjusted = proposedWorldWindow.CenterX - widthAdjusted / 2;

                    proposedWorldWindow = new BoundingBox(
                        minXAdjusted,
                        minXAdjusted + widthAdjusted,
                        proposedWorldWindow.MinY,
                        proposedWorldWindow.MaxY);
                }

                // Now we have a world window that preserves aspect ratio but it might be too big to fit inside bounds. If so, we scale it uniformly down
                if (proposedWorldWindow.Width > WorldWindowBounds.Width ||
                    proposedWorldWindow.Height > WorldWindowBounds.Height)
                {
                    var scaleX = WorldWindowBounds.Width / proposedWorldWindow.Width;
                    var scaleY = WorldWindowBounds.Height / proposedWorldWindow.Height;
                    var scale = System.Math.Min(1, System.Math.Min(scaleX, scaleY));
                    var newWidth = proposedWorldWindow.Width * scale;
                    var newHeight = proposedWorldWindow.Height * scale;
                    minXAdjusted = proposedWorldWindow.CenterX - newWidth / 2;
                    minYAdjusted = proposedWorldWindow.CenterY - newHeight / 2;

                    proposedWorldWindow = new BoundingBox(
                        minXAdjusted,
                        minXAdjusted + newWidth,
                        minYAdjusted,
                        minYAdjusted + newHeight);
                }

                // Now the window size fits the bounds and still has the correct aspect ratio, but it might still exceed the bounds in terms of position
                var minCx = WorldWindowBounds.MinX + proposedWorldWindow.Width / 2;
                var maxCx = WorldWindowBounds.MaxX - proposedWorldWindow.Width / 2;
                var minCy = WorldWindowBounds.MinY + proposedWorldWindow.Height / 2;
                var maxCy = WorldWindowBounds.MaxY - proposedWorldWindow.Height / 2;
                var newCenterX = System.Math.Max(System.Math.Min(proposedWorldWindow.CenterX, maxCx), minCx);
                var newCenterY = System.Math.Max(System.Math.Min(proposedWorldWindow.CenterY, maxCy), minCy);
                minXAdjusted = newCenterX - proposedWorldWindow.Width / 2;
                minYAdjusted = newCenterY - proposedWorldWindow.Height / 2;

                proposedWorldWindow = new BoundingBox(
                    minXAdjusted,
                    minXAdjusted + proposedWorldWindow.Width,
                    minYAdjusted,
                    minYAdjusted + proposedWorldWindow.Height);
            }

            if (DampFocusShifts)
            {
                _current = ComputeWorldWindow();
                _target = proposedWorldWindow;
            }
            else
            {
                UpdateViewState(proposedWorldWindow);
            }
        }

        private void OnRequestedWorldFocusChanged(
            WorldFocusRequest? requestedWorldFocus)
        {
            if (requestedWorldFocus == null)
            {
                return;
            }

            var worldWindow = ComputeWorldWindow();

            var proposedWidth = worldWindow.Width;
            var proposedHeight = worldWindow.Height;

            if (requestedWorldFocus.Scaling.HasValue)
            {
                proposedWidth = ActualWidth * requestedWorldFocus.Scaling.Value.Width;
                proposedHeight = ActualHeight * requestedWorldFocus.Scaling.Value.Height;
            }

            var minX = requestedWorldFocus.WorldPoint.X - requestedWorldFocus.ViewportRatio.Width * proposedWidth;
            var minY = requestedWorldFocus.WorldPoint.Y - requestedWorldFocus.ViewportRatio.Height * proposedHeight;
            var maxX = minX + proposedWidth;
            var maxY = minY + proposedHeight;

            var proposedWorldWindow = new BoundingBox(minX, maxX, minY, maxY);

            if (DampFocusShifts)
            {
                _current = worldWindow;
                _target = _worldWindowLimiter.Limit(proposedWorldWindow);
            }
            else
            {
                UpdateViewState(proposedWorldWindow);
            }
        }

        private void OnWorldWindowBoundsChanged(
            BoundingBox? worldWindowBounds)
        {
            if (worldWindowBounds == null)
            {
                return;
            }

            _worldWindowLimiter = new WorldWindowLimiter(worldWindowBounds);
            UpdateViewState(ComputeWorldWindow());
        }

        private static void OnCanvasModeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;
            canvas.OnCanvasModeChanged((CanvasMode)e.NewValue);
        }

        private void OnCanvasModeChanged(
            CanvasMode canvasMode)
        {
            ShowDefaultCursorForMode();
        }

        private void ShowDefaultCursorForMode()
        {
            switch (CanvasMode)
            {
                case CanvasMode.Select:
                    Mouse.OverrideCursor = Cursors.Arrow;
                    break;
                case CanvasMode.Draw:
                    Mouse.OverrideCursor = Cursors.Pen;
                    break;
            }
        }

        private void OnRendering(
            object sender,
            EventArgs e)
        {
            if (e is not RenderingEventArgs args)
            {
                return;
            }

            if (!IsLoaded)
            {
                return;
            }

            if (_lastTime == TimeSpan.Zero)
            {
                _lastTime = args.RenderingTime;
                return;
            }

            // Determine the timespan since the previous frame
            var dt = (args.RenderingTime - _lastTime).TotalSeconds;
            _lastTime = args.RenderingTime;

            UpdateCamera(dt);

            FrameRendering?.Invoke(this,
                new FrameEventArgs(args.RenderingTime, dt));
        }

        private double Lerp(
            double a,
            double b,
            double t)
        {
            return a + (b - a) * t;
        }

        private void UpdateCamera(
            double dt)
        {
            if (_target == null)
            {
                return;
            }

            if (System.Math.Abs(_current.MinX - _target.MinX) < 0.001 &&
                System.Math.Abs(_current.MinY - _target.MinY) < 0.001)
            {
                _current = _target;
                _target = null;
            }
            else
            {
                var ratio = 1 - System.Math.Exp(-FocusShiftDamping * dt);
                var width = Lerp(_current.Width, _target.Width, ratio);
                var height = Lerp(_current.Height, _target.Height, ratio);
                var minX = Lerp(_current.MinX, _target.MinX, ratio);
                var minY = Lerp(_current.MinY, _target.MinY, ratio);
                var maxX = minX + width;
                var maxY = minY + height;

                _current = new BoundingBox(
                    minX, maxX, minY, maxY);
            }

            UpdateViewState(_current);

            if (_target == null)
            {
                _current = null;
            }
        }

        private Point SnapPointToGrid(
            Point point)
        {
            return new Point(
                System.Math.Round(point.X / GridSpacing) * GridSpacing,
                System.Math.Round(point.Y / GridSpacing) * GridSpacing);
        }
    }
}
