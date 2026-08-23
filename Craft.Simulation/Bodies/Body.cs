namespace Craft.Simulation.Bodies
{
    public abstract class Body
    {
        public int Id { get; }
        public bool Visible { get; private set; }

        public double Mass { get; private set; }
        public bool AffectedByGravity { get; private set; }
        public bool AffectedByBoundaries { get; private set; }
        public string? Tag { get; }

        public Body(
            int id,
            double mass,
            bool affectedByGravity,
            bool affectedByBoundaries,
            string? tag,
            bool visible = true)
        {
            Id = id;
            Mass = mass;
            AffectedByGravity = affectedByGravity;
            AffectedByBoundaries = affectedByBoundaries;
            Tag = tag;
            Visible = visible;
        }
    }
}