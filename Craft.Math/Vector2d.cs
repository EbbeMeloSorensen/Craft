using System;

namespace Craft.Math
{
    public class Vector2D
    {
        private const double _toleranceForComparingCoordinateWithZero = 0.0000000001;

        public double X { get; }
        public double Y { get; }

        public Vector2D(
            double x, 
            double y)
        {
            X = x;
            Y = y;
        }

        public double Length
        {
            get
            {
                if (System.Math.Abs(X) < _toleranceForComparingCoordinateWithZero &&
                    System.Math.Abs(Y) < _toleranceForComparingCoordinateWithZero)
                {
                    return 0;
                }

                return System.Math.Sqrt(X * X + Y * Y);
            }
        }

        public double SqrLength
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(-v.X, -v.Y);
        }

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2D operator *(Vector2D v, double factor)
        {
            return new Vector2D(v.X * factor, v.Y * factor);
        }

        public static Vector2D operator *(double factor, Vector2D v)
        {
            return v * factor;
        }

        public static Vector2D operator /(Vector2D v, double divisor)
        {
            return new Vector2D(v.X / divisor, v.Y / divisor);
        }

        public static double DotProduct(Vector2D a, Vector2D b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public Vector2D Hat()
        {
            return new Vector2D(-Y, X);
        }

        public Vector2D Rotate(double angle)
        {
            var cosAngle = System.Math.Cos(angle);
            var sinAngle = System.Math.Sin(angle);

            return new Vector2D(
                cosAngle * X - sinAngle * Y,
                sinAngle * X + cosAngle * Y);
        }

        public Vector2D Normalize()
        {
            var length = Length;

            if (System.Math.Abs(length) < _toleranceForComparingCoordinateWithZero)
            {
                throw new InvalidOperationException("Cannot normalize a null vector");
            }

            return new Vector2D(X / length, Y / length);
        }

        public Vector3D AsVector3D()
        {
            return new Vector3D(X, Y, 0);
        }

        public Point2D AsPoint2D()
        {
            return new Point2D(X, Y);
        }

        public PolarVector AsPolarVector()
        {
            var length = Length;

            if (length < _toleranceForComparingCoordinateWithZero)
            {
                return new PolarVector(0, 0);
            }

            return new PolarVector(
                length,
                System.Math.Atan2(Y, X));
        }
    }
}
