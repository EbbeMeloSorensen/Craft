using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Craft.Utils
{
    public static class StringExtensions
    {
        // yyyy: 19 or 20 followed by 2 digits
        // MM: (0 followed by 1 to 9) or (1 followed by 0 to 2), i.e. numbers from 1 to 12 prefixed with zero in case of 1 digit
        // dd: (0 followed by 1 to 9) or (1 or 2 followed by digit) or (3 followed by 0 to 1), i.e. numbers from 1 to 31 prefixed with zero in case of 1 digit
        // HH: (0 or 1 followed by a digit) or (2 followed by 0 to 3), i.e. numbers from 0 to 23
        // mm: (0 to 5 followed by a digit), i.e. numbers from 0 to 59
        // ss: (0 to 5 followed by a digit), i.e. numbers from 0 to 59
        // fff: 3 digits

        // Format: yyyy-MM-dd
        private static Regex regexDateWithoutTime = new Regex(@"^(19|20)\d\d\-(0[1-9]|1[0-2])\-(0[1-9]|(1|2)\d|3[0-1])$");

        // Format: yyyy-MM-dd HH:mm:ss
        private static Regex regexDateWithTimeExcludingMilliseconds = new Regex(@"^(19|20)\d\d\-(0[1-9]|1[0-2])\-(0[1-9]|(1|2)\d|3[0-1])\ ([0-1]\d|2[0-3])\:([0-5]\d)\:([0-5]\d)$");

        // Format: yyyy-MM-dd HH:mm:ss.fff
        private static Regex regexDateWithTimeIncludingMilliseconds = new Regex(@"^(19|20)\d\d\-(0[1-9]|1[0-2])\-(0[1-9]|(1|2)\d|3[0-1])\ ([0-1]\d|2[0-3])\:([0-5]\d)\:([0-5]\d)\.\d\d\d$");

        public static string NullifyIfEmpty(
            this string s)
        {
            if (s != null && s.Trim().Equals(string.Empty))
            {
                return null;
            }

            return s;
        }

        public static bool IsProperlyFormattedAsADate(
            this string s)
        {
            return regexDateWithoutTime.IsMatch(s);
        }

        public static bool IsProperlyFormattedAsADateTime(
            this string s)
        {
            return regexDateWithoutTime.IsMatch(s) ||
                regexDateWithTimeExcludingMilliseconds.IsMatch(s) ||
                regexDateWithTimeIncludingMilliseconds.IsMatch(s);
        }

        public static bool TryParsingAsDateTime(
            this string s,
            out DateTime? dateTime)
        {
            var success = 
                DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var temp) ||
                DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out temp) ||
                DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out temp);

            dateTime = success
                ? new DateTime(temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, temp.Second,
                    temp.Millisecond, DateTimeKind.Utc)
                : new DateTime?();

            return success;
        }
    }
}
