using Craft.Logging;

namespace Craft.DataStructures.UnitTest
{
    public class TestLogger : ILogger
    {
        private StreamWriter _streamWriter;

        public TestLogger()
        {
            _streamWriter = new StreamWriter(@"C:\Temp\CS_Log.txt");
        }

        public string WriteLine(
            LogMessageCategory category,
            string message,
            string aspect = "general",
            bool startStopwatch = false)
        {
            _streamWriter.WriteLine(message);

            return message;
        }

        public void Complete()
        {
            _streamWriter.Dispose();
        }
    }
}
