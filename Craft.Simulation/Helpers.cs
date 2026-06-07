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
            _ => throw new ArgumentException()
        };
    }
}

