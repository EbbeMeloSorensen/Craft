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

}

