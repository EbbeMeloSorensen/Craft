using System.Windows;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Dialogs
{
    public class DialogViewModelBase : ViewModelBase
    {
        public DialogResult UserDialogResult
        {
            get;
            private set;
        }

        public void CloseDialogWithResult(
            Window dialog,
            DialogResult result)
        {
            UserDialogResult = result;

            if (dialog != null)
            {
                dialog.DialogResult = true;
            }
        }
    }
}
