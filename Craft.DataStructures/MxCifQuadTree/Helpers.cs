using Craft.DataStructures.Geometry;

namespace Craft.DataStructures.MxCifQuadTree;

public static class Helpers
{
    public static readonly int[] g_XF = [-1, 1, -1, 1];
    public static readonly int[] g_YF = [-1, -1, 1, 1];
    public static readonly int[] g_VF = [-1, 1];

    public static DIRECTION BIN_COMPARE(
        this BoundingBox rectangle,
        double CV,
        AXIS V)
    {   
        if (V == AXIS.XA)
        {
            if (rectangle.MinX <= CV &&
                CV <= rectangle.MaxX)
            {   
                return DIRECTION.BOTH;
            }

            return CV > rectangle.CenterX ? DIRECTION.LEFT : DIRECTION.RIGHT;
        }

        if (rectangle.MinY <= CV &&
            CV <= rectangle.MaxY)
        {
            return DIRECTION.BOTH;
        }

        return CV > rectangle.CenterY ? DIRECTION.LEFT : DIRECTION.RIGHT;
    }

    public static QUADRANT CIF_COMPARE(
        this BoundingBox rectangle,
        double cx,
        double cy)
    {
        if (rectangle.CenterX < cx)
        {
            return rectangle.CenterY < cy ? QUADRANT.NW : QUADRANT.SW;
        }

        return rectangle.CenterY < cy ? QUADRANT.NE : QUADRANT.SE;
    }

    public static bool CROSS_AXIS<T>(
        this BoundingBox rectangle,
        BinNode<T> binNode,
        double cv,
        double lv,
        AXIS v)
    {
        // Is there a quadnode in the first place?
        if (binNode == null)
        {
            return false;
        }

        // Does the rectangle intersect any of the rectangles of the bin node
        if (binNode.Rectangles.Any(_ => _.Intersects(rectangle)))
        {
            return true;
        }

        lv /= 2;
        var d = rectangle.BIN_COMPARE(cv, v);

        if (d == DIRECTION.BOTH)
        {
            return rectangle.CROSS_AXIS(binNode.Child[0], cv - lv, lv, v) ||
                   rectangle.CROSS_AXIS(binNode.Child[1], cv + lv, lv, v);
        }

        return rectangle.CROSS_AXIS(binNode.Child[(int)d], cv + g_VF[(int)d] * lv, lv, v);
    }

    public static bool CIF_SEARCH<T>(
        this BoundingBox rectangle,
        QuadNode<T> quadNode,
        double cx,
        double cy,
        double lx,
        double ly)
    {
        // Is there a quadnode in the first place?
        if (quadNode == null)
        {
            return false;
        }

        // Is rectangle outside the rectangle of the very quadnode
        if (!rectangle.Intersects(new BoundingBox(cx - lx, cx + lx, cy - ly, cy + ly)))
        {
            return false;
        }

        // Does the rectangle intersect any rectangles in any of the two bintrees of the quadnode?
        //if (rectangle.CROSS_AXIS(quadNode._axis[1], cy, ly, AXIS.YA) ||
        //    rectangle.CROSS_AXIS(quadNode._axis[0], cx, lx, AXIS.XA))
        //{
        //    return true;
        //}

        if (rectangle.CROSS_AXIS(quadNode._axis[1], cy, ly, AXIS.YA))
        {
            return true;
        }

        if (rectangle.CROSS_AXIS(quadNode._axis[0], cx, lx, AXIS.XA))
        {
            return true;
        }

        lx /= 2;
        ly /= 2;

        //if (CIF_SEARCH(rectangle, quadNode._child[0], cx + g_XF[0] * lx, cy + g_YF[0] * ly, lx, ly) ||
        //    CIF_SEARCH(rectangle, quadNode._child[1], cx + g_XF[1] * lx, cy + g_YF[1] * ly, lx, ly) ||
        //    CIF_SEARCH(rectangle, quadNode._child[2], cx + g_XF[2] * lx, cy + g_YF[2] * ly, lx, ly) ||
        //    CIF_SEARCH(rectangle, quadNode._child[3], cx + g_XF[3] * lx, cy + g_YF[3] * ly, lx, ly))
        //{
        //    return true;
        //}

        if (CIF_SEARCH(rectangle, quadNode._child[0], cx + g_XF[0] * lx, cy + g_YF[0] * ly, lx, ly))
        {
            return true;
        }

        if (CIF_SEARCH(rectangle, quadNode._child[1], cx + g_XF[1] * lx, cy + g_YF[1] * ly, lx, ly))
        {
            return true;
        }

        if (CIF_SEARCH(rectangle, quadNode._child[2], cx + g_XF[2] * lx, cy + g_YF[2] * ly, lx, ly))
        {
            return true;
        }

        if (CIF_SEARCH(rectangle, quadNode._child[3], cx + g_XF[3] * lx, cy + g_YF[3] * ly, lx, ly))
        {
            return true;
        }

        return false;
    }

    public static IEnumerable<BoundingBox> CIF_SEARCH_ALL<T>(
        this BoundingBox rectangle,
        QuadNode<T> quadNode,
        double cx,
        double cy,
        double lx,
        double ly)
    {
        // Is there a quadnode in the first place?
        //if (quadNode == null)
        //{
        //    return Enumerable.Empty<Rectangle>();
        //}

        //// Is rectangle outside the rectangle of the very quadnode
        //if (!rectangle.Intersects(new Rectangle(cx, cy, lx, ly)))
        //{
        //    return Enumerable.Empty<Rectangle>();
        //}

        if (quadNode != null &&
            rectangle.Intersects(new BoundingBox(cx - lx, cx + lx, cy - ly, cy + ly)))
        {
            foreach (var rect in rectangle.CROSS_AXIS_ALL(quadNode._axis[1], cy, ly, AXIS.YA))
            {
                yield return rect;
            }

            foreach (var rect in rectangle.CROSS_AXIS_ALL(quadNode._axis[0], cx, lx, AXIS.XA))
            {
                yield return rect;
            }

            lx /= 2;
            ly /= 2;

            foreach (var rect in CIF_SEARCH_ALL(rectangle, quadNode._child[0], cx + g_XF[0] * lx, cy + g_YF[0] * ly, lx, ly))
            {
                yield return rect;
            }

            foreach (var rect in CIF_SEARCH_ALL(rectangle, quadNode._child[1], cx + g_XF[1] * lx, cy + g_YF[1] * ly, lx, ly))
            {
                yield return rect;
            }

            foreach (var rect in CIF_SEARCH_ALL(rectangle, quadNode._child[2], cx + g_XF[2] * lx, cy + g_YF[2] * ly, lx, ly))
            {
                yield return rect;
            }

            foreach (var rect in CIF_SEARCH_ALL(rectangle, quadNode._child[3], cx + g_XF[3] * lx, cy + g_YF[3] * ly, lx, ly))
            {
                yield return rect;
            }
        }

        //return rectangle.CROSS_AXIS_ALL(quadNode._axis[1], cy, ly, AXIS.YA);
        //return rectangle.CROSS_AXIS_ALL(quadNode._axis[0], cx, lx, AXIS.XA);

        //lx /= 2;
        //ly /= 2;

        /*
        if (CIF_SEARCH(rectangle, quadNode._child[0], cx + g_XF[0] * lx, cy + g_YF[0] * ly, lx, ly))
        {
            return true;
        }

        if (CIF_SEARCH(rectangle, quadNode._child[1], cx + g_XF[1] * lx, cy + g_YF[1] * ly, lx, ly))
        {
            return true;
        }

        if (CIF_SEARCH(rectangle, quadNode._child[2], cx + g_XF[2] * lx, cy + g_YF[2] * ly, lx, ly))
        {
            return true;
        }

        if (CIF_SEARCH(rectangle, quadNode._child[3], cx + g_XF[3] * lx, cy + g_YF[3] * ly, lx, ly))
        {
            return true;
        }
        */
    }

    public static IEnumerable<BoundingBox> CROSS_AXIS_ALL<T>(
        this BoundingBox rectangle,
        BinNode<T> binNode,
        double cv,
        double lv,
        AXIS v)
    {
        if (binNode != null)
        {
            foreach (var rect in binNode.Rectangles.Where(_ => _.Intersects(rectangle)))
            {
                yield return rect;
            }

            lv /= 2;
            var d = rectangle.BIN_COMPARE(cv, v);

            if (d == DIRECTION.BOTH)
            {
                foreach (var rect in rectangle.CROSS_AXIS_ALL(binNode.Child[0], cv - lv, lv, v))
                {
                    yield return rect;
                }

                foreach (var rect in rectangle.CROSS_AXIS_ALL(binNode.Child[1], cv + lv, lv, v))
                {
                    yield return rect;
                }
            }
            else
            {
                foreach (var rect in rectangle.CROSS_AXIS_ALL(binNode.Child[(int)d], cv + g_VF[(int)d] * lv, lv, v))
                {
                    yield return rect;
                }
            }
        }


        //return rectangle.CROSS_AXIS(binNode.Child[(int)d], cv + g_VF[(int)d] * lv, lv, v);


        //// Is there a quadnode in the first place?
        //if (binNode == null)
        //{
        //    return false;
        //}

        //// Does the rectangle intersect any of the rectangles of the bin node
        //if (binNode.Rectangles.Any(_ => _.Intersects(rectangle)))
        //{
        //    return true;
        //}

        //lv /= 2;
        //var d = rectangle.BIN_COMPARE(cv, v);

        //if (d == DIRECTION.BOTH)
        //{
        //    return rectangle.CROSS_AXIS(binNode.Child[0], cv - lv, lv, v) ||
        //           rectangle.CROSS_AXIS(binNode.Child[1], cv + lv, lv, v);
        //}

        //return rectangle.CROSS_AXIS(binNode.Child[(int)d], cv + g_VF[(int)d] * lv, lv, v);
    }

    public static QUADRANT OPQUAD(
        this QUADRANT quadrant)
    {
        return quadrant switch
        {
            QUADRANT.NW => QUADRANT.SE,
            QUADRANT.NE => QUADRANT.SW,
            QUADRANT.SW => QUADRANT.NE,
            QUADRANT.SE => QUADRANT.NW,
            _ => throw new ArgumentOutOfRangeException(nameof(quadrant), quadrant, null)
        };
    }

    public static QUADRANT CQUAD(
        this QUADRANT quadrant)
    {
        return quadrant switch
        {
            QUADRANT.NW => QUADRANT.NE,
            QUADRANT.NE => QUADRANT.SE,
            QUADRANT.SW => QUADRANT.NW,
            QUADRANT.SE => QUADRANT.SW,
            _ => throw new ArgumentOutOfRangeException(nameof(quadrant), quadrant, null)
        };
    }

    public static QUADRANT CCQUAD(
        this QUADRANT quadrant)
    {
        return quadrant switch
        {
            QUADRANT.NW => QUADRANT.SW,
            QUADRANT.NE => QUADRANT.NW,
            QUADRANT.SW => QUADRANT.SE,
            QUADRANT.SE => QUADRANT.NE,
            _ => throw new ArgumentOutOfRangeException(nameof(quadrant), quadrant, null)
        };
    }

    public static DIRECTION OPDIR(
        this DIRECTION direction)
    {
        return direction == DIRECTION.LEFT ? DIRECTION.RIGHT : DIRECTION.LEFT;
    }

    public static AXIS OTHERAXIS(
        this AXIS axis)
    {
        return axis == AXIS.XA ? AXIS.YA : AXIS.XA;
    }
}