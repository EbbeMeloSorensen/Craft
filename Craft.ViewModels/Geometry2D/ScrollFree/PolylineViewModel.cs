using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree;

public class PolylineViewModel : ViewModelBase
{
    private IEnumerable<PointD> _points;
    private string _points2;

    public string Points { get; }

    public string Points2
    {
        get => _points2;
        private set
        {
            _points2 = value;
            RaisePropertyChanged();
        }
    }

    public double Thickness { get; }
    public Brush Brush { get; }

    public PolylineViewModel(
        IEnumerable<PointD> points,
        double thickness,
        Brush brush)
    {
        _points = points;
        Thickness = thickness;
        Brush = brush;

        Points = string.Join(" ", points.Select(
            point => $"{string.Format(CultureInfo.InvariantCulture, "{0}", point.X)},{string.Format(CultureInfo.InvariantCulture, "{0}", point.Y)}"));
    }

    public void Update(
        Size scaling,
        Point worldWindowUpperLeft)
    {
        Points2 = string.Join(" ", _points
            .Select(point => $"{string.Format(CultureInfo.InvariantCulture, "{0}", (point.X - worldWindowUpperLeft.X) * scaling.Width)},{string.Format(CultureInfo.InvariantCulture, "{0}", (point.Y - worldWindowUpperLeft.Y) * scaling.Height)}"));
    }
}