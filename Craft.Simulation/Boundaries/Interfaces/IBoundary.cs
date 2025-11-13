using Craft.Math;
using Craft.Simulation.BodyStates;

namespace Craft.Simulation.Boundaries.Interfaces
{
    public interface IBoundary
    {
        bool Visible { get; }

        string Tag { get; }

        double DistanceToPoint(
            Vector2D point);

        double DistanceToBody(
            BodyState bodyState);

        bool Intersects(
            BodyState bodyState);
    }
}
