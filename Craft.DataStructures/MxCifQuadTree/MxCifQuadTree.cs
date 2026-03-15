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
    public static readonly int[] g_VF = [-1, 1];

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

        var CX = _p.CenterX;
        var CY = _p.CenterY;
        var LX = _p.HalfWidth;
        var LY = _p.HalfHeight;
        double CV;
        double LV;

        QuadNode T = _root;
        QuadNode FT = null;
        QuadNode TT = null;
        QuadNode TEMPC = null;

        BinNode B = null;
        BinNode FB = null;
        BinNode TB = null;
        BinNode TEMPB = null;

        AXIS V;
        QUADRANT Q;
        QUADRANT QF = QUADRANT.NW;
        DIRECTION D;
        DIRECTION DF = DIRECTION.LEFT;

        while (rectangle.BIN_COMPARE(CX, V = AXIS.XA) != DIRECTION.BOTH &&
               rectangle.BIN_COMPARE(CY, V = AXIS.YA) != DIRECTION.BOTH)
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
        B = T._axis[(int)V];
        FB = null;

        if (V == AXIS.XA)
        {
            CV = CX;
            LV = LX;
        }
        else
        {
            CV = CY;
            LV = LY;
        }

        D = rectangle.BIN_COMPARE(CV, V);

        while (B != null && D != DIRECTION.BOTH)
        {
            if (B.Child[(int)D.OPDIR()] != null || !B.Rectangles.Any())
            {
                FB = B;
                DF = D;
            }

            B = B.Child[(int)D];
            LV /= 2;
            CV += LV * g_VF[(int)D];
            D = rectangle.BIN_COMPARE(CV, V);
        }

        if (B == null)
        {
            return;
        }

        if (B.Child[0] != null || B.Child[1] != null)
        {
            // No collapsing is possible, so just remove the rectangle from the bin
            B.Rectangles.Remove(rectangle);
        }
        else
        {
            // Attempt to collapse bin nodes

            // Get a link to the oldest dismissable bin node
            TB = FB != null ? FB.Child[(int)DF] : T._axis[(int)V];

            // Initialize direction variable for scanning
            D = DIRECTION.LEFT;

            // Destroy BinNodes
            while (TB != B)
            {
                // Determine the direction to the BinNode child
                if (TB.Child[(int)D] == null)
                {
                    D = D.OPDIR();
                }

                TEMPB = TB.Child[(int)D];

                // Detach in order to avoid premature destruction of children
                TB.Child[(int)D] = null;
                TB = TEMPB;
            }

            if (FB != null)
            {
                // Set pointer to oldest destroyed BinNode to NULL
                FB.Child[(int)DF] = null;
            }
            else
            {
                T._axis[(int)V] = null;

                if (T._axis[(int)V.OTHERAXIS()] != null ||
                    T._child[0] != null ||
                    T._child[1] != null ||
                    T._child[2] != null ||
                    T._child[3] != null)
                {
                    return;
                }

                // Attempt to collapse quad nodes

                // Get a link to the oldest dismissable QuadNode
                TT = FT != null ? FT._child[(int)QF] : _root;

                // Initialize quadrant variable for scanning
                Q = QUADRANT.NW;

                // Destroy QuadNodes
                while (TT != T)
                {
                    // Determine the direction to the QuadNode child
                    while (TT._child[(int)Q] == null)
                    {
                        Q = Q.CCQUAD();
                    }

                    // Get a link to the QuadNode child for the next iteration
                    TEMPC = TT._child[(int)Q];

                    // Detach in order to avoid premature destruction of children
                    TT._child[(int)Q] = null;

                    // Proceed to the QuadNode child
                    TT = TEMPC;
                }

                // Set pointer to oldest destroyed QuadNode to NULL
                if (FT != null)
                {
                    FT._child[(int)QF] = null;
                }
                else
                {
                    _root = null;
                }
            }
        }

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

    public IEnumerable<Rectangle> GetAllIntersecting(
        Rectangle rectangle)
    {
        return _root == null 
            ? Enumerable.Empty<Rectangle>() 
            : rectangle.CIF_SEARCH_ALL(_root, _p.CenterX, _p.CenterY, _p.HalfWidth, _p.HalfHeight);
    }

    public bool Clear()
    {
        throw new NotImplementedException();
    }
}