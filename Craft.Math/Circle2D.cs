namespace Craft.Math
{
    public class Circle2D
    {
        public Point2D Center { get; }
        public double Radius { get; }

        public Circle2D(
            Point2D center,
            double radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}