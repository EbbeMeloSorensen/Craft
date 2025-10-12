using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Craft.ViewModel.Utils
{
    public class LogViewModel : ViewModelBase
    {
        private int _lineCount;
        private int _maxLineCount;
        private string _log;
        private bool _logUpdated;
        private RelayCommand _clearLogsCommand;

        public string Log
        {
            get { return _log; }
            set
            {
                _log = value;
                RaisePropertyChanged();
            }
        }

        public bool LogUpdated
        {
            get { return _logUpdated; }
            set
            {
                _logUpdated = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ClearLogsCommand
        {
            get { return _clearLogsCommand ?? (_clearLogsCommand = new RelayCommand(ClearLogs)); }
        }

        public LogViewModel(
            int maxLineCount)
        {
            _log = "";
            _lineCount = 0;
            _maxLineCount = maxLineCount;
        }

        public void Append(
            string message)
        {
            if (_lineCount == _maxLineCount)
            {
                _log = _log.Substring(_log.IndexOf('\n') + 1);
                _lineCount--;
            }

            Log += $"{message}\n";
            _lineCount++;
        }

        private void ClearLogs()
        {
            Log = "";
        }
    }
}
