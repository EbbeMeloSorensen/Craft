using System.Collections;

namespace Craft.DataStructures.Geometry;

public interface IGeometryDataSource
{
    IEnumerable GetAll();

    IEnumerable GetIntersecting(
        BoundingBox window);
}