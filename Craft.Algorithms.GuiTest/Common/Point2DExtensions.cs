using Craft.Math;
using Craft.Utils;

namespace Craft.Algorithms.GuiTest.Common
{
    public static class Point2DExtensions
    {
        public static PointD AsPointD(this Point2D point)
        {
            return new PointD(point.X, point.Y);
        }
    }
}
