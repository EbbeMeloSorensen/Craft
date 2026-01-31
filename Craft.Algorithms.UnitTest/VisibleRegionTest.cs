using Xunit;

namespace Craft.Algorithms.UnitTest
{
    public class VisibleRegionTest
    {
        [Fact]
        public void EuclideanDistanceTransform_GivesCorrectDistanceVectorXValues()
        {
            // Arrange
            var input = new int[5, 5];
            input[2, 2] = 1;

            // Act
            input.Dilate(2);

            // Assert
            //xValues[0, 0].Should().Be(2);
            //xValues[1, 0].Should().Be(2);
            //xValues[2, 0].Should().Be(2);
            //xValues[3, 0].Should().Be(2);
            //xValues[4, 0].Should().Be(2);
            //xValues[0, 1].Should().Be(1);
            //xValues[1, 1].Should().Be(1);
            //xValues[2, 1].Should().Be(1);
            //xValues[3, 1].Should().Be(1);
            //xValues[4, 1].Should().Be(1);
            //xValues[0, 2].Should().Be(0);
            //xValues[1, 2].Should().Be(0);
            //xValues[2, 2].Should().Be(0);
            //xValues[3, 2].Should().Be(0);
            //xValues[4, 2].Should().Be(0);
            //xValues[0, 3].Should().Be(-1);
            //xValues[1, 3].Should().Be(-1);
            //xValues[2, 3].Should().Be(-1);
            //xValues[3, 3].Should().Be(-1);
            //xValues[4, 3].Should().Be(-1);
            //xValues[0, 4].Should().Be(-2);
            //xValues[1, 4].Should().Be(-2);
            //xValues[2, 4].Should().Be(-2);
            //xValues[3, 4].Should().Be(-2);
            //xValues[4, 4].Should().Be(-2);
        }
    }
}
