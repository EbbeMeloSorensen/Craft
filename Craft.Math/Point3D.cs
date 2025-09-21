namespace Craft.Math
{
    public class Point3D
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Point3D(
            double x, 
            double y, 
            double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(Vector v)
        {
            X = v[0];
            Y = v[1];
            Z = v[2];
        }

        public Vector AsVectorInHomogeneousCoordinates()
        {
            var vector = new Vector(4);
            vector[0] = X;
            vector[1] = Y;
            vector[2] = Z;
            vector[3] = 1;

            return vector;
        }

        public static Point3D operator +(Point3D a, Point3D b)
        {
            return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point3D operator *(Point3D p, double factor)
        {
            return new Point3D(p.X * factor, p.Y * factor, p.Z * factor);
        }

        public static Point3D operator *(double factor, Point3D p)
        {
            return p * factor;
        }
    }
}