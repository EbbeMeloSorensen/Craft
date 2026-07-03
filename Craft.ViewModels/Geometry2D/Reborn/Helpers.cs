using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;

namespace Craft.ViewModels.Geometry2D.Reborn;

public static class Helpers
{
    public static bool Contains(
        this BoundingBox? boundingBox,
        BoundingBox? otherBoundingBox)
    {
        if (boundingBox == null || otherBoundingBox == null)
        {
            return false;
        }

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

    public static BoundingBox ComputeBoundingBox(
        this PointModel pointModel)
    {
        return new BoundingBox(
            pointModel.P.X,
            pointModel.P.X,
            pointModel.P.Y,
            pointModel.P.Y);
    }

    public static BoundingBox ComputeBoundingBox(
        this CircleModel circleModel)
    {
        return new BoundingBox(
            circleModel.Center.X - circleModel.Radius,
            circleModel.Center.X + circleModel.Radius,
            circleModel.Center.Y - circleModel.Radius,
            circleModel.Center.Y + circleModel.Radius);
    }

    public static BoundingBox ComputeBoundingBox(
        this PolyLineModel polyLineModel)
    {
        return new BoundingBox(
            polyLineModel.Points.Min(_ => _.X),
            polyLineModel.Points.Max(_ => _.X),
            polyLineModel.Points.Min(_ => _.Y),
            polyLineModel.Points.Max(_ => _.Y));
    }
}

