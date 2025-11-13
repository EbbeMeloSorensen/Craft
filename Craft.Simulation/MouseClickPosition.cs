using Craft.Math;

namespace Craft.Simulation
{
    public class MouseClickPosition
    {
        public Point2D Position { get; }

        public MouseClickPosition(
            Point2D position)
        {
            Position = position;
        }
    }
}
