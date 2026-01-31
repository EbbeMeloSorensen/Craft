using System.Globalization;
using System.Windows.Data;

namespace Craft.UI.Utils.ValueConverters
{
    // Used in GeometryEditorView for positioning Labels
    public class LabelPositionConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            // Hvis den her ikke er der, kan det åbenbart gå i kuk, hvor den prøver at caste et "unset" objekt
            if (!(values[1] is double))
            {
                return 0;
            }

            var coordinate = (double)values[0];
            var upperLeft = (double)values[1];
            var scale = (double)values[2];
            var extent = (double)values[3];
            var shift = (double)values[4];
            var fixedViewPortCoordinate = (double?)values[5];

            if (fixedViewPortCoordinate.HasValue)
            {
                return fixedViewPortCoordinate.Value;
            }

            return (coordinate - upperLeft) * scale - extent / 2 + shift;
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
