namespace Craft.Math
{
    public class Vector
    {
        private double[] _elements;

        public Vector(int size = 0)
        {
            _elements = new double[size];
        }

        public Vector(IEnumerable<double> elements)
        {
            _elements = elements.ToArray();
        }

        public double this[int index]
        {
            get { return _elements[index]; }
            set { _elements[index] = value; }
        }

        public int Size
        {
            get { return _elements.Length; }
        }

        public double TwoNorm()
        {
            return System.Math.Sqrt(_elements.Select(e => e * e).Sum());
        }

        public override string ToString()
        {
            return "(" + _elements.Select(e => $"{e:F3}").Aggregate((c, n) => $"{c}, {n}") + ")";
        }

        public Vector3D AsVector3D()
        {
            if (Size != 3)
            {
                throw new ArgumentException();
            }

            return new Vector3D(_elements[0], _elements[1], _elements[2]);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Size != b.Size)
            {
                throw new ArgumentException("Vectors must be of equal size for addition");
            }

            return new Vector(a._elements.Zip(b._elements, (p, q) => p + q));
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Size != b.Size)
            {
                throw new ArgumentException("Vectors must be of equal size for subtraction");
            }

            return new Vector(a._elements.Zip(b._elements, (p, q) => p - q));
        }

        public static double DotProduct(Vector a, Vector b)
        {
            if (a.Size != b.Size)
            {
                throw new ArgumentException("Vectors must be of equal size for dot product");
            }

            return a._elements.Zip(b._elements, (x, y) => x * y).Sum();
        }

        public static Vector operator *(double factor, Vector v)
        {
            return new Vector(v._elements.Select(e => factor * e));
        }

        public static Vector operator *(Vector v, double factor)
        {
            return factor * v;
        }

        public static Vector operator /(Vector v, double divisor)
        {
            return new Vector(v._elements.Select(e => e / divisor));
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            if (m.Columns != v.Size)
            {
                throw new ArgumentException("Inner Matrix and Vector dimensions must agree for multiplication");
            }

            var result = new Vector(m.Rows);

            for (var r = 0; r < result.Size; r++)
            {
                for (var i = 0; i < m.Columns; i++)
                {
                    result[r] += m[r, i] * v[i];
                }
            }

            return result;
        }
    }
}
