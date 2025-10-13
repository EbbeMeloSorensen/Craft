using Xunit;
using FluentAssertions;

namespace Craft.Math.UnitTest
{
    public class Vector2DTest
    {
        private const double _tolerance = 0.000000001;

        [Fact]
        public void Length_ReturnsProperLength_GivenOrdinaryVector()
        {
            // Arrange
            var vector = new Vector2D(4, 3);

            // Act
            var length = vector.Length;

            // Assert
            length.Should().BeApproximately(5, _tolerance);
        }

        [Fact]
        public void Length_ReturnsZero_GivenThatVectorIsZeroVector()
        {
            // Arrange
            var vector = new Vector2D(0, 0);

            // Act
            var length = vector.Length;

            // Assert
            length.Should().BeApproximately(0, _tolerance);
        }

        [Fact]
        public void UnaryMinusOperator_ReturnsCorrectResult_GivenOrdinaryVector()
        {
            // Arrange
            var vector1 = new Vector2D(4, -3);

            // Act
            var vector2 = -vector1;

            // Assert
            vector2.X.Should().Be(-4);
            vector2.Y.Should().Be(3);
        }

        [Fact]
        public void AsPolarVector_GivenUnitVectorAlongXAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector2D(1, 0);

            // Act
            var vector2 = vector1.AsPolarVector();

            // Assert
            vector2.Length.Should().BeApproximately(1, _tolerance);
            vector2.Angle.Should().BeApproximately(0, _tolerance);
        }

        [Fact]
        public void AsPolarVector_GivenUnitVectorAlongYAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector2D(0, 1);

            // Act
            var vector2 = vector1.AsPolarVector();

            // Assert
            vector2.Length.Should().BeApproximately(1, _tolerance);
            vector2.Angle.Should().BeApproximately(System.Math.PI / 2, _tolerance);
        }

        [Fact]
        public void AsPolarVector_GivenNegativeUnitVectorAlongXAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector2D(-1, 0);

            // Act
            var vector2 = vector1.AsPolarVector();

            // Assert
            vector2.Length.Should().BeApproximately(1, _tolerance);
            vector2.Angle.Should().BeApproximately(System.Math.PI, _tolerance);
        }

        [Fact]
        public void AsPolarVector_GivenNegativeUnitVectorAlongYAxis_ReturnsCorrectResult()
        {
            // Arrange
            var vector1 = new Vector2D(0, -1);

            // Act
            var vector2 = vector1.AsPolarVector();

            // Assert
            vector2.Length.Should().BeApproximately(1, _tolerance);
            vector2.Angle.Should().BeApproximately(-System.Math.PI / 2, _tolerance);
        }
    }
}