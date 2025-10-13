using GalaSoft.MvvmLight;
using Craft.Algorithms.GuiTest.Tab1;
using Craft.Algorithms.GuiTest.Tab2;
using Craft.Algorithms.GuiTest.Tab3;
using Craft.Algorithms.GuiTest.Tab4;
using Craft.Algorithms.GuiTest.Tab5;
using Craft.Algorithms.GuiTest.Tab6;
using Craft.Algorithms.GuiTest.Tab7;
using Craft.Algorithms.GuiTest.Tab8;
using Craft.Algorithms.GuiTest.Tab9;

namespace Craft.Algorithms.GuiTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Tab1ViewModel Tab1ViewModel { get; }
        public Tab2ViewModel Tab2ViewModel { get; }
        public Tab3ViewModel Tab3ViewModel { get; }
        public Tab4ViewModel Tab4ViewModel { get; }
        public Tab5ViewModel Tab5ViewModel { get; }
        public Tab6ViewModel Tab6ViewModel { get; }
        public Tab7ViewModel Tab7ViewModel { get; }
        public Tab8ViewModel Tab8ViewModel { get; }
        public Tab9ViewModel Tab9ViewModel { get; }

        public MainWindowViewModel()
        {
            Tab1ViewModel = new Tab1ViewModel();
            Tab2ViewModel = new Tab2ViewModel(1200, 900);
            Tab3ViewModel = new Tab3ViewModel(1200, 900);
            Tab4ViewModel = new Tab4ViewModel(1200, 900);
            Tab5ViewModel = new Tab5ViewModel();
            Tab6ViewModel = new Tab6ViewModel();
            Tab7ViewModel = new Tab7ViewModel();
            Tab8ViewModel = new Tab8ViewModel();
            Tab9ViewModel = new Tab9ViewModel();
        }
    }
}
