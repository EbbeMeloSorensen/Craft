using Craft.UIElements.GuiTest.Tab1;
using Craft.UIElements.GuiTest.Tab2;
using Craft.UIElements.GuiTest.Tab3;
using Craft.UIElements.GuiTest.Tab4;
using Craft.UIElements.GuiTest.Tab5;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Tab1ViewModel Tab1ViewModel { get; }
        public Tab2ViewModel Tab2ViewModel { get; }
        public Tab3ViewModel Tab3ViewModel { get; }
        public Tab4ViewModel Tab4ViewModel { get; }
        public Tab5ViewModel Tab5ViewModel { get; }

        public MainWindowViewModel()
        {
            Tab1ViewModel = new Tab1ViewModel();
            Tab2ViewModel = new Tab2ViewModel();
            Tab3ViewModel = new Tab3ViewModel();
            Tab4ViewModel = new Tab4ViewModel();
            Tab5ViewModel = new Tab5ViewModel();
        }
    }
}
