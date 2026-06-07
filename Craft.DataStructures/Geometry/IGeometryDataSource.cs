using System.Collections;

namespace Craft.DataStructures.Geometry;

public interface IGeometryDataSource
{
    IEnumerable Query(
        BoundingBox window);
}