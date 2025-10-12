using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Craft.UI.Utils.ValueConverters
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null || !(value is bool))
            {
                throw new ArgumentException("value should be of type bool");
            }

            var valueAsBool = (bool)value;

            return valueAsBool
                ? new SolidColorBrush(Colors.DarkRed)
                : new SolidColorBrush(Colors.DodgerBlue);
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
