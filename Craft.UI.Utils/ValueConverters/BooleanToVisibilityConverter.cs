using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Craft.UI.Utils.ValueConverters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null || !(value is bool))
            {
                return Visibility.Hidden;
            }

            var valueAsBool = (bool)value;

            return valueAsBool
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}