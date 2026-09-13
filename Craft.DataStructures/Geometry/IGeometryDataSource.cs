using System.Collections;

namespace Craft.DataStructures.Geometry;

public interface IGeometryDataSource
{
    IEnumerable GetGeometries(
        BoundingBox window);
}