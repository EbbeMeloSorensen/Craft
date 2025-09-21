namespace Craft.Math
{
    public class Size2D
    {
        public double Width { get; }
        public double Height { get; }

        public Size2D(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public static Size2D operator *(Size2D size, double factor)
        {
            return new Size2D(size.Width * factor, size.Height * factor);
        }

        public static Size2D operator /(Size2D size, double divisor)
        {
            return new Size2D(size.Width / divisor, size.Height / divisor);
        }
    }
}