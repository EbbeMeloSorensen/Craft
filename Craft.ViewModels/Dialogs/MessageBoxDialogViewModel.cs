using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace Craft.ViewModels.Dialogs
{
    public class MessageBoxDialogViewModel : DialogViewModelBase
    {
        private string _message;
        private bool _cancelButtonIsVisible;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        public bool CancelButtonIsVisible
        {
            get { return _cancelButtonIsVisible; }
            set
            {
                _cancelButtonIsVisible = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public RelayCommand<object> OKCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand<object>(OK)); }
        }

        public RelayCommand<object> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(Cancel)); }
        }

        public MessageBoxDialogViewModel(string message, bool cancelButtonIsVisible)
        {
            _message = message;
            _cancelButtonIsVisible = cancelButtonIsVisible;
        }

        private void OK(object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.OK);
        }

        private void Cancel(object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
        }
    }
}
