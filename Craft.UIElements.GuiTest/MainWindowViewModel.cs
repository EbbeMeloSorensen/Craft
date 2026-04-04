using Craft.UIElements.GuiTest.Tab3;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Tab3ViewModel Tab3ViewModel { get; }

        public MainWindowViewModel()
        {
            Tab3ViewModel = new Tab3ViewModel();
        }
    }
}
