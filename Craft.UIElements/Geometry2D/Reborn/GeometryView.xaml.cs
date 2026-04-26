using Craft.ViewModels.Geometry2D.Reborn;
using System.Windows;
using System.Windows.Controls;

namespace Craft.UIElements.Geometry2D.Reborn
{
    /// <summary>
    /// Interaction logic for GeometryView.xaml
    /// </summary>
    public partial class GeometryView : UserControl
    {
        public IFrameAware FrameHandler
        {
            get => (IFrameAware)GetValue(FrameHandlerProperty);
            set => SetValue(FrameHandlerProperty, value);
        }

        public static readonly DependencyProperty FrameHandlerProperty =
            DependencyProperty.Register(
                nameof(FrameHandler),
                typeof(IFrameAware),
                typeof(GeometryView),
                new FrameworkPropertyMetadata(null));

        public GeometryView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(
            object sender,
            RoutedEventArgs e)
        {
            GeometryCanvas.FrameRendering += OnFrameRendering;
        }

        private void OnUnloaded(
            object sender,
            RoutedEventArgs e)
        {
            GeometryCanvas.FrameRendering -= OnFrameRendering;
        }

        private void OnFrameRendering(
            object sender,
            FrameEventArgs e)
        {
            FrameHandler?.OnFrame(e.Time, e.DeltaSeconds);
        }
    }
}
