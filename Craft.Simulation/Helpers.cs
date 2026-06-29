using Craft.DataStructures.Geometry;
using Craft.Simulation.Boundaries;
using Craft.Simulation.Boundaries.Interfaces;

namespace Craft.Simulation;

internal static class Helpers
{
    public static BoundingBox ComputeBoundingBox(
        this IBoundary boundary)
    {
        return boundary switch
        {
            HorizontalLineSegment horizontalLineSegment => new BoundingBox(
                horizontalLineSegment.X0,
                horizontalLineSegment.X1,
                horizontalLineSegment.Y,
                horizontalLineSegment.Y),
            VerticalLineSegment verticalLineSegment => new BoundingBox(
                verticalLineSegment.X,
                verticalLineSegment.X,
                verticalLineSegment.Y0,
                verticalLineSegment.Y1),
            LineSegment lineSegment => new BoundingBox(
                System.Math.Min(lineSegment.Point1.X, lineSegment.Point2.X),
                System.Math.Max(lineSegment.Point1.X, lineSegment.Point2.X),
                System.Math.Min(lineSegment.Point1.Y, lineSegment.Point2.Y),
                System.Math.Max(lineSegment.Point1.Y, lineSegment.Point2.Y)),
            CircularBoundary circularBoundary => new BoundingBox(
                circularBoundary.Center.X - circularBoundary.Radius,
                circularBoundary.Center.X + circularBoundary.Radius,
                circularBoundary.Center.Y - circularBoundary.Radius,
                circularBoundary.Center.Y + circularBoundary.Radius),
            _ => throw new ArgumentException("Unknown boundary type")
        };
    }
}

