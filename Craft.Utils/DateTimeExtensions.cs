using System;

namespace Craft.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime TruncateToMilliseconds(
            this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
        }

        public static long AsEpochInMicroSeconds(
            this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;
        }

        // Todo: Se om det giver mening at lave een metode ved navn AsDateTimeString i stedet for de 2, der er her
        public static string AsDateTimeString(
            this DateTime dateTime,
            bool includeMilliseconds)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            var result = $"{year}-{month}-{day} {hour}:{minute}:{second}";

            if (includeMilliseconds)
            {
                var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

                result += $".{millisecond}";
            }

            return result;
        }

        // Todo: Se om det giver mening at lave een metode ved navn AsDateTimeString i stedet for de 2, der er her
        public static string AsDateTimeString(
            this DateTime dateTime,
            bool includeMilliseconds,
            bool omitTimePartIfZero)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');
            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');
            var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

            var result = $"{year}-{month}-{day}";

            if (omitTimePartIfZero && hour == "00" && minute == "00" && second == "00" && millisecond == "000")
            {
                return result;
            }

            result += $" {hour}:{minute}:{second}";

            if (omitTimePartIfZero && millisecond == "000")
            {
                return result;
            }

            if (includeMilliseconds)
            {
                result += $".{millisecond}";
            }

            return result;
        }

        public static string AsDateString(
            this DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day}";
        }

        public static string AsRFC3339(
            this DateTime dateTime,
            bool includeMilliseconds)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            var result = $"{year}-{month}-{day}T{hour}:{minute}:{second}";

            if (includeMilliseconds)
            {
                var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

                result += $".{millisecond}";
            }

            result += "Z";

            return result;
        }

        public static string AsRFC3339(
            this long epochInMicroseconds)
        {
            var dateTime = epochInMicroseconds.ConvertFromEpochInMicrosecondsToDateTime();

            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day}T{hour}:{minute}:{second}Z";
        }
    }
}
