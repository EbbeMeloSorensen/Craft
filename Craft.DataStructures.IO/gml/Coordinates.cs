using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class Coordinates
    {
        [XmlAttribute(AttributeName = "decimal")]
        public string Decimal { get; set; }

        [XmlAttribute(AttributeName = "cs")]
        public string CS { get; set; }

        [XmlAttribute(AttributeName = "ts")]
        public string TS { get; set; }

        [XmlText]
        public string value { get; set; }
    }
}
