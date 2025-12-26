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
            bool affectedByBoundaries = true) : base(id, mass, affectedByGravity, affectedByBoundaries)
        {
            Radius = radius;
        }
    }
}
