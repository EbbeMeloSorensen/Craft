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
            if (DataContext is GeometryViewModel vm)
            {
                vm.OnFrame(e.Time, e.DeltaSeconds);
            }
        }
    }
}
