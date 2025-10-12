using Craft.Logging;
using System;

namespace Craft.ViewModel.Utils
{
    // This is a decorator class for writing messages in a wpf control as well as to file
    public class ViewModelLogger : LoggerBase
    {
        private ILogger _logger;
        private LogViewModel _logViewModel;

        public ViewModelLogger(
            ILogger logger, 
            LogViewModel logViewModel)
        {
            _logger = logger;
            _logViewModel = logViewModel;
        }

        public override string WriteLine(
            LogMessageCategory category,
            string message,
            string aspect,
            bool startStopWatch)
        {
            message = _logger.WriteLine(category, message, aspect, startStopWatch);
            _logViewModel.Append($"{GetCurrentTime()}: {message}");
            _logViewModel.LogUpdated = true;
            return message;
        }

        private string GetCurrentTime()
        {
            var currentTime = DateTime.Now;

            var year = $"{currentTime.Year}";
            var month = $"{currentTime.Month}".PadLeft(2, '0');
            var day = $"{currentTime.Day}".PadLeft(2, '0');
            var hh = $"{currentTime.Hour}".PadLeft(2, '0');
            var mm = $"{currentTime.Minute}".PadLeft(2, '0');
            var ss = $"{currentTime.Second}".PadLeft(2, '0');
            var ms = $"{currentTime.Millisecond}".PadLeft(3, '0');

            return $"{day}/{month}/{year} {hh}:{mm}:{ss}.{ms}";
        }
    }
}
