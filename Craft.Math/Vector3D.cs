namespace Craft.Math
{
    public class Vector3D
    {
        private const double _toleranceForComparingCoordinateWithZero = 0.0000000001;

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Vector3D()
        {
        }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length
        {
            get
            {
                if (System.Math.Abs(X) < _toleranceForComparingCoordinateWithZero &&
                    System.Math.Abs(Y) < _toleranceForComparingCoordinateWithZero &&
                    System.Math.Abs(Z) < _toleranceForComparingCoordinateWithZero)
                {
                    return 0;
                }

                return System.Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public Vector3D Normalize()
        {
            var length = Length;

            return new Vector3D(X / length, Y / length, Z / length);
        }

        public Vector AsVector()
        {
            return new Vector(3) {[0] = X, [1] = Y, [2] = Z};
        }

        public SphericalVector AsSphericalVector()
        {
            var radialDistance = Length;

            if (radialDistance < _toleranceForComparingCoordinateWithZero)
            {
                return new SphericalVector(radialDistance, 0, 0);
            }

            return new SphericalVector(
                radialDistance,
                System.Math.Acos(Z / radialDistance),
                System.Math.Atan2(Y, X));
        }

        public override string ToString()
        {
            return $"({X,-10:F3}, {Y,-10:F3}, {Z,-10:F3})";
        }

        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(-v.X, -v.Y, -v.Z);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3D operator *(Vector3D v, double factor)
        {
            return new Vector3D(v.X * factor, v.Y * factor, v.Z * factor);
        }

        public static Vector3D operator *(double factor, Vector3D v)
        {
            return v * factor;
        }

        public static Vector3D operator /(Vector3D v, double divisor)
        {
            return new Vector3D(v.X / divisor, v.Y / divisor, v.Z / divisor);
        }

        public static Vector3D CrossProduct(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X);
        }
    }
}