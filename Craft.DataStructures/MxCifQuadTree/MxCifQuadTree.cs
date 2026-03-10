using Craft.Logging;

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
    private ILogger _logger;

    public MxCifQuadTree(
        Rectangle rectangle,
        ILogger logger = null)
    {
        _p = rectangle;
        _logger = logger;

        _logger?.WriteLine(LogMessageCategory.Information, "Instantiating CMxCifQuadTree");
    }

    public void Insert(
        Rectangle rectangle)
    {
        if (_root == null)
        {
            _root = new QuadNode(_logger);
        }

        var quadNode = _root;
        var cx = _p.CenterX;
        var cy = _p.CenterY;
        var lx = _p.HalfWidth;
        var ly = _p.HalfHeight;

        _logger?.WriteLine(LogMessageCategory.Information, $"Inserting rectangle: (Cx, Cy) = ({rectangle.CenterX}, {rectangle.CenterY}), (W, H) = ({2 * rectangle.HalfWidth}, {2 * rectangle.HalfHeight})");

        var dx = rectangle.BIN_COMPARE(cx, AXIS.XA);
        var dy = rectangle.BIN_COMPARE(cy, AXIS.YA);

        var quadNodeLevel = 1;

        while (dx != DIRECTION.BOTH && dy != DIRECTION.BOTH)
        {
            var q = rectangle.CIF_COMPARE(cx, cy);
            var index = (int)q;

            quadNode._child[index] ??= new QuadNode(_logger);
            quadNode = quadNode._child[index];
            lx /= 2;
            ly /= 2;
            cx += lx * g_XF[index];
            cy += ly * g_YF[index];
            dx = rectangle.BIN_COMPARE(cx, AXIS.XA);
            dy = rectangle.BIN_COMPARE(cy, AXIS.YA);

            _logger?.WriteLine(LogMessageCategory.Information, $"  No intersection at quad node level {quadNodeLevel} => Navigating to the {q}, where quad node is centered at (x, y) = ({cx}, {cy})");
            quadNodeLevel++;
        }

        if (dx == DIRECTION.BOTH)
        {
            _logger?.WriteLine(LogMessageCategory.Information, $"    Intersection with x axis on quad level {quadNodeLevel}");
            quadNode.InsertOnAxis(rectangle, cy, ly, AXIS.YA);
        }
        else
        {
            _logger?.WriteLine(LogMessageCategory.Information, $"    Intersection with y axis on quad level {quadNodeLevel}");
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

        var intersection = rectangle.CIF_SEARCH(_root, _p.CenterX, _p.CenterY, _p.HalfWidth, _p.HalfHeight);

        if (intersection)
        {
            _logger?.WriteLine(LogMessageCategory.Information, $"Rectangle: (Cx, Cy) = ({rectangle.CenterX}, {rectangle.CenterY}), (W, H) = ({rectangle.HalfWidth * 2}, {rectangle.HalfHeight * 2}) intersects existing rectangles and is therefore rejected");
        }

        return intersection;
    }

    public bool Clear()
    {
        throw new NotImplementedException();
    }
}