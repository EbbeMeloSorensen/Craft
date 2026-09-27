using Xunit;

namespace Craft.Math.UnitTest;

public class Arrow2DTest
{
    [Theory]
    [InlineData(3, 5)]
    [InlineData(2, 0)]
    [InlineData(2.5, 5)]
    [InlineData(358, 0)]
    [InlineData(-6, 355)]
    [InlineData(721, 0)]
    public void SnapsToNearestFiveDegrees(double input, double expected)
        => Assert.Equal(expected, Arrow2D.SnapAngle(input));

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0.01, 0.01)]
    [InlineData(10, 3)]
    [InlineData(0.2, 50)]
    public void WorldShaftPreservesViewportLengthAndAngle(double scaleX, double scaleY)
    {
        for (var angle = 0; angle < 360; angle += 5)
        {
            var arrow = Arrow2D.FromAngle(new Point2D(12, -34), angle);
            var shaft = arrow.GetWorldShaft(scaleX, scaleY);
            var dx = (shaft.Point2.X - shaft.Point1.X) * scaleX;
            var dy = (shaft.Point2.Y - shaft.Point1.Y) * scaleY;
            Assert.Equal(100, System.Math.Sqrt(dx * dx + dy * dy), 8);
            var actualAngle = Arrow2D.SnapAngle(System.Math.Atan2(-dy, dx) * 180 / System.Math.PI);
            Assert.Equal(angle, actualAngle);
        }
    }
}
