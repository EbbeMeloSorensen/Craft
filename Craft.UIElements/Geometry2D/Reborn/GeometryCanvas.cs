using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;

namespace Craft.UIElements.Geometry2D.Reborn
{
    public class GeometryCanvas : FrameworkElement
    {
        private const double _zoomBase = 1.2;
        private const int _minZoomLevel = -20;
        private const int _maxZoomLevel = 20;

        private int _zoomLevelX;
        private int _zoomLevelY;

        private bool _isPanning;
        private System.Windows.Point _panStartMouse;
        private System.Windows.Point _panStartWorldOrigin;

        private BoundingBox _worldWindowBounds;
        private WorldWindowLimiter _worldWindowLimiter;

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

        public System.Windows.Point? CursorWorldPosition
        {
            get => (System.Windows.Point)GetValue(CursorWorldPositionProperty);
            set => SetValue(CursorWorldPositionProperty, value);
        }

        public static readonly DependencyProperty ViewStateProperty =
            DependencyProperty.Register(
                nameof(ViewState),
                typeof(ViewState),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    new ViewState(new System.Windows.Point(0, 0), new Size(1, 1)), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CursorWorldPositionProperty =
            DependencyProperty.Register(
                nameof(CursorWorldPosition),
                typeof(System.Windows.Point?),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null));

        public GeometryCanvas()
        {
            _worldWindowBounds = new BoundingBox(0, 1000, 0, 500);
            _worldWindowLimiter = new WorldWindowLimiter(_worldWindowBounds);
        }

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

            var worldWindow = ComputeWorldWindow();

            if (Items == null /*|| worldWindow.IsEmpty*/)
                return;

            var transform = CreateWorldToViewportTransform(worldWindow, RenderSize);
            var pen = new Pen(Brushes.Black, 1); // always 1 pixel

            foreach (var item in Items)
            {
                if (item is LineModel line)
                {
                    var p1 = transform.Transform(line.P1);
                    var p2 = transform.Transform(line.P2);

                    dc.DrawLine(pen, p1, p2);
                }
            }
        }

        // Suggested by ChatGpt, but it seems unnecessary
        //protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        //{
        //    base.OnRenderSizeChanged(sizeInfo);
        //    InvalidateVisual();
        //}

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
            if (ctrl)
            {
                // X only
                proposedZoomLevelX += steps;
            }
            else if (alt)
            {
                // Y only
                proposedZoomLevelY += steps;
            }
            else
            {
                // uniform
                proposedZoomLevelX += steps;
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
            return new BoundingBox(
                ViewState.WorldOrigin.X,
                ViewState.WorldOrigin.X + ActualWidth / ViewState.Scaling.Width,
                ViewState.WorldOrigin.Y,
                ViewState.WorldOrigin.Y + ActualHeight / ViewState.Scaling.Height);
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

            var worldWindowBounds = new BoundingBox(0, 1000, 0, 500);

            // Make sure the zoom levels are adequately large to ensure the constrained world window covers the viewport
            while (System.Math.Pow(_zoomBase, proposedZoomLevelX) * worldWindowBounds.Width < ActualWidth &&
                   proposedZoomLevelX < _maxZoomLevel)
            {
                proposedZoomLevelX++;
            }

            while (System.Math.Pow(_zoomBase, proposedZoomLevelY) * worldWindowBounds.Height < ActualHeight &&
                   proposedZoomLevelY < _maxZoomLevel)
            {
                proposedZoomLevelY++;
            }

            _zoomLevelX = proposedZoomLevelX;
            _zoomLevelY = proposedZoomLevelY;
        }

        private void UpdateViewState(
            BoundingBox proposedWorldWindow)
        {
            // Just accept it for now
            //var possiblyConstrainedWorldWindow = proposedWorldWindow;

            var possiblyConstrainedWorldWindow = _worldWindowLimiter.Limit(proposedWorldWindow);

            var newScalingX = ActualWidth / proposedWorldWindow.Width;
            var newScalingY = ActualHeight / proposedWorldWindow.Height;

            // Vi transformerer tilbage fra det World Window, vi kunne få
            ViewState = new ViewState(
                new System.Windows.Point(
                    possiblyConstrainedWorldWindow.MinX,
                    possiblyConstrainedWorldWindow.MinY),
                new Size(
                    newScalingX,
                    newScalingY
                ));
        }
    }
}
