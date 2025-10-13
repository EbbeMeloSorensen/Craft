using Craft.DataStructures.Graph;
using Craft.Utils;

namespace Craft.Algorithms.GuiTest.Common;

public static class Point2DVertexExtensions
{
    public static PointD AsPointD(this Point2DVertex vertex)
    {
        return new PointD(vertex.X, vertex.Y);
    }
}