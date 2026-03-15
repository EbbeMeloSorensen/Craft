namespace Craft.Logging
{
    public enum LogMessageCategory
    {
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }

    public interface ILogger
    {
        bool IsEnabled { get; set; }

        string WriteLineGoddammit(
            LogMessageCategory category,
            string message,
            string aspect = "general",
            bool startStopwatch = false);
    }
}
