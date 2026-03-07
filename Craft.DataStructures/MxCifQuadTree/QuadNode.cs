namespace Craft.DataStructures.MxCifQuadTree;

public class QuadNode
{
    public static readonly int[] g_VF = [-1, 1];

    public BinNode[] _axis;
    public QuadNode[] _child;

    public QuadNode()
    {
        _axis = new BinNode[2];
        _child = new QuadNode[4];
    }

    public void InsertOnAxis(
        Rectangle rectangle,
        double cv,
        double lv,
        AXIS v)
    {
        _axis[(int)v] ??= new BinNode();

        var binNode = _axis[(int)v];
        var d = rectangle.BIN_COMPARE(cv, v);

        while (d != DIRECTION.BOTH)
        {
            binNode.Child[(int)d] ??= new BinNode();

            binNode = binNode.Child[(int)d];
            lv /= 2;
            cv += lv * g_VF[(int)d];
            d = rectangle.BIN_COMPARE(cv, v);
        }

        binNode.Insert(rectangle);
    }
}