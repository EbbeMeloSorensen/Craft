using Xunit;
using FluentAssertions;

namespace Craft.Algorithms.UnitTest
{
    public class DistanceTransformTest
    {
        [Fact]
        public void EuclideanDistanceTransform_GivesCorrectDistanceVectorXValues()
        {
            // Arrange
            var input = new int[5, 5];
            input[2, 2] = 1;

            // Act
            DistanceTransform.EuclideanDistanceTransform(
                input,
                out var distances,
                out var xValues,
                out var yValues);

            // Assert
            xValues[0, 0].Should().Be(2);
            xValues[1, 0].Should().Be(2);
            xValues[2, 0].Should().Be(2);
            xValues[3, 0].Should().Be(2);
            xValues[4, 0].Should().Be(2);
            xValues[0, 1].Should().Be(1);
            xValues[1, 1].Should().Be(1);
            xValues[2, 1].Should().Be(1);
            xValues[3, 1].Should().Be(1);
            xValues[4, 1].Should().Be(1);
            xValues[0, 2].Should().Be(0);
            xValues[1, 2].Should().Be(0);
            xValues[2, 2].Should().Be(0);
            xValues[3, 2].Should().Be(0);
            xValues[4, 2].Should().Be(0);
            xValues[0, 3].Should().Be(-1);
            xValues[1, 3].Should().Be(-1);
            xValues[2, 3].Should().Be(-1);
            xValues[3, 3].Should().Be(-1);
            xValues[4, 3].Should().Be(-1);
            xValues[0, 4].Should().Be(-2);
            xValues[1, 4].Should().Be(-2);
            xValues[2, 4].Should().Be(-2);
            xValues[3, 4].Should().Be(-2);
            xValues[4, 4].Should().Be(-2);
        }

        [Fact]
        public void EuclideanDistanceTransform_GivesCorrectDistanceVectorYValues()
        {
            // Arrange
            var input = new int[5, 5];
            input[2, 2] = 1;

            // Act
            DistanceTransform.EuclideanDistanceTransform(
                input,
                out var distances,
                out var xValues,
                out var yValues);

            // Assert
            yValues[0, 0].Should().Be(2);
            yValues[1, 0].Should().Be(1);
            yValues[2, 0].Should().Be(0);
            yValues[3, 0].Should().Be(-1);
            yValues[4, 0].Should().Be(-2);
            yValues[0, 1].Should().Be(2);
            yValues[1, 1].Should().Be(1);
            yValues[2, 1].Should().Be(0);
            yValues[3, 1].Should().Be(-1);
            yValues[4, 1].Should().Be(-2);
            yValues[0, 2].Should().Be(2);
            yValues[1, 2].Should().Be(1);
            yValues[2, 2].Should().Be(0);
            yValues[3, 2].Should().Be(-1);
            yValues[4, 2].Should().Be(-2);
            yValues[0, 3].Should().Be(2);
            yValues[1, 3].Should().Be(1);
            yValues[2, 3].Should().Be(0);
            yValues[3, 3].Should().Be(-1);
            yValues[4, 3].Should().Be(-2);
            yValues[0, 4].Should().Be(2);
            yValues[1, 4].Should().Be(1);
            yValues[2, 4].Should().Be(0);
            yValues[3, 4].Should().Be(-1);
            yValues[4, 4].Should().Be(-2);
        }

        [Fact]
        public void EuclideanDistanceTransform_GivesCorrectDistances()
        {
            // Arrange
            var input = new int[5,5];
            input[2, 2] = 1;

            // Act
            DistanceTransform.EuclideanDistanceTransform(
                input, 
                out var distances,
                out var xValues,
                out var yValues);

            // Assert
            distances[0, 0].Should().BeApproximately(2 * System.Math.Sqrt(2), 0.000000001);
            distances[0, 4].Should().BeApproximately(2 * System.Math.Sqrt(2), 0.000000001);
            distances[4, 0].Should().BeApproximately(2 * System.Math.Sqrt(2), 0.000000001);
            distances[4, 4].Should().BeApproximately(2 * System.Math.Sqrt(2), 0.000000001);
            distances[0, 1].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[1, 0].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[0, 3].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[1, 4].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[3, 0].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[4, 1].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[3, 4].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[4, 3].Should().BeApproximately(System.Math.Sqrt(5), 0.000000001);
            distances[1, 1].Should().BeApproximately(System.Math.Sqrt(2), 0.000000001);
            distances[1, 3].Should().BeApproximately(System.Math.Sqrt(2), 0.000000001);
            distances[3, 1].Should().BeApproximately(System.Math.Sqrt(2), 0.000000001);
            distances[3, 3].Should().BeApproximately(System.Math.Sqrt(2), 0.000000001);
            distances[0, 2].Should().BeApproximately(2, 0.000000001);
            distances[2, 0].Should().BeApproximately(2, 0.000000001);
            distances[2, 4].Should().BeApproximately(2, 0.000000001);
            distances[4, 2].Should().BeApproximately(2, 0.000000001);
            distances[1, 2].Should().BeApproximately(1, 0.000000001);
            distances[2, 1].Should().BeApproximately(1, 0.000000001);
            distances[2, 3].Should().BeApproximately(1, 0.000000001);
            distances[3, 2].Should().BeApproximately(1, 0.000000001);
            distances[2, 2].Should().BeApproximately(0, 0.000000001);

            //xValues.WriteToFile(@"C:\Temp\xValues.txt");
            //yValues.WriteToFile(@"C:\Temp\yValues.txt");
            //distances.WriteToFile(@"C:\Temp\Distances.txt");
        }
    }
}