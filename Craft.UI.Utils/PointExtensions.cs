using System.Windows;
using Craft.Utils;

namespace Craft.UI.Utils
{
    public static class PointExtensions
    {
        public static PointD AsPointD(this Point point)
        {
            return new PointD(point.X, point.Y);
        }
    }
}