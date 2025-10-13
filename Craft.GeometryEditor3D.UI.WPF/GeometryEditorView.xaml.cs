using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Craft.GeometryEditor3D.ViewModel;
using Craft.Math;
using Point = System.Windows.Point;

namespace Craft.GeometryEditor3D.UI.WPF
{
    /// <summary>
    /// Interaction logic for GeometryEditorView.xaml
    /// </summary>
    public partial class GeometryEditorView : UserControl
    {
        private GeometryEditorViewModel ViewModel => DataContext as GeometryEditorViewModel;
        private Point _mouseDownViewport;
        private bool _dragging;

        public GeometryEditorView()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Todo:
            // Her har jeg måttet lave en ændring i forhold til det oprindelige for at få det til at virke.
            // Jeg er ikke sikker på at denne konstruktion er optimal

            //ViewModel.ViewPortSize = e.NewSize;
            ViewModel.ViewPortSize = new System.Drawing.Size(
                (int)e.NewSize.Width,
                (int)e.NewSize.Height);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDownViewport = e.GetPosition((FrameworkElement)sender);
            Canvas.CaptureMouse();
            _dragging = true;

            ViewModel.InitiateCameraRotationModification();
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragging)
            {
                _dragging = false;
                Canvas.ReleaseMouseCapture();
            }
        }

        private void UIElement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            var mouseViewPosition = e.GetPosition((FrameworkElement)sender);

            if (_dragging)
            {
                var mouseOffsetViewPort = new Vector2D
                (
                    mouseViewPosition.X - _mouseDownViewport.X,
                    mouseViewPosition.Y - _mouseDownViewport.Y
                );

                ViewModel.UpdateCameraOrientation(-mouseOffsetViewPort.X, -mouseOffsetViewPort.Y);
            }
        }

        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }
    }
}
