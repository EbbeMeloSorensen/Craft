using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn;

public static class Helpers
{
    public static bool Contains(
        this BoundingBox boundingBox,
        BoundingBox otherBoundingBox)
    {
        return boundingBox.MinX <= otherBoundingBox.MinX &&
               otherBoundingBox.MaxX <= boundingBox.MaxX &&
               boundingBox.MinY <= otherBoundingBox.MinY &&
               otherBoundingBox.MaxY <= boundingBox.MaxY;
    }

    public static BoundingBox Expand(
        this BoundingBox boundingBox,
        double factor)
    {
        var width = boundingBox.Width;
        var height = boundingBox.Height;

        var expandX = width * (factor - 1) / 1.2;
        var expandY = height * (factor - 1) / 1.2;

        return new BoundingBox(
            boundingBox.MinX - expandX,
            boundingBox.MaxX + expandX,
            boundingBox.MinY - expandY,
            boundingBox.MaxY + expandY);
    }

    public static BoundingBox ComputeBoundingBox(
        this LineModel lineModel)
    {
        var minX = System.Math.Min(lineModel.P1.X, lineModel.P2.X);
        var maxX = System.Math.Max(lineModel.P1.X, lineModel.P2.X);
        var minY = System.Math.Min(lineModel.P1.Y, lineModel.P2.Y);
        var maxY = System.Math.Max(lineModel.P1.Y, lineModel.P2.Y);
        return new BoundingBox(minX, maxX, minY, maxY);
    }
}

