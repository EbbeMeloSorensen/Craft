using System.Globalization;
using System.Windows.Data;

namespace Craft.UI.Utils.ValueConverters
{
    // Used in the 3D GeometryEditorView for positioning of Points
    public class WorldToCanvasCoordinateConverter : IMultiValueConverter
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
            var extent = (double)values[1];

            return coordinate - extent / 2;
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
