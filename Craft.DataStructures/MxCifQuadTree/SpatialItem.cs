using Craft.DataStructures.Geometry;

namespace Craft.DataStructures.MxCifQuadTree;

public class SpatialItem<T>
{
    public BoundingBox Bounds { get; }
    public T Item { get; }

    public SpatialItem(
        BoundingBox bounds,
        T item)
    {
        Bounds = bounds;
        Item = item;
    }
}

