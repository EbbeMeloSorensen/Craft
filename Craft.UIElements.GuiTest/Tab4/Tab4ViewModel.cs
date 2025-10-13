using Craft.ViewModels.Basic;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab4
{
    public class Tab4ViewModel : ViewModelBase
    {
        public DatePickerWithPartsViewModel DatePickerWithPartsViewModel { get; private set; }

        public Tab4ViewModel()
        {
            DatePickerWithPartsViewModel = new DatePickerWithPartsViewModel();
        }
    }
}
