using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;

namespace Craft.UIElements.Geometry2D.Reborn
{
    public class GeometryCanvas : FrameworkElement
    {
        private WorldWindowLimiter _worldWindowLimiter;

        private const double _zoomBase = 1.2;
        private const int _minZoomLevel = -20;
        private const int _maxZoomLevel = 20;

        private int _zoomLevelX;
        private int _zoomLevelY;

        private bool _isPanning;
        private System.Windows.Point _panStartMouse;
        private System.Windows.Point _panStartWorldOrigin;
        private bool _updateWorldWindowPending;

        // =============================
        // Items (your geometries)
        // =============================
        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                nameof(Items),
                typeof(IEnumerable),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null, OnItemsChanged));

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
                new FrameworkPropertyMetadata(default(BoundingBox)));

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

        public BoundingBox ExpandedWorldWindow
        {
            get => (BoundingBox)GetValue(ExpandedWorldWindowProperty);
            set => SetValue(ExpandedWorldWindowProperty, value);
        }

        public static readonly DependencyProperty ExpandedWorldWindowProperty =
            DependencyProperty.Register(
                nameof(ExpandedWorldWindow),
                typeof(BoundingBox),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(default(BoundingBox)));

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

        // =============================
        // Handle collection changes
        // =============================
        private static void OnItemsChanged(
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
            
            if (Items == null /*|| WorldWindow == null*/)
                return;

            var worldWindow = ComputeWorldWindow();
            var worldToViewportTransform = CreateWorldToViewportTransform(worldWindow, RenderSize);
            var pen = new Pen(Brushes.IndianRed, 2); // always 2 pixels
            //pen.Freeze();

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

                var expandedRect = ToRect(ExpandedWorldWindow);
                dc.DrawRectangle(null, expandedPen, expandedRect);

                var boundsRect = ToRect(WorldWindowBounds);
                dc.DrawRectangle(null, boundsPen, boundsRect);

                foreach (var item in Items)
                {
                    if (item is LineModel line)
                    {
                        dc.DrawLine(pen, line.P1, line.P2);
                    }
                }

                dc.Pop();
                dc.Pop();
            }
            else
            {
                if (ShowGrid)
                {
                    DrawGrid(dc, true, true);
                }

                if (ShowCoordinateSystem)
                {
                    DrawAxes(dc, true, true);
                    DrawAxisTicks(dc, true, true);
                    DrawGridLabels(dc, true, true);
                }

                foreach (var item in Items)
                {
                    if (item is LineModel line)
                    {
                        var p1 = worldToViewportTransform.Transform(line.P1);
                        var p2 = worldToViewportTransform.Transform(line.P2);

                        dc.DrawLine(pen, p1, p2);
                    }
                }
            }
        }

        // Suggested by ChatGpt, but it seems unnecessary
        protected override void OnRenderSizeChanged(
            SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (_worldWindowLimiter == null)
            {
                return;
            }

            var worldWindow = ComputeWorldWindow();

            // Som udgangspunkt prøver vi at holde fast i de eksisterende zoom levels
            // (men det kan være nødvendigt at zoomd ind for at sikre, at det constrainede world vindue dække view porten)
            var proposedZoomLevelX = _zoomLevelX;
            var proposedZoomLevelY = _zoomLevelY;

            UpdateZoomLevels(
                proposedZoomLevelX,
                proposedZoomLevelY);

            // Vi beregner den nye skalering, der sædvanligvis vil være uændret
            var scaling = new Size(
                System.Math.Pow(_zoomBase, _zoomLevelX),
                System.Math.Pow(_zoomBase, _zoomLevelY));

            var proposedWorldWindow = new BoundingBox(
                worldWindow.MinX,
                worldWindow.MinX + sizeInfo.NewSize.Width / scaling.Width,
                worldWindow.MinY,
                worldWindow.MinY + sizeInfo.NewSize.Height / scaling.Height);

            UpdateViewState(proposedWorldWindow);
            InvalidateVisual();
        }

        protected override void OnMouseDown(
            MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
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

            var mousePos = e.GetPosition(this);

            var worldWindow = ComputeWorldWindow();
            var transform = CreateWorldToViewportTransform(worldWindow, RenderSize);

            var inverse = transform;
            inverse.Invert();

            // 1. World point under cursor BEFORE zoom
            var worldBefore = inverse.Transform(mousePos);

            // 2. Determine new zoom level
            var steps = e.Delta / 120; // standard wheel notch

            var ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
            var alt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;

            var proposedZoomLevelX = _zoomLevelX;
            var proposedZoomLevelY = _zoomLevelY;

            // Determine new desired zoom level
            if (LockAspectRatio || !ctrl && !alt)
            {
                proposedZoomLevelX += steps;
                proposedZoomLevelY += steps;
            }
            else if (ctrl)
            {
                // X only
                proposedZoomLevelX += steps;
            }
            else if (alt)
            {
                // Y only
                proposedZoomLevelY += steps;
            }

            UpdateZoomLevels(
                proposedZoomLevelX,
                proposedZoomLevelY);

            var scaling = new Size(
                System.Math.Pow(_zoomBase, _zoomLevelX),
                System.Math.Pow(_zoomBase, _zoomLevelY));

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

        private void UpdateZoomLevels(
            int proposedZoomLevelX,
            int proposedZoomLevelY)
        {
            // If scaling is uniform, then we would like to preserve that
            var uniformZooming = proposedZoomLevelX == proposedZoomLevelY;

            // Make sure zoom level is within bounds
            if (proposedZoomLevelX > _maxZoomLevel)
            {
                proposedZoomLevelX = _maxZoomLevel;
            }
            else if (proposedZoomLevelX < _minZoomLevel)
            {
                proposedZoomLevelX = _minZoomLevel;
            }

            if (proposedZoomLevelY > _maxZoomLevel)
            {
                proposedZoomLevelY = _maxZoomLevel;
            }
            else if (proposedZoomLevelY < _minZoomLevel)
            {
                proposedZoomLevelY = _minZoomLevel;
            }

            // Make sure the zoom levels are adequately large to ensure the constrained world window covers the viewport
            while (System.Math.Pow(_zoomBase, proposedZoomLevelX) * WorldWindowBounds.Width < ActualWidth &&
                   proposedZoomLevelX < _maxZoomLevel)
            {
                proposedZoomLevelX++;
            }

            while (System.Math.Pow(_zoomBase, proposedZoomLevelY) * WorldWindowBounds.Height < ActualHeight &&
                   proposedZoomLevelY < _maxZoomLevel)
            {
                proposedZoomLevelY++;
            }

            if (uniformZooming && proposedZoomLevelX != proposedZoomLevelY)
            {
                proposedZoomLevelX = proposedZoomLevelY = System.Math.Max(proposedZoomLevelX, proposedZoomLevelY);
            }

            _zoomLevelX = proposedZoomLevelX;
            _zoomLevelY = proposedZoomLevelY;
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

            var worldWindowLimiter = new WorldWindowLimiter(WorldWindowBounds);
            var newWorldWindow = worldWindowLimiter.Limit(proposedWorldWindow);

            UpdateWorldWindowDeferred();

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

        private void DrawGrid(
            DrawingContext dc,
            bool horizontalLines,
            bool verticalLines)
        {
            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            var world = ComputeWorldWindow();
            var pen = new Pen(Brushes.LightGray, 1);

            if (horizontalLines)
            {
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

            if (verticalLines)
            {
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

        private void DrawGridLabels(
            DrawingContext dc,
            bool horizontalAxisLabels,
            bool verticalAxisLabels)
        {
            var scaleX = ViewState.Scaling.Width;
            var scaleY = ViewState.Scaling.Height;
            var stepX = GetNiceStep(scaleX);
            var stepY = GetNiceStep(scaleY);
            var world = ComputeWorldWindow();
            var typeface = new Typeface("Segoe UI");
            double fontSize = 10;
            double margin = 4;

            if (horizontalAxisLabels)
            {
                var yScreen = ActualHeight - margin;

                for (var x = System.Math.Floor(world.MinX / stepX) * stepX; x < world.MaxX; x += stepX)
                {
                    var sx = WorldToScreenX(x);

                    // Skip if outside screen (safety)
                    if (sx < 0 || sx > ActualWidth)
                        continue;

                    var text = new FormattedText(
                        $"{x / 1:G}",
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        fontSize,
                        Brushes.Black,
                        1.0);

                    // Center text under grid line
                    dc.DrawText(
                        text,
                        new System.Windows.Point(sx - text.Width / 2, yScreen - text.Height));
                }
            }

            if (verticalAxisLabels)
            {
                var xScreen = margin;

                for (var y = System.Math.Floor(world.MinY / stepY) * stepY; y < world.MaxY; y += stepY)
                {
                    var sy = WorldToScreenY(y);

                    if (sy < 0 || sy > ActualHeight)
                        continue;

                    var text = new FormattedText(
                        $"{y / 1:G}",
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        fontSize,
                        Brushes.Black,
                        1.0);

                    dc.DrawText(
                        text,
                        new System.Windows.Point(xScreen, sy - text.Height / 2));
                }
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

        private void UpdateWorldWindowDeferred()
        {
            if (_updateWorldWindowPending)
                return;

            _updateWorldWindowPending = true;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Should we perhaps update the expanded world window first? It does after all affect the line collection
                _updateWorldWindowPending = false;
                var worldWindow = ComputeWorldWindow();
                SetCurrentValue(WorldWindowProperty, worldWindow);

                if (ExpandedWorldWindow == null ||
                    !ExpandedWorldWindow.Contains(WorldWindow) ||
                    ExpandedWorldWindow.Width / WorldWindow.Width > 2.0)
                {
                    var expandedWorldWindow = worldWindow.Expand(1.2);

                    if (WorldWindowBounds != null)
                    {
                        var limiter = new WorldWindowLimiter(WorldWindowBounds);
                        expandedWorldWindow = limiter.Limit(expandedWorldWindow);
                    }

                    SetCurrentValue(ExpandedWorldWindowProperty, expandedWorldWindow);
                }

                InvalidateVisual();

            }), DispatcherPriority.Loaded);
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

        private static void OnRequestedWorldWindowChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;
            canvas.OnRequestedWorldWindowChanged((BoundingBox)e.NewValue);
        }

        private static void OnWorldWindowBoundsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;
            canvas.OnWorldWindowBoundsChanged((BoundingBox)e.NewValue);
        }

        private void OnRequestedWorldWindowChanged(
            BoundingBox requestedWorldWindow)
        {
            UpdateViewState(requestedWorldWindow);
        }

        private void OnWorldWindowBoundsChanged(
            BoundingBox worldWindowBounds)
        {
            _worldWindowLimiter = new WorldWindowLimiter(worldWindowBounds);
            var proposedWorldWindow = ComputeWorldWindow();
            UpdateViewState(proposedWorldWindow);
        }
    }
}
