using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Craft.ViewModels.Tasks
{
    public class TaskViewModel : ViewModelBase
    {
        private bool _busy;
        private bool _abortPossible;
        private double _progress;
        private string _nameOfTask;
        private string _nameOfCurrentSubtask;

        public bool Busy
        {
            get => _busy;
            set
            {
                _busy = value;
                RaisePropertyChanged();
            }
        }

        public bool AbortPossible
        {
            get => _abortPossible;
            set
            {
                _abortPossible = value;
                RaisePropertyChanged();
            }
        }

        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

        public string NameOfTask
        {
            get => _nameOfTask;
            set
            {
                _nameOfTask = value;
                RaisePropertyChanged();
            }
        }

        public string NameOfCurrentSubtask
        {
            get => _nameOfCurrentSubtask;
            set
            {
                _nameOfCurrentSubtask = value;
                RaisePropertyChanged();
            }
        }

        public bool Abort { get; set; }

        public RelayCommand AbortCommand { get; }

        public TaskViewModel()
        {
            _progress = 0;
            _nameOfTask = "";
            _nameOfCurrentSubtask = "";

            AbortCommand = new RelayCommand(() =>
            {
                Abort = true;
            });
        }

        public void Show(
            string nameOfTask,
            bool abortPossible)
        {
            Progress = 1;
            NameOfTask = nameOfTask;
            AbortPossible = abortPossible;
            Abort = false;
            Busy = true;
        }

        public void Hide()
        {
            Busy = false;
        }
    }
}
