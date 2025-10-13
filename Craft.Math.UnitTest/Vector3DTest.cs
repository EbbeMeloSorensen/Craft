using Xunit;
using FluentAssertions;

namespace Craft.Math.UnitTest
{
    public class Vector3DTest
    {
        private const double _tolerance = 0.000000001;

        [Fact]
        public void Length_GivenOrdinaryVector_ReturnsProperLength()
        {
            // Arrange
            var vector = new Vector3D(4, 3, 2);

            // Act
            var length = vector.Length;

            // Assert
            length.Should().BeApproximately(5.3851648071345037, _tolerance);
        }

        [Fact]
        public void UnaryMinusOperator_GivenOrdinaryVector_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(4, -3, 2);

            // Act
            var vector2 = -vector1;

            // Assert
            vector2.X.Should().Be(-4);
            vector2.Y.Should().Be(3);
            vector2.Z.Should().Be(-2);
        }

        [Fact]
        public void AsSphericalVector_GivenUnitVectorAlongXAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(1, 0, 0);

            // Act
            var vector2 = vector1.AsSphericalVector();

            // Assert
            vector2.RadialDistance.Should().BeApproximately(1, _tolerance);
            vector2.PolarAngle.Should().BeApproximately(System.Math.PI / 2, _tolerance);
            vector2.AzimuthalAngle.Should().BeApproximately(0, _tolerance);
        }

        [Fact]
        public void AsSphericalVector_GivenNegativeUnitVectorAlongXAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(-1, 0, 0);

            // Act
            var vector2 = vector1.AsSphericalVector();

            // Assert
            vector2.RadialDistance.Should().BeApproximately(1, _tolerance);
            vector2.PolarAngle.Should().BeApproximately(System.Math.PI / 2, _tolerance);
            vector2.AzimuthalAngle.Should().BeApproximately(System.Math.PI, _tolerance);
        }

        [Fact]
        public void AsSphericalVector_GivenUnitVectorAlongYAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 1, 0);

            // Act
            var vector2 = vector1.AsSphericalVector();

            // Assert
            vector2.RadialDistance.Should().BeApproximately(1, _tolerance);
            vector2.PolarAngle.Should().BeApproximately(System.Math.PI / 2, _tolerance);
            vector2.AzimuthalAngle.Should().BeApproximately(System.Math.PI / 2, _tolerance);
        }

        [Fact]
        public void AsSphericalVector_GivenNegativeUnitVectorAlongYAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, -1, 0);

            // Act
            var vector2 = vector1.AsSphericalVector();

            // Assert
            vector2.RadialDistance.Should().BeApproximately(1, _tolerance);
            vector2.PolarAngle.Should().BeApproximately(System.Math.PI / 2, _tolerance);
            vector2.AzimuthalAngle.Should().BeApproximately(-System.Math.PI / 2, _tolerance);
        }

        [Fact]
        public void AsSphericalVector_GivenUnitVectorAlongZAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 0, 1);

            // Act
            var vector2 = vector1.AsSphericalVector();

            // Assert
            vector2.RadialDistance.Should().BeApproximately(1, _tolerance);
            vector2.PolarAngle.Should().BeApproximately(0, _tolerance);
            vector2.AzimuthalAngle.Should().BeApproximately(0, _tolerance);
        }

        [Fact]
        public void AsSphericalVector_GivenNegativeUnitVectorAlongZAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector3D(0, 0, -1);

            // Act
            var vector2 = vector1.AsSphericalVector();

            // Assert
            vector2.RadialDistance.Should().BeApproximately(1, _tolerance);
            vector2.PolarAngle.Should().BeApproximately(System.Math.PI, _tolerance);
            vector2.AzimuthalAngle.Should().BeApproximately(0, _tolerance);
        }
    }
}
