namespace Craft.DataStructures.MxCifQuadTree;

public class Rectangle
{
    public double CenterX { get; }
    public double CenterY { get; }

    public double HalfWidth { get; }
    public double HalfHeight { get; }

    public Rectangle(
        double centerX,
        double centerY,
        double halfWidth,
        double halfHeight)
    {
        CenterX = centerX;
        CenterY = centerY;
        HalfWidth = halfWidth;
        HalfHeight = halfHeight;
    }

    public bool Intersects(
        Rectangle rectangle)
    {
        var v0a = CenterX - HalfWidth;
        var v1a = CenterX + HalfWidth;
        var v0b = rectangle.CenterX - rectangle.HalfWidth;
        var v1b = rectangle.CenterX + rectangle.HalfWidth;

        // Test for overlap on the X axis
        if ((v0a <= v0b && v0b <= v1a) ||
            (v0b <= v0a && v0a <= v1b))
        {
            v0a = CenterY - HalfHeight;
            v1a = CenterY + HalfHeight;
            v0b = rectangle.CenterY - rectangle.HalfHeight;
            v1b = rectangle.CenterY + rectangle.HalfHeight;

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