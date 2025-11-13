using Craft.Math;

namespace Craft.Simulation.Props
{
    public class PropCircle : Prop
    {
        public double Diameter { get; }
        public Vector2D Position { get; }

        public PropCircle(
            int id,
            double diameter,
            Vector2D position) : base(id)
        {
            Diameter = diameter;
            Position = position;
        }

        public override double DistanceToPoint(
            Vector2D point)
        {
            return System.Math.Sqrt(point.SquaredDistanceTo(Position)) - Diameter / 2;
        }
    }
}