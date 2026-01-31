using Xunit;
using FluentAssertions;

namespace Craft.Math.UnitTest
{
    public class VectorTest
    {
        [Fact]
        public void Constructor1_works()
        {
            var v = new Vector(2);
        }

        [Fact]
        public void Constructor2_works()
        {
            // Act
            var v = new Vector(Enumerable.Range(1, 3).Select(x => (double)x));

            // Assert
            v.Size.Should().Be(3);
        }

        [Fact]
        public void Size_works()
        {
            // Act
            var v = new Vector(2);

            // Assert
            v.Size.Should().Be(2);
        }

        [Fact]
        public void IndexingOperator_works()
        {
            // Arrange
            var v = new Vector(2);

            // Act
            v[0] = 7;
            var a = v[0];

            // Assert
            a.Should().Be(7);
        }

        [Fact]
        public void Addition_works()
        {
            // Arrange
            var v1 = new Vector(2);
            v1[0] = 1;
            v1[1] = 2;

            var v2 = new Vector(2);
            v2[0] = 3;
            v2[1] = 4;

            // Act
            var v3 = v1 + v2;

            // Assert
            v3.Size.Should().Be(2);
            v3[0].Should().BeApproximately(4, 0.0000000001);
            v3[1].Should().BeApproximately(6, 0.0000000001);
        }

        [Fact]
        public void Subtraction_works()
        {
            // Arrange
            var v1 = new Vector(2);
            v1[0] = 5;
            v1[1] = 9;

            var v2 = new Vector(2);
            v2[0] = 3;
            v2[1] = 4;

            // Act
            var v3 = v1 - v2;

            // Assert
            v3.Size.Should().Be(2);
            v3[0].Should().BeApproximately(2, 0.0000000001);
            v3[1].Should().BeApproximately(5, 0.0000000001);
        }

        [Fact]
        public void DotProduct_works()
        {
            // Arrange
            var v1 = new Vector(2);
            v1[0] = 1;
            v1[1] = 2;

            var v2 = new Vector(2);
            v2[0] = 3;
            v2[1] = 4;

            // Act
            var dotProduct = Vector.DotProduct(v1, v2);

            // Assert
            dotProduct.Should().BeApproximately(11, 0.000000001);
        }

        [Fact]
        public void MultiplicationByScalar_givenFactorToTheLeft_works()
        {
            // Arrange
            var v1 = new Vector(3);
            v1[0] = 1;
            v1[1] = 2;
            v1[2] = 3;

            // Act
            var v2 = 3 * v1;

            // Assert
            v2.Size.Should().Be(3);
            v2[0].Should().BeApproximately(3, 0.000000001);
            v2[1].Should().BeApproximately(6, 0.000000001);
            v2[2].Should().BeApproximately(9, 0.000000001);
        }

        [Fact]
        public void MultiplicationByScalar_givenFactorToTheRight_works()
        {
            // Arrange
            var v1 = new Vector(3);
            v1[0] = 1;
            v1[1] = 2;
            v1[2] = 3;

            // Act
            var v2 = v1 * 3;

            // Assert
            v2.Size.Should().Be(3);
            v2[0].Should().BeApproximately(3, 0.000000001);
            v2[1].Should().BeApproximately(6, 0.000000001);
            v2[2].Should().BeApproximately(9, 0.000000001);
        }

        [Fact]
        public void MultiplicationByMatrix_givenMatrixToTheLeft_works()
        {
            // Arrange
            var v1 = new Vector(2);
            v1[0] = 1;
            v1[1] = 2;

            var m = new Matrix(3, 2);
            m[0, 0] = 1;
            m[0, 1] = 2;
            m[1, 0] = 3;
            m[1, 1] = 4;
            m[2, 0] = 5;
            m[2, 1] = 6;

            // Act
            var v2 = m * v1;

            // Assert
            v2.Size.Should().Be(3);
            v2[0].Should().BeApproximately(5, 0.0000001);
            v2[1].Should().BeApproximately(11, 0.0000001);
            v2[2].Should().BeApproximately(17, 0.0000001);
        }

        [Fact]
        public void TwoNorm_works()
        {
            // Arrange
            var v = new Vector(2);
            v[0] = 3;
            v[1] = 4;

            // Act
            var twoNorm = v.TwoNorm();

            // Assert
            twoNorm.Should().BeApproximately(5, 0.00000001);
        }
    }
}
