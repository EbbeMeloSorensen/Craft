using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class SimpleGeometryDataSource : IGeometryDataSource
{
    public IEnumerable Query(BoundingBox window)
    {
        yield return new CircleModel
        {
            Center = new System.Windows.Point(200, 75),
            Radius = 25
        };

        yield return new LineModel
        {
            P1 = new System.Windows.Point(0, 0),
            P2 = new System.Windows.Point(0, 200)
        };

        yield return new LineModel
        {
            P1 = new System.Windows.Point(0, 200),
            P2 = new System.Windows.Point(200, 300)
        };

        yield return new LineModel
        {
            P1 = new System.Windows.Point(200, 300),
            P2 = new System.Windows.Point(400, 200)
        };

        yield return new LineModel
        {
            P1 = new System.Windows.Point(400, 200),
            P2 = new System.Windows.Point(400, 0)
        };

        yield return new LineModel
        {
            P1 = new System.Windows.Point(400, 0),
            P2 = new System.Windows.Point(0, 0)
        };

        yield return new PointModel
        {
            P = new System.Windows.Point(200, 150)
        };
    }
}