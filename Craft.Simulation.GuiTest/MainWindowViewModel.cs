using Craft.Simulation.GuiTest.Tab1;
using GalaSoft.MvvmLight;

namespace Craft.Simulation.GuiTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        public BouncingBallViewModel BouncingBallViewModel { get; }

        public MainWindowViewModel()
        {
            BouncingBallViewModel = new BouncingBallViewModel();
        }
    }
}
