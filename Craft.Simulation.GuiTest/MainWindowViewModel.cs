using Craft.Simulation.GuiTest.BouncingBall;
using Craft.Simulation.GuiTest.VanishingBoundaries;
using GalaSoft.MvvmLight;

namespace Craft.Simulation.GuiTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        public BouncingBallViewModel BouncingBallViewModel { get; }
        public VanishingBoundariesViewModel VanishingBoundariesViewModel { get; }

        public MainWindowViewModel()
        {
            BouncingBallViewModel = new BouncingBallViewModel();
            VanishingBoundariesViewModel = new VanishingBoundariesViewModel();
        }
    }
}
