using System;

namespace Craft.Utils
{
    public static class LongExtensions
    {
        public static DateTime ConvertFromEpochInMicrosecondsToDateTime(
            this long epochInMicroseconds)
        {
            return new DateTime(1970, 1, 1).Add(new TimeSpan(epochInMicroseconds * 10));
        }
    }
}
