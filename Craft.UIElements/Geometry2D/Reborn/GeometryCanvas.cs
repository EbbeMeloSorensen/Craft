using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Craft.UIElements.Geometry2D.Reborn
{
    public class GeometryCanvas : FrameworkElement
    {
        private const double _zoomingFactor = 1.2;
        private WorldWindowLimiter _worldWindowLimiter;

        private bool _isPanning;
        private System.Windows.Point _panStartMouse;
        private System.Windows.Point _panStartWorldOrigin;
        private TimeSpan _lastTime;
        private BoundingBox _current;
        private BoundingBox _target;

        // =============================
        // Items (your geometries)
        // =============================

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
                    new ViewState(new System.Windows.Point(0, 0), new Size(1, 1)), FrameworkPropertyMetadataOptions.AffectsRender));

        public System.Windows.Point? CursorWorldPosition
        {
            get => (System.Windows.Point)GetValue(CursorWorldPositionProperty);
            set => SetValue(CursorWorldPositionProperty, value);
        }

        public static readonly DependencyProperty CursorWorldPositionProperty =
            DependencyProperty.Register(
                nameof(CursorWorldPosition),
                typeof(System.Windows.Point?),
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

        public event EventHandler<FrameEventArgs> FrameRendering;

        public GeometryCanvas()
        {
            CompositionTarget.Rendering += OnRendering;
            Unloaded += (s, e) => CompositionTarget.Rendering -= OnRendering;
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
            
            //if (Items == null /*|| WorldWindow == null*/)
            //    return;

            if (GeometryLayers == null)
            {
                return;
            }

            var worldWindow = ComputeWorldWindow();
            var worldToViewportTransform = CreateWorldToViewportTransform(worldWindow, RenderSize);

            if (DebugMode)
            {
                dc.PushTransform(new MatrixTransform(worldToViewportTransform));

                var debugTransform = CreateDebugShrinkTransform(worldWindow, 0.5);
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

                DrawGeometries(dc, worldWindow, worldToViewportTransform);

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
                                new System.Windows.Point(tick.X, 0),
                                new System.Windows.Point(tick.X, ActualHeight));
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
                                    new System.Windows.Point(x, y));
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
            var drawingPen = new Pen(Brushes.IndianRed, 2); // always 2 pixels
            var drawingBrush = Brushes.IndianRed;
            //pen.Freeze(); // What does this do? ChatGpt talked about it

            foreach (var geometryLayer in GeometryLayers)
            {
                foreach (var geometricObject in geometryLayer.GeometricObjects)
                {
                    switch (geometricObject)
                    {
                        case LineModel line:
                            var p1 = worldToViewportTransform.Transform(line.P1);
                            var p2 = worldToViewportTransform.Transform(line.P2);
                            dc.DrawLine(drawingPen, p1, p2);
                            break;

                        case VerticalLineModel verticalLine:
                            var screenX = (verticalLine.X - worldWindow.MinX) * ViewState.Scaling.Width;

                            dc.DrawLine(
                                drawingPen,
                                new System.Windows.Point(screenX, 0),
                                new System.Windows.Point(screenX, ActualHeight));
                            break;

                        case HorizontalLineModel horizontalLine:
                            var screenY = (horizontalLine.Y - worldWindow.MinY) * ViewState.Scaling.Height;

                            dc.DrawLine(
                                drawingPen,
                                new System.Windows.Point(0, screenY),
                                new System.Windows.Point(ActualWidth, screenY));
                            break;

                        case PointModel point:
                            var p = worldToViewportTransform.Transform(point.P);
                            dc.DrawEllipse(drawingBrush, null, p, 3, 3);
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
                            dc.DrawGeometry(null, drawingPen, sg);
                            break;

                        case CircleModel circle:
                            var c = worldToViewportTransform.Transform(circle.Center);
                            var radiusX = ViewState.Scaling.Width * circle.Radius;
                            var radiusY = ViewState.Scaling.Height * circle.Radius;
                            dc.DrawEllipse(drawingBrush, null, c, radiusX, radiusY);
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

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_target != null)
                {
                    _target = null;
                    _current = null;
                }

                Mouse.OverrideCursor = Cursors.Hand;
                _isPanning = true;
                _panStartMouse = e.GetPosition(this);
                _panStartWorldOrigin = ViewState.WorldOrigin;

                CaptureMouse();
            }
        }

        protected override void OnMouseMove(
            MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePos = e.GetPosition(this);

            if (_isPanning)
            {
                var deltaPixel = _panStartMouse - mousePos;

                var deltaWorld = new Vector(
                    deltaPixel.X / ViewState.Scaling.Width,
                    deltaPixel.Y / ViewState.Scaling.Height);

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
                if (IsMouseOver)
                {
                    var scaleX = ViewState.Scaling.Width;
                    var scaleY = ViewState.Scaling.Height;
                    var worldX = ViewState.WorldOrigin.X + mousePos.X / scaleX;
                    var worldY = ViewState.WorldOrigin.Y + mousePos.Y / scaleY;
                    CursorWorldPosition = new System.Windows.Point(worldX, worldY);
                }
                else
                {
                    CursorWorldPosition = null;
                }
            }
        }

        protected override void OnMouseUp(
            MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isPanning)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                _isPanning = false;
                ReleaseMouseCapture();
            }
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
                new System.Windows.Point(
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
                    new System.Windows.Point(0, screenY),
                    new System.Windows.Point(ActualWidth, screenY));
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
                    new System.Windows.Point(screenX, 0),
                    new System.Windows.Point(screenX, ActualHeight));
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
                    new System.Windows.Point(screenX, 0),
                    new System.Windows.Point(screenX, ActualHeight));
            }

            if (horizontalAxis && world.MinY <= 0 && world.MaxY >= 0)
            {
                var screenY = WorldToScreenY(0);

                dc.DrawLine(
                    pen,
                    new System.Windows.Point(0, screenY),
                    new System.Windows.Point(ActualWidth, screenY));
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
                        new System.Windows.Point(sx, y - tickSize),
                        new System.Windows.Point(sx, y + tickSize));
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
                        new System.Windows.Point(x - tickSize, sy),
                        new System.Windows.Point(x + tickSize, sy));
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
                    new System.Windows.Point(xScreen, sy - formattedText.Height / 2));
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
                    new System.Windows.Point(sx - formattedText.Width / 2, yScreen - formattedText.Height));
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
                new System.Windows.Point(box.MinX, box.MinY),
                new System.Windows.Point(box.MaxX, box.MaxY));
        }

        private static void OnWorldWindowChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine("WorldWindow changed!");

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
            BoundingBox worldWindow)
        {
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
            BoundingBox requestedWorldWindow)
        {
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
            WorldFocusRequest requestedWorldFocus)
        {
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
            BoundingBox worldWindowBounds)
        {
            _worldWindowLimiter = new WorldWindowLimiter(worldWindowBounds);
            UpdateViewState(ComputeWorldWindow());
        }

        private void OnRendering(
            object sender,
            EventArgs e)
        {
            if (e is not RenderingEventArgs args)
                return;

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
    }
}
