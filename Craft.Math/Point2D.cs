namespace Craft.Math
{
    public class Point2D
    {
        public double X { get; }
        public double Y { get; }

        public Point2D(
            double x, 
            double y)
        {
            X = x;
            Y = y;
        }

        public Vector2D AsVector2D()
        {
            return new Vector2D(X, Y);
        }

        public static Point2D operator +(
            Point2D point1, 
            Vector2D point2)
        {
            return new Point2D(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point2D operator -(
            Point2D point1, 
            Vector2D point2)
        {
            return new Point2D(point1.X - point2.X, point1.Y - point2.Y);
        }
    }
}
