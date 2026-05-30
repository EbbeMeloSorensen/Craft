using System.Collections;
using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class EmptyDataSource : IGeometryDataSource
{
    public IEnumerable Query(
        BoundingBox window)
    {
        return Enumerable.Empty<object>();
    }
}