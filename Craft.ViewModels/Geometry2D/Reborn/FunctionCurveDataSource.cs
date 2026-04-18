using Craft.DataStructures.Geometry;
using Craft.Utils.Linq;

namespace Craft.ViewModels.Geometry2D.Reborn;

public class FunctionCurveDataSource : IGeometryDataSource
{
    public IEnumerable<LineModel> Query(
        BoundingBox window)
    {
        var points = new List<System.Windows.Point>();

        for (var x = window.MinX; x <= window.MaxX; x += 0.5)
        {
            var point = new System.Windows.Point(x, 100 * System.Math.Sin(0.05 * x) + 200);
            points.Add(point);
        }

        return points.AdjacentPairs()
            .Select(_ => new LineModel
            {
                P1 = _.Item1,
                P2 = _.Item2
            });
    }
}