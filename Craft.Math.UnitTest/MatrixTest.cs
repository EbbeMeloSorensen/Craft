using System.Linq;
using Xunit;
using FluentAssertions;

namespace Craft.Math.UnitTest
{
    public class MatrixTest
    {
        private double _toleranceForComparingWithZero = 0.0000000001;

        [Fact]
        public void DefaultConstructor_returnsExpectedResult()
        {
            // Act
            var m = new Matrix();

            // Assert
            m.Rows.Should().Be(0);
            m.Columns.Should().Be(0);
        }

        [Fact]
        public void Constructor1_returnsExpectedResult()
        {
            // Act
            var m = new Matrix(2, 2);

            // Assert
            m.Rows.Should().Be(2);
            m.Columns.Should().Be(2);
            m[0, 0].Should().BeApproximately(0, _toleranceForComparingWithZero);
            m[0, 1].Should().BeApproximately(0, _toleranceForComparingWithZero);
            m[1, 0].Should().BeApproximately(0, _toleranceForComparingWithZero);
            m[1, 1].Should().BeApproximately(0, _toleranceForComparingWithZero);
        }

        [Fact]
        public void Constructor2_returnsExpectedResult()
        {
            // Arrange
            var elements = Enumerable.Range(1, 6).Select(n => (double) n);

            // Act
            var m = new Matrix(elements, 2, 3);

            // Assert
            m.Rows.Should().Be(2);
            m.Columns.Should().Be(3);
            m[0, 0].Should().BeApproximately(1, _toleranceForComparingWithZero);
            m[0, 1].Should().BeApproximately(2, _toleranceForComparingWithZero);
            m[0, 2].Should().BeApproximately(3, _toleranceForComparingWithZero);
            m[1, 0].Should().BeApproximately(4, _toleranceForComparingWithZero);
            m[1, 1].Should().BeApproximately(5, _toleranceForComparingWithZero);
            m[1, 2].Should().BeApproximately(6, _toleranceForComparingWithZero);
        }

        [Fact]
        public void IndexingOperator_works()
        {
            // Arrange
            var m = new Matrix(2, 2);

            // Act
            m[0,1] = 7;
            var a = m[0,1];

            // Assert
            a.Should().Be(7);
        }

        [Fact]
        public void Addition_works()
        {
            // Arrange
            var m1 = new Matrix(2, 2);
            m1[0,0] = 1;
            m1[0,1] = 2;
            m1[1,0] = 3;
            m1[1,1] = 4;

            var m2 = new Matrix(2, 2);
            m2[0, 0] = 2;
            m2[0, 1] = 3;
            m2[1, 0] = 4;
            m2[1, 1] = 5;

            // Act
            var m3 = m1 + m2;

            // Assert
            m3[0, 0].Should().BeApproximately(3, _toleranceForComparingWithZero);
            m3[0, 1].Should().BeApproximately(5, _toleranceForComparingWithZero);
            m3[1, 0].Should().BeApproximately(7, _toleranceForComparingWithZero);
            m3[1, 1].Should().BeApproximately(9, _toleranceForComparingWithZero);
        }

        [Fact]
        public void Subtraction_works()
        {
            // Arrange
            var m1 = new Matrix(2, 2);
            m1[0, 0] = 2;
            m1[0, 1] = 3;
            m1[1, 0] = 4;
            m1[1, 1] = 5;

            var m2 = new Matrix(2, 2);
            m2[0, 0] = 2;
            m2[0, 1] = 4;
            m2[1, 0] = 6;
            m2[1, 1] = 8;

            // Act
            var m3 = m2 - m1;

            // Assert
            m3[0, 0].Should().BeApproximately(0, _toleranceForComparingWithZero);
            m3[0, 1].Should().BeApproximately(1, _toleranceForComparingWithZero);
            m3[1, 0].Should().BeApproximately(2, _toleranceForComparingWithZero);
            m3[1, 1].Should().BeApproximately(3, _toleranceForComparingWithZero);
        }

        [Fact]
        public void Multiplication_works()
        {
            // Arrange
            var m1 = new Matrix(2, 3);
            m1[0, 0] = 1;
            m1[0, 1] = 2;
            m1[0, 2] = 3;
            m1[1, 0] = 4;
            m1[1, 1] = 5;
            m1[1, 2] = 6;

            var m2 = new Matrix(3, 2);
            m2[0, 0] = 2;
            m2[0, 1] = 4;
            m2[1, 0] = 6;
            m2[1, 1] = 8;
            m2[2, 0] = 10;
            m2[2, 1] = 12;

            // Act
            var m3 = m1 * m2;

            // Assert
            m3.Rows.Should().Be(2);
            m3.Columns.Should().Be(2);
            m3[0, 0].Should().BeApproximately(44, _toleranceForComparingWithZero);
            m3[0, 1].Should().BeApproximately(56, _toleranceForComparingWithZero);
            m3[1, 0].Should().BeApproximately(98, _toleranceForComparingWithZero);
            m3[1, 1].Should().BeApproximately(128, _toleranceForComparingWithZero);
        }

        [Fact]
        public void MultiplicationByScalar_givenFactorToTheLeft_works()
        {
            // Arrange
            var m1 = new Matrix(2, 2);
            m1[0, 0] = 1;
            m1[0, 1] = 2;
            m1[1, 0] = 3;
            m1[1, 1] = 4;

            // Act
            var m2 = 2 * m1;

            // Assert
            m2.Rows.Should().Be(2);
            m2.Columns.Should().Be(2);
            m2[0, 0].Should().BeApproximately(2, _toleranceForComparingWithZero);
            m2[0, 1].Should().BeApproximately(4, _toleranceForComparingWithZero);
            m2[1, 0].Should().BeApproximately(6, _toleranceForComparingWithZero);
            m2[1, 1].Should().BeApproximately(8, _toleranceForComparingWithZero);
        }

        [Fact]
        public void MultiplicationByScalar_givenFactorToTheRight_works()
        {
            // Arrange
            var m1 = new Matrix(2, 2);
            m1[0, 0] = 1;
            m1[0, 1] = 2;
            m1[1, 0] = 3;
            m1[1, 1] = 4;

            // Act
            var m2 = m1 * 2;

            // Assert
            m2.Rows.Should().Be(2);
            m2.Columns.Should().Be(2);
            m2[0, 0].Should().BeApproximately(2, _toleranceForComparingWithZero);
            m2[0, 1].Should().BeApproximately(4, _toleranceForComparingWithZero);
            m2[1, 0].Should().BeApproximately(6, _toleranceForComparingWithZero);
            m2[1, 1].Should().BeApproximately(8, _toleranceForComparingWithZero);
        }

        [Fact]
        public void GenerateRotationMatrixForRotationAroundXAxis_GivenUnitVectorAlongYAxis_returnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 1, 0);

            // Act
            var matrix = Matrix.GenerateRotationMatrixForRotationAroundXAxis(System.Math.PI / 2);
            var vector2 = (matrix * vector1.AsVector()).AsVector3D();

            // Assert
            vector2.X.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Y.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Z.Should().BeApproximately(1, _toleranceForComparingWithZero);
        }

        [Fact]
        public void GenerateRotationMatrixForRotationAroundYAxis_GivenUnitVectorAlongZAxis_returnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 0, 1);

            // Act
            var matrix = Matrix.GenerateRotationMatrixForRotationAroundYAxis(System.Math.PI / 2);
            var vector2 = (matrix * vector1.AsVector()).AsVector3D();

            // Assert
            vector2.X.Should().BeApproximately(1, _toleranceForComparingWithZero);
            vector2.Y.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Z.Should().BeApproximately(0, _toleranceForComparingWithZero);
        }

        [Fact]
        public void GenerateRotationMatrixForRotationAroundZAxis_GivenUnitVectorAlongXAxis_returnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(1, 0, 0);

            // Act
            var matrix = Matrix.GenerateRotationMatrixForRotationAroundZAxis(System.Math.PI / 2);
            var vector2 = (matrix * vector1.AsVector()).AsVector3D();

            // Assert
            vector2.X.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Y.Should().BeApproximately(1, _toleranceForComparingWithZero);
            vector2.Z.Should().BeApproximately(0, _toleranceForComparingWithZero);
        }

        [Fact]
        public void GenerateRotationMatrix_GivenUnitVectorAlongXAxisAndRotatingHalfPIAroundYAxis_returnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(1, 0, 0);
            var axis = new Vector3D(0, 1, 0);

            // Act
            var matrix = Matrix.GenerateRotationMatrix(axis, System.Math.PI / 2);
            var vector2 = (matrix * vector1.AsVector()).AsVector3D();

            // Assert
            vector2.X.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Y.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Z.Should().BeApproximately(-1, _toleranceForComparingWithZero);
        }

        [Fact]
        public void GenerateRotationMatrix_GivenUnitVectorAlongYAxisAndRotatingHalfPIAroundXAxis_returnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 1, 0);
            var axis = new Vector3D(1, 0, 0);

            // Act
            var matrix = Matrix.GenerateRotationMatrix(axis, System.Math.PI / 2);
            var vector2 = (matrix * vector1.AsVector()).AsVector3D();

            // Assert
            vector2.X.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Y.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Z.Should().BeApproximately(1, _toleranceForComparingWithZero);
        }

        [Fact]
        public void GenerateRotationMatrix_GivenUnitVectorAlongYAxisAndRotatingHalfPIAroundZAxis_returnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 1, 0);
            var axis = new Vector3D(0, 0, 1);

            // Act
            var matrix = Matrix.GenerateRotationMatrix(axis, System.Math.PI / 2);
            var vector2 = (matrix * vector1.AsVector()).AsVector3D();

            // Assert
            vector2.X.Should().BeApproximately(-1, _toleranceForComparingWithZero);
            vector2.Y.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Z.Should().BeApproximately(0, _toleranceForComparingWithZero);
        }

        [Fact]
        public void GenerateRotationMatrix_GivenUnitVectorAlongYAxis_returnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 1, 0);

            // Act
            var matrix = Matrix.GenerateRotationMatrix(System.Math.PI / 2, System.Math.PI / 2);
            var vector2 = (matrix * vector1.AsVector()).AsVector3D();

            // Assert
            vector2.X.Should().BeApproximately(1, _toleranceForComparingWithZero);
            vector2.Y.Should().BeApproximately(0, _toleranceForComparingWithZero);
            vector2.Z.Should().BeApproximately(0, _toleranceForComparingWithZero);
        }
    }
}
