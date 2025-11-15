using Craft.Simulation.GuiTest.Tab1;
using GalaSoft.MvvmLight;

namespace Craft.Simulation.GuiTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Tab1ViewModel Tab1ViewModel { get; }

        public MainWindowViewModel()
        {
            Tab1ViewModel = new Tab1ViewModel();
        }
    }
}
