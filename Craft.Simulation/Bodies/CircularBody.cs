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
            string? tag = null) : base(id, mass, affectedByGravity, affectedByBoundaries, tag)
        {
            Radius = radius;
        }
    }
}
