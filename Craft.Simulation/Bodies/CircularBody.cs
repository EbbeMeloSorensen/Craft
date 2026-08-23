namespace Craft.Simulation.Bodies
{
    public class CircularBody : Body
    {
        public double Radius { get; }

        public CircularBody(
            int id,
            double radius,
            double mass,
            bool affectedByGravity,
            bool affectedByBoundaries = true,
            string? tag = null,
            bool visible = true) : base(id, mass, affectedByGravity, affectedByBoundaries, tag, visible)
        {
            Radius = radius;
        }
    }
}
