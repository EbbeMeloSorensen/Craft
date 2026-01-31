using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Craft.UI.Utils.ValueConverters
{
    public class NullableDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;

            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (double.TryParse(input, out double result))
                return result;

            return DependencyProperty.UnsetValue; // triggers validation error
        }
    }
}
