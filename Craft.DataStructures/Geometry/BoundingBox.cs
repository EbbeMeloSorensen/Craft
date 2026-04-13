namespace Craft.DataStructures.Geometry;

public class BoundingBox
{
    public double MinX { get; }
    public double MaxX { get; }
    public double MinY { get; }
    public double MaxY { get; }

    public double CenterX => (MinX + MaxX) / 2;
    public double CenterY => (MinY + MaxY) / 2;

    public BoundingBox(
        double minX,
        double maxX,
        double minY,
        double maxY)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }

    public bool Intersects(
        BoundingBox boundingBox)
    {
        var v0a = MinX;
        var v1a = MaxX;
        var v0b = boundingBox.MinX;
        var v1b = boundingBox.MaxX;

        // Test for overlap on the X axis
        if (v0a <= v0b && v0b <= v1a ||
            v0b <= v0a && v0a <= v1b)
        {
            v0a = MinY;
            v1a = MaxY;
            v0b = boundingBox.MinY;
            v1b = boundingBox.MaxY;

            // Test for overlap on the Y axis
            if (v0a <= v0b && v0b <= v1a ||
                v0b <= v0a && v0a <= v1b)
            {
                return true;
            }
        }

        return false;
    }
}