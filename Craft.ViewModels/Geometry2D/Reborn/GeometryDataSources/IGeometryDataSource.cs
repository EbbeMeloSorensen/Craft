using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public interface IGeometryDataSource
{
    IEnumerable<LineModel> Query(BoundingBox window);
}