using Craft.UI.Utils;
using Craft.Utils;
using Craft.ViewModels.Graph;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Craft.UIElements.Graph
{
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        private PointD _mouseDownViewport;
        private PointD _initialScrollOffset;
        private bool _panning;
        private bool _draggingPoint;

        private GraphViewModel ViewModel
        {
            get { return DataContext as GraphViewModel; }
        }

        public GraphView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ViewModel == null) return;

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ViewModel == null) return;

            // Is the scroll change caused by a change in the size of the embedded Canvas?
            if (System.Math.Abs(e.ExtentWidthChange) > 0.0000001 ||
                System.Math.Abs(e.ExtentHeightChange) > 0.0000001)
            {
                ViewModel.ScrollableOffset = new PointD(
                    ScrollViewer.ScrollableWidth,
                    ScrollViewer.ScrollableHeight);

                ViewModel.ScrollOffset = new PointD(0, 0);

                return;
            }

            // Otherwise we assume it is because the user interacted with a scrollbar 
            ViewModel.ScrollOffset = new PointD(
                e.HorizontalOffset,
                e.VerticalOffset);
        }

        private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null) return;

            Canvas.CaptureMouse();
            _mouseDownViewport = e.GetPosition(ScrollViewer).AsPointD();

            if (ViewModel.PointWasClicked)
            {
                if (ViewModel.AllowMovingVertices)
                {
                    _draggingPoint = true;
                }
            }
            else
            {
                _initialScrollOffset = ViewModel.ScrollOffset;
                _panning = true;
            }
        }

        private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _draggingPoint = false;
            _panning = false;
            Canvas.ReleaseMouseCapture();
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel == null) return;

            if (_panning)
            {
                var scrollViewerPositionForMouseMove = e.GetPosition(ScrollViewer).AsPointD();
                var offset = scrollViewerPositionForMouseMove - _mouseDownViewport;

                ViewModel.ScrollOffset = new PointD(
                    System.Math.Min(System.Math.Max(0, _initialScrollOffset.X - offset.X), ViewModel.ScrollableOffset.X),
                    System.Math.Min(System.Math.Max(0, _initialScrollOffset.Y - offset.Y), ViewModel.ScrollableOffset.Y));
            }
            else
            {
                var canvasPositionForMouseMove = e.GetPosition(Canvas).AsPointD();
                ViewModel.MousePositionWorld.Object = canvasPositionForMouseMove / ViewModel.Magnification;

                if (_draggingPoint)
                {
                    var temp = e.GetPosition(ScrollViewer).AsPointD();
                    var offset = temp - _mouseDownViewport;

                    ViewModel.MovePoint(offset);
                }
            }
        }
    }
}
