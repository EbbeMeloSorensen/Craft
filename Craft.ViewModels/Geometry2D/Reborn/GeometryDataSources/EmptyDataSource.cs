using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class EmptyDataSource : IGeometryDataSource
{
    public IEnumerable<LineModel> Query(BoundingBox window)
    {
        return new List<LineModel>();
    }
}