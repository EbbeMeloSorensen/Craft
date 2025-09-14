namespace Craft.Utils
{
    public class BoundingBox
    {
        public double Left { get; set; }
        public double Top { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public PointD Position => new PointD(Left, Top);

        public double Right => Left + Width;

        public double Bottom => Top + Height;

        public void Place(
            double left,
            double top)
        {
            Left = left;
            Top = top;
        }
    }
}
