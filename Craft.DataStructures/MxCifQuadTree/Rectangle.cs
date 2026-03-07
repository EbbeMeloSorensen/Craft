namespace Craft.DataStructures.MxCifQuadTree;

public class Rectangle
{
    public double CenterX { get; }
    public double CenterY { get; }

    public double Width { get; }
    public double Height { get; }

    public Rectangle(
        double centerX,
        double centerY,
        double width,
        double height)
    {
        CenterX = centerX;
        CenterY = centerY;
        Width = width;
        Height = height;
    }

    public bool Intersects(
        Rectangle rectangle)
    {
        // Test for overlap on the X axis
        var v0a = CenterX - Width;
        var v1a = CenterX + Width;
        var v0b = rectangle.CenterX - rectangle.Width;
        var v1b = rectangle.CenterX + rectangle.Width;

        // Test for overlap on the Y axis
        if ((v0a <= v0b && v0b <= v1a) ||
            (v0b <= v0a && v0a <= v1b))
        {
            v0a = CenterY - Height;
            v1a = CenterY + Height;
            v0b = rectangle.CenterY - rectangle.Height;
            v1b = rectangle.CenterY + rectangle.Height;

            if ((v0a <= v0b && v0b <= v1a) ||
                (v0b <= v0a && v0a <= v1b))
            {
                return true;
            }
        }

        return false;
    }
}