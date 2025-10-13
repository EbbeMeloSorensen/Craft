using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class RectD
    {
        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        [XmlAttribute]
        public int Width { get; set; }

        [XmlAttribute]
        public int Height { get; set; }
    }
}