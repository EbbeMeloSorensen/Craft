using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.Logging;

namespace Craft.DataStructures.MxCifQuadTree;

public class DataStore : IGeometryDataStore
{
    private MxCifQuadTree<object> _mxCifQuadTree;

    public DataStore(
        BoundingBox region,
        int maxDepth = 8)
    {
        _mxCifQuadTree = new MxCifQuadTree<object>(region, maxDepth, new DummyLogger());
    }

    public IEnumerable Query(
        BoundingBox window)
    {
        var result = _mxCifQuadTree.GetAllIntersecting(window);
        return result.Select(_ => _.Item);
    }

    public void AddGeometricObject(
        object geometricObject,
        BoundingBox boundingBox)
    {
        _mxCifQuadTree.Insert(new SpatialItem<object>(boundingBox, geometricObject));
    }
}

