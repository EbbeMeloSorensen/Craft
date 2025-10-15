using System;
using System.Globalization;
using System.Windows.Data;

namespace Craft.UI.Utils.ValueConverters
{
    public class StrokeThicknessConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var thickness = (double)values[0];
            var scaleX = (double)values[1];
            var scaleY = (double)values[2];

            var result = thickness / Math.Max(scaleX, scaleY);
            return Math.Max(result, 0.01);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}