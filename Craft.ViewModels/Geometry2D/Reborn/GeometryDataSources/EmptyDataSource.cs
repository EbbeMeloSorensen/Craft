using System.Collections;
using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class EmptyDataSource : IGeometryDataSource
{
    public IEnumerable GetAll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable GetIntersecting(
        BoundingBox window)
    {
        return Enumerable.Empty<object>();
    }
}