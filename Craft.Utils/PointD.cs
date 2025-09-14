using System.Globalization;

namespace Craft.Utils
{
    public class PointD
    {
        public static PointD operator +(PointD a, PointD b) => new PointD { X = a.X + b.X, Y = a.Y + b.Y };
        public static PointD operator -(PointD a, PointD b) => new PointD { X = a.X - b.X, Y = a.Y - b.Y };
        public static PointD operator *(PointD a, double factor) => new PointD { X = a.X * factor, Y = a.Y * factor };
        public static PointD operator /(PointD a, double divisor) => new PointD { X = a.X / divisor, Y = a.Y / divisor };

        public double X { get; set; }
        public double Y { get; set; }

        public PointD()
        {
        }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X.ToString(CultureInfo.InvariantCulture)}, {Y.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}
