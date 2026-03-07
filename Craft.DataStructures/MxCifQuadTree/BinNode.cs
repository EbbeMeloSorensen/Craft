namespace Craft.DataStructures.MxCifQuadTree;

public class BinNode
{
    public BinNode[] Child;

    public List<Rectangle> Rectangles { get; }

    public BinNode()
    {
        Child = new BinNode[2];
        Rectangles = new List<Rectangle>();
    }

    public void Insert(
        Rectangle rectangle)
    {
        Rectangles.Add(rectangle);
    }

    public void Holds(
        Rectangle rectangle)
    {
        throw new NotImplementedException();
    }

    public void Remove(
        Rectangle rectangle)
    {
        throw new NotImplementedException();
    }

    public int GetSize()
    {
        throw new NotImplementedException();
    }
}