namespace Craft.DataStructures.MxCifQuadTree;

public class Rectangle
{
    public double MinX { get; }
    public double MaxX { get; }
    public double MinY { get; }
    public double MaxY { get; }

    public double CenterX => (MinX + MaxX) / 2;
    public double CenterY => (MinY + MaxY) / 2;

    public double HalfWidth => (MaxX - MinX) / 2;
    public double HalfHeight => (MaxY - MinY) / 2;

    public Rectangle(
        double centerX,
        double centerY,
        double halfWidth,
        double halfHeight)
    {
        MinX = centerX - halfWidth;
        MaxX = centerX + halfWidth;
        MinY = centerY - halfHeight;
        MaxY = centerY + halfHeight;
    }

    public bool Intersects(
        Rectangle rectangle)
    {
        var v0a = MinX;
        var v1a = MaxX;
        var v0b = rectangle.MinX;
        var v1b = rectangle.MaxX;

        // Test for overlap on the X axis
        if ((v0a <= v0b && v0b <= v1a) ||
            (v0b <= v0a && v0a <= v1b))
        {
            v0a = MinY;
            v1a = MaxY;
            v0b = rectangle.MinY;
            v1b = rectangle.MaxY;

            // Test for overlap on the Y axis
            if ((v0a <= v0b && v0b <= v1a) ||
                (v0b <= v0a && v0a <= v1b))
            {
                return true;
            }
        }

        return false;
    }
}