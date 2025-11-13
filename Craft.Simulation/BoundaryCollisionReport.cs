using Craft.Math;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries.Interfaces;

namespace Craft.Simulation
{
    public class BoundaryCollisionReport
    {
        public BodyState BodyState { get; }
        public IBoundary Boundary { get; }
        public Vector2D EffectiveSurfaceNormal { get; }

        public BoundaryCollisionReport(
            BodyState bodyState,
            IBoundary boundary,
            Vector2D effectiveSurfaceNormal)
        {
            BodyState = bodyState;
            Boundary = boundary;
            EffectiveSurfaceNormal = effectiveSurfaceNormal;
        }
    }
}