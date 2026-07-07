using System.Collections;
using Craft.Utils.Linq;
using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class FunctionCurveDataSource : IGeometryDataSource
{
    public IEnumerable Query(
        BoundingBox window)
    {
        var points = new List<Craft.Math.Point2D>();

        for (var x = window.MinX; x <= window.MaxX; x += 2)
        {
            var point = new Craft.Math.Point2D(x, 100 * System.Math.Sin(0.1 * x) * System.Math.Sin(0.005 * x) + 150);
            points.Add(point);
        }

        return points.AdjacentPairs()
            .Select(_ => new Math.LineSegment2D(_.Item1, _.Item2));
    }
}