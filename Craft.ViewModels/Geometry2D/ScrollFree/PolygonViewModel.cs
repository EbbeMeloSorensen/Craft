using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class PolygonViewModel : ViewModelBase
    {
        private Brush _brush;

        public string Points { get; }

        public double Thickness { get; }

        public Brush Brush
        {
            get => _brush;
            set
            {
                _brush = value;
                RaisePropertyChanged();
            }
        }

        public PolygonViewModel(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            Points = string.Join(" ", points
                .Select( point => $"{string.Format(CultureInfo.InvariantCulture, "{0}", point.X)},{string.Format(CultureInfo.InvariantCulture, "{0}", point.Y)}"));

            Thickness = thickness;
            Brush = brush;
        }
    }
}
