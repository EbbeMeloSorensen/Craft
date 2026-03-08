namespace Craft.DataStructures.MxCifQuadTree;

public enum QUADRANT
{
    NW,
    NE,
    SW,
    SE
}

public enum DIRECTION
{
    LEFT,
    RIGHT,
    BOTH
}

public enum AXIS
{
    XA,
    YA
}

public class MxCifQuadTree
{
    public static readonly int[] g_XF = [-1, 1, -1, 1];
    public static readonly int[] g_YF = [-1, -1, 1, 1];

    private Rectangle _p;
    private QuadNode _root;

    public MxCifQuadTree(
        Rectangle rectangle)
    {
        _p = rectangle;
    }

    public void Insert(
        Rectangle rectangle)
    {
        if (_root == null)
        {
            _root = new QuadNode();
        }

        var quadNode = _root;
        var cx = _p.CenterX;
        var cy = _p.CenterY;
        var lx = _p.HalfWidth;
        var ly = _p.HalfHeight;

        var dx = rectangle.BIN_COMPARE(cx, AXIS.XA);
        var dy = rectangle.BIN_COMPARE(cy, AXIS.YA);

        while (dx != DIRECTION.BOTH && dy != DIRECTION.BOTH)
        {
            var q = (int)rectangle.CIF_COMPARE(cx, cy);

            quadNode._child[q] ??= new QuadNode();
            quadNode = quadNode._child[q];
            lx /= 2;
            ly /= 2;
            cx += lx * g_XF[q];
            cy += ly * g_YF[q];
            dx = rectangle.BIN_COMPARE(cx, AXIS.XA);
            dy = rectangle.BIN_COMPARE(cy, AXIS.YA);
        }

        if (dx == DIRECTION.BOTH)
        {
            quadNode.InsertOnAxis(rectangle, cy, ly, AXIS.YA);
        }
        else
        {
            quadNode.InsertOnAxis(rectangle, cx, lx, AXIS.XA);
        }
    }

    public void Remove(
        Rectangle rectangle)
    {
        if (_root == null)
        {
            return;
        }

        var T = _root;
        QuadNode FT = null;

        var CX = _p.CenterX;
        var CY = _p.CenterY;
        var LX = _p.HalfWidth;
        var LY = _p.HalfHeight;
        AXIS V = AXIS.XA;
        QUADRANT Q, QF;
        DIRECTION D, DF;

        while (rectangle.BIN_COMPARE(CX, AXIS.XA) != DIRECTION.BOTH &&
               rectangle.BIN_COMPARE(CY, AXIS.YA) != DIRECTION.BOTH)
        {
            Q = rectangle.CIF_COMPARE(CX, CY);

            if (T._child[(int)Q] == null)
            {
                // The rectangle is not int the tree
                return;
            }

            if (T._axis[0] != null ||
                T._axis[1] != null ||
                T._child[(int)Q.OPQUAD()] != null ||
                T._child[(int)Q.CQUAD()] != null ||
                T._child[(int)Q.CCQUAD()] != null)
            {
                FT = T;
                QF = Q;
            }

            T = T._child[(int)Q];
            LX /= 2;
            LY /= 2;
            CX += LX * g_XF[(int)Q];
            CY += LY * g_YF[(int)Q];
        }

        V = V.OTHERAXIS();

        throw new NotImplementedException("Det er en større sag, det her. I øvrigt er det lidt af en edge use case");
    }

    public bool Intersects(
        Rectangle rectangle)
    {
        if (_root == null)
        {
            return false;
        }

        return rectangle.CIF_SEARCH(_root, _p.CenterX, _p.CenterY, _p.HalfWidth, _p.HalfHeight);
    }

    public bool Clear()
    {
        throw new NotImplementedException();
    }
}