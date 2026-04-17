using Craft.ViewModels.Geometry2D.Reborn;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Craft.DataStructures.Geometry;
using Point = System.Windows.Point;

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
        private Point _panStartMouse;
        private Point _panStartWorldOrigin;

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

        public Point? CursorWorldPosition
        {
            get => (Point)GetValue(CursorWorldPositionProperty);
            set => SetValue(CursorWorldPositionProperty, value);
        }

        public static readonly DependencyProperty ViewStateProperty =
            DependencyProperty.Register(
                nameof(ViewState),
                typeof(ViewState),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(
                    new ViewState(new Point(0, 0), new Size(1, 1)), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CursorWorldPositionProperty =
            DependencyProperty.Register(
                nameof(CursorWorldPosition),
                typeof(Point?),
                typeof(GeometryCanvas),
                new FrameworkPropertyMetadata(null));

        // =============================
        // Handle collection changes
        // =============================
        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = (GeometryCanvas)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= canvas.OnCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += canvas.OnCollectionChanged;

            canvas.InvalidateVisual();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }

        // =============================
        // Rendering
        // =============================
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Debug background
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));

            var worldWindow = ComputeWorldWindow();

            if (Items == null || worldWindow.IsEmpty)
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
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePos = e.GetPosition(this);

            if (_isPanning)
            {
                var deltaPixel = _panStartMouse - mousePos;

                var deltaWorld = new Vector(
                    deltaPixel.X / ViewState.Scaling.Width,
                    deltaPixel.Y / ViewState.Scaling.Height);

                ViewState = new ViewState(
                    new Point(
                        _panStartWorldOrigin.X + deltaWorld.X,
                        _panStartWorldOrigin.Y + deltaWorld.Y),
                    ViewState.Scaling);
            }
            else
            {
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
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isPanning)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                _isPanning = false;
                ReleaseMouseCapture();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!_isPanning)
            {
                CursorWorldPosition = null;
            }
        }

        // =============================
        // Transform
        // =============================
        private Matrix CreateWorldToViewportTransform(Rect world, Size viewport)
        {
            var scaleX = viewport.Width / world.Width;
            var scaleY = viewport.Height / world.Height;

            var m = Matrix.Identity;

            m.Translate(-world.X, -world.Y);
            m.Scale(scaleX, scaleY);

            return m;
        }

        private Rect ComputeWorldWindow()
        {
            var width = ActualWidth / ViewState.Scaling.Width;
            var height = ActualHeight / ViewState.Scaling.Height;

            var worldWindow =
                new BoundingBox(
                    ViewState.WorldOrigin.X,
                    ViewState.WorldOrigin.X + width,
                    ViewState.WorldOrigin.Y,
                    ViewState.WorldOrigin.Y + height);

            //var limiter = new WorldWindowLimiter(new BoundingBox(0, 1000, 0, 1000));
            //worldWindow = limiter.Limit(worldWindow);

            return new Rect(
                worldWindow.MinX,
                worldWindow.MinY,
                worldWindow.Width,
                worldWindow.Height);
        }
    }
}
