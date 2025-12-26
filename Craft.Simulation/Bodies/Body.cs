namespace Craft.Simulation.Bodies
{
    public abstract class Body
    {
        public int Id { get; }
        public double Mass { get; private set; }
        public bool AffectedByGravity { get; private set; }
        public bool AffectedByBoundaries { get; private set; }

        public Body(
            int id,
            double mass,
            bool affectedByGravity,
            bool affectedByBoundaries)
        {
            Id = id;
            Mass = mass;
            AffectedByGravity = affectedByGravity;
            AffectedByBoundaries = affectedByBoundaries;
        }
    }
}