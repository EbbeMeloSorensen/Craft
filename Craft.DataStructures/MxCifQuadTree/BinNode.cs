using Craft.DataStructures.Geometry;

namespace Craft.DataStructures.MxCifQuadTree;

public class BinNode<T>
{
    public BinNode<T>[] Child;

    public List<BoundingBox> Rectangles { get; }

    public BinNode()
    {
        Child = new BinNode<T>[2];
        Rectangles = new List<BoundingBox>();
    }

    public void Insert(
        SpatialItem<T> spatialItem)
    {
        var rectangle = spatialItem.Bounds;
        Rectangles.Add(rectangle);
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