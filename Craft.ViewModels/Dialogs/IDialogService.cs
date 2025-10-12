using System.Windows;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Dialogs
{
    public enum DialogResult
    {
        Undefined,
        OK,
        Cancel
    }

    public interface IDialogService
    {
        DialogResult ShowDialog(
            ViewModelBase viewModel, 
            Window owner);
    }
}
