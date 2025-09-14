namespace Craft.Utils
{
    public class Pixel
    {
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
        public byte Alpha { get; }
        public string TextOverlay { get; }
        public string ImagePath { get; set; }

        public Pixel(
            byte red,
            byte green,
            byte blue,
            byte alpha,
            string textOverlay = null)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
            TextOverlay = textOverlay;
        }
    }
}
