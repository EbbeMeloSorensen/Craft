namespace Craft.Math
{
    public class Matrix
    {
        private readonly double[,] _elements;

        public Matrix()
        {
        }

        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;

            if (rows > 0 && columns > 0)
            {
                _elements = new double[rows, columns];
            }
        }

        public Matrix(IEnumerable<double> elements, int rows, int columns)
        {
            Columns = columns;
            Rows = rows;

            _elements = new double[rows, columns];

            var elementCount = Rows * Columns;

            using (var enumerator = elements.GetEnumerator())
            {
                for (var i = 0; i < elementCount; i++)
                {
                    enumerator.MoveNext();
                    _elements[i / Columns, i % Columns] = enumerator.Current;
                }
            }
        }

        public double this[int rowIndex, int columnIndex]
        {
            get { return _elements[rowIndex, columnIndex]; }
            set { _elements[rowIndex, columnIndex] = value; }
        }

        public int Rows { get; }

        public int Columns { get; }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
            {
                throw new ArgumentException("Matrices must be of equal size for addition");
            }

            var result = new Matrix(a.Rows, a.Columns);

            for (var r = 0; r < a.Rows; r++)
            {
                for (var c = 0; c < a.Columns; c++)
                {
                    result[r, c] = a[r, c] + b[r, c];
                }
            }

            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
            {
                throw new ArgumentException("Matrices must be of equal size for addition");
            }

            var result = new Matrix(a.Rows, a.Columns);

            for (var r = 0; r < a.Rows; r++)
            {
                for (var c = 0; c < a.Columns; c++)
                {
                    result[r, c] = a[r, c] - b[r, c];
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Columns != b.Rows)
            {
                throw new ArgumentException("Inner Matrix dimensions must agree for multiplication");
            }

            var result = new Matrix(a.Rows, b.Columns);

            for (var r = 0; r < result.Rows; r++)
            {
                for (var c = 0; c < result.Columns; c++)
                {
                    for (var i = 0; i < a.Columns; i++)
                    {
                        result[r,c] += a[r,i] * b[i,c];
                    }
                }
            }

            return result;
        }

        public static Matrix operator *(double factor, Matrix m)
        {
            var result = new Matrix(m.Rows, m.Columns);

            for (var r = 0; r < result.Rows; r++)
            {
                for (var c = 0; c < result.Columns; c++)
                {
                    result[r, c] = factor * m[r, c];
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix m, double factor)
        {
            return factor * m;
        }

        public static Matrix GenerateIdentityMatrix3x3()
        {
            var result = new Matrix(3, 3);
            result[0, 0] = 1;
            result[1, 1] = 1;
            result[2, 2] = 1;

            return result;
        }

        public static Matrix GenerateIdentityMatrix4x4()
        {
            var result = new Matrix(4, 4);
            result[0, 0] = 1;
            result[1, 1] = 1;
            result[2, 2] = 1;
            result[3, 3] = 1;

            return result;
        }

        public static Matrix GenerateTranslationMatrix(Vector3D vector)
        {
            var result = GenerateIdentityMatrix4x4();
            result[0, 3] = vector.X;
            result[1, 3] = vector.Y;
            result[2, 3] = vector.Z;

            return result;
        }

        public static Matrix GenerateScalingMatrix(double sx, double sy, double sz)
        {
            var result = GenerateIdentityMatrix4x4();
            result[0, 0] = sx;
            result[1, 1] = sy;
            result[2, 2] = sz;

            return result;
        }

        public static Matrix GenerateRotationMatrix(Vector3D vpn, Vector3D vup)
        {
            var Rz = vpn / vpn.Length;

            var crossProduct = Vector3D.CrossProduct(vup, Rz);
            var Rx = crossProduct / crossProduct.Length;

            var Ry = Vector3D.CrossProduct(Rz, Rx);

            var result = new Matrix(4, 4);

            result[0, 0] = Rx.X;
            result[0, 1] = Rx.Y;
            result[0, 2] = Rx.Z;
            result[1, 0] = Ry.X;
            result[1, 1] = Ry.Y;
            result[1, 2] = Ry.Z;
            result[2, 0] = Rz.X;
            result[2, 1] = Rz.Y;
            result[2, 2] = Rz.Z;
            result[3, 3] = 1;

            return result;
        }

        public static Matrix GenerateRotationMatrixForRotationAroundXAxis(double angle)
        {
            var result = GenerateIdentityMatrix3x3();

            var cosAngle = System.Math.Cos(angle);
            var sinAngle = System.Math.Sin(angle);

            result[1, 1] = cosAngle;
            result[1, 2] = -sinAngle;
            result[2, 1] = sinAngle;
            result[2, 2] = cosAngle;

            return result;
        }

        public static Matrix GenerateRotationMatrixForRotationAroundYAxis(double angle)
        {
            var result = GenerateIdentityMatrix3x3();

            var cosAngle = System.Math.Cos(angle);
            var sinAngle = System.Math.Sin(angle);

            result[0, 0] = cosAngle;
            result[0, 2] = sinAngle;
            result[2, 0] = -sinAngle;
            result[2, 2] = cosAngle;

            return result;
        }

        public static Matrix GenerateRotationMatrixForRotationAroundZAxis(double angle)
        {
            var result = GenerateIdentityMatrix3x3();

            var cosAngle = System.Math.Cos(angle);
            var sinAngle = System.Math.Sin(angle);

            result[0, 0] = cosAngle;
            result[0, 1] = -sinAngle;
            result[1, 0] = sinAngle;
            result[1, 1] = cosAngle;

            return result;
        }

        public static Matrix GenerateRotationMatrix(Vector3D axis, double angle)
        {
            var result = GenerateIdentityMatrix3x3();

            var cosAngle = System.Math.Cos(angle);
            var sinAngle = System.Math.Sin(angle);

            result[0, 0] = cosAngle + axis.X * axis.X * (1 - cosAngle);
            result[0, 1] = axis.X * axis.Y * (1 - cosAngle) - axis.Z * sinAngle;
            result[0, 2] = axis.X * axis.Z * (1 - cosAngle) + axis.Y * sinAngle;

            result[1, 0] = axis.X * axis.Y * (1 - cosAngle) + axis.Z * sinAngle;
            result[1, 1] = cosAngle + axis.Y * axis.Y * (1 - cosAngle);
            result[1, 2] = axis.Y * axis.Z * (1 - cosAngle) - axis.X * sinAngle;

            result[2, 0] = axis.X * axis.Z * (1 - cosAngle) - axis.Y * sinAngle;
            result[2, 1] = axis.Y * axis.Z * (1 - cosAngle) + axis.X * sinAngle;
            result[2, 2] = cosAngle + axis.Z * axis.Z * (1 - cosAngle);

            return result;
        }

        public static Matrix GenerateRotationMatrix(double rotationAngleAroundX, double rotationAngleAroundY)
        {
            var m1 = GenerateRotationMatrixForRotationAroundXAxis(rotationAngleAroundX);
            var m2 = GenerateRotationMatrixForRotationAroundYAxis(rotationAngleAroundY);

            return m2 * m1;
        }

        public static Matrix GenerateXYShearMatrix(double shx, double shy)
        {
            var result = GenerateIdentityMatrix4x4();
            result[0, 2] = shx;
            result[1, 2] = shy;

            return result;
        }
    }
}
