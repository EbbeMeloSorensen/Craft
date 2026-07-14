using Craft.Math;

namespace Craft.Simulation.Bodies;

public class BodyLineSegment : Body
{
    public Vector2D Point1 { get; set; }
    public Vector2D Point2 { get; set; }

    public BodyLineSegment(
        int id,
        double mass,
        bool affectedByGravity,
        bool affectedByBoundaries,
        string? tag) : base(
            id,
            mass,
            affectedByGravity,
            affectedByBoundaries,
            tag)
    {
    }
}
