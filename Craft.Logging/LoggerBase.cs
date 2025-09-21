using System;
using System.Diagnostics;

namespace Craft.Logging
{
    public abstract class LoggerBase : ILogger
    {
        private Stopwatch _stopwatch;

        protected bool MeasuringTime
        {
            get => _stopwatch.IsRunning;
        }

        public LoggerBase()
        {
            _stopwatch = new Stopwatch();
        }

        public virtual string WriteLine(
            LogMessageCategory category,
            string message,
            string aspect,
            bool startStopwatch)
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                var elapsedTime = _stopwatch.Elapsed;
                _stopwatch.Reset();

                return $"{message} (elapsed time: {TimeSpanAsString(elapsedTime)})";
            }
            else if (startStopwatch)
            {
                _stopwatch.Start();
            }

            return message;
        }

        private string TimeSpanAsString(
            TimeSpan elapsedTime)
        {
            var hh = $"{elapsedTime.Hours}".PadLeft(2, '0');
            var mm = $"{elapsedTime.Minutes}".PadLeft(2, '0');
            var ss = $"{elapsedTime.Seconds}".PadLeft(2, '0');
            var ms = $"{elapsedTime.Milliseconds}".PadLeft(3, '0');

            return $"{hh}:{mm}:{ss}.{ms}";
        }
    }
}
