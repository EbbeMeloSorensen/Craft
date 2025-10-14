using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree;

public class PolylineViewModel : ViewModelBase
{
    private IEnumerable<PointD> _pointsInWorldCoordinates;
    private PointCollection _pointsInViewportCoordinates;

    public PointCollection PointsInViewportCoordinates
    {
        get => _pointsInViewportCoordinates;
        private set
        {
            _pointsInViewportCoordinates = value;
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
        _pointsInWorldCoordinates = points;
        Thickness = thickness;
        Brush = brush;
    }

    public void UpdateViewportCoordinates(
        Size scaling,
        Point worldWindowUpperLeft)
    {
        PointsInViewportCoordinates = new PointCollection(_pointsInWorldCoordinates.Select(_ => new Point(
            (_.X - worldWindowUpperLeft.X) * scaling.Width,
            (_.Y - worldWindowUpperLeft.Y) * scaling.Height)));
    }
}