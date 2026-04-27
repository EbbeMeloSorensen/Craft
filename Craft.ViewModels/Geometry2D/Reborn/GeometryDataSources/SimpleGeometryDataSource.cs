using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class SimpleGeometryDataSource : IGeometryDataSource
{
    public IEnumerable<LineModel> Query(BoundingBox window)
    {
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
    }
}