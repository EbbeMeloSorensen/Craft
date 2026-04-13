using Craft.DataStructures.Geometry;

namespace Craft.DataStructures.MxCifQuadTree;

public class BinNode
{
    public BinNode[] Child;

    public List<BoundingBox> Rectangles { get; }

    public BinNode()
    {
        Child = new BinNode[2];
        Rectangles = new List<BoundingBox>();
    }

    public void Insert(
        BoundingBox rectangle)
    {
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