namespace Craft.Simulation.Bodies
{
    public class RectangularBody : Body
    {
        public double Width { get; }
        public double Height { get; }

        public RectangularBody(
            int id,
            double width,
            double height,
            double mass,
            bool affectedByGravity,
            bool affectedByBoundaries = true,
            string? tag = null) : base(id, mass, affectedByGravity, affectedByBoundaries, tag)
        {
            Width = width;
            Height = height;
        }
    }
}