using Craft.DataStructures.Geometry;

namespace Craft.DataStructures.MxCifQuadTree;

public class BinNode<T>
{
    public BinNode<T>[] Child;

    public List<SpatialItem<T>> SpatialItems { get; }

    public BinNode()
    {
        Child = new BinNode<T>[2];
        SpatialItems = new List<SpatialItem<T>>();
    }

    public void Insert(
        SpatialItem<T> spatialItem)
    {
        SpatialItems.Add(spatialItem);
    }

    public void Holds(
        BoundingBox rectangle)
    {
        throw new NotImplementedException();
    }

    public void Remove(
        BoundingBox rectangle)
    {
        throw new NotImplementedException();
    }

    public int GetSize()
    {
        throw new NotImplementedException();
    }
}