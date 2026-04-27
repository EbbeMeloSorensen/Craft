using Craft.DataStructures.Geometry;
using Craft.Utils.Linq;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class FunctionCurveDataSource : IGeometryDataSource
{
    public IEnumerable<LineModel> Query(
        BoundingBox window)
    {
        var points = new List<System.Windows.Point>();

        for (var x = window.MinX; x <= window.MaxX; x += 2)
        {

            var point = new System.Windows.Point(x, 100 * System.Math.Sin(0.1 * x) * System.Math.Sin(0.005 * x) + 150);
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