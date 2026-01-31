using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Craft.UI.Utils.ValueConverters
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            try
            {
                string myPath = (string)value;
                Uri myURI = new Uri(myPath, UriKind.RelativeOrAbsolute);
                BitmapImage anImage = new BitmapImage(myURI);
                return anImage;
            }
            catch (Exception)
            {
                return new BitmapImage(new Uri("/Images/NoPreview.png", UriKind.RelativeOrAbsolute));
            }
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