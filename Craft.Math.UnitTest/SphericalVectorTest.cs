using Xunit;
using FluentAssertions;

namespace Craft.Math.UnitTest
{
    public class SphericalVectorTest
    {
        private const double _tolerance = 0.000000001;

        [Fact]
        public void AsVector3D_GivenSomething_ReturnsCorrectResult()
        {
            // Arrange
            var vectorXPos1 = new SphericalVector(1, System.Math.PI / 2, 0);
            var vectorXNeg1 = new SphericalVector(1, System.Math.PI / 2, System.Math.PI);

            var vectorYPos1 = new SphericalVector(1, System.Math.PI / 2, System.Math.PI / 2);
            var vectorYNeg1 = new SphericalVector(1, System.Math.PI / 2, 3 * System.Math.PI / 2);

            var vectorZPos1 = new SphericalVector(1, 0, 0);
            var vectorZNeg1 = new SphericalVector(1, System.Math.PI, 0);

            // Act
            var vectorXPos2 = vectorXPos1.AsVector3D();
            var vectorXNeg2 = vectorXNeg1.AsVector3D();

            var vectorYPos2 = vectorYPos1.AsVector3D();
            var vectorYNeg2 = vectorYNeg1.AsVector3D();

            var vectorZPos2 = vectorZPos1.AsVector3D();
            var vectorZNeg2 = vectorZNeg1.AsVector3D();

            // Assert
            vectorXPos2.X.Should().BeApproximately(1, _tolerance);
            vectorXPos2.Y.Should().BeApproximately(0, _tolerance);
            vectorXPos2.Z.Should().BeApproximately(0, _tolerance);
            vectorXNeg2.X.Should().BeApproximately(-1, _tolerance);
            vectorXNeg2.Y.Should().BeApproximately(0, _tolerance);
            vectorXNeg2.Z.Should().BeApproximately(0, _tolerance);

            vectorYPos2.X.Should().BeApproximately(0, _tolerance);
            vectorYPos2.Y.Should().BeApproximately(1, _tolerance);
            vectorYPos2.Z.Should().BeApproximately(0, _tolerance);
            vectorYNeg2.X.Should().BeApproximately(0, _tolerance);
            vectorYNeg2.Y.Should().BeApproximately(-1, _tolerance);
            vectorYNeg2.Z.Should().BeApproximately(0, _tolerance);

            vectorZPos2.X.Should().BeApproximately(0, _tolerance);
            vectorZPos2.Y.Should().BeApproximately(0, _tolerance);
            vectorZPos2.Z.Should().BeApproximately(1, _tolerance);
            vectorZNeg2.X.Should().BeApproximately(0, _tolerance);
            vectorZNeg2.Y.Should().BeApproximately(0, _tolerance);
            vectorZNeg2.Z.Should().BeApproximately(-1, _tolerance);
        }
    }
}
