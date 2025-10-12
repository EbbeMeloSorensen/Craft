using System.Windows;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.Dialogs
{
    public class DialogService : IDialogService
    {
        public DialogResult ShowDialog(
            ViewModelBase viewModel,
            Window owner)
        {
            var dialog = new DialogView
            {
                DataContext = viewModel,
                Owner = owner,
                ShowInTaskbar = false
            };

            dialog.ShowDialog();

            return ((DialogViewModelBase)dialog.DataContext).UserDialogResult;
        }
    }
}
