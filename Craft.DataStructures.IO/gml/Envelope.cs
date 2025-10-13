using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class Envelope
    {
        [XmlAttribute(AttributeName = "srsName")]
        public string SrsName { get; set; }

        [XmlElement(ElementName = "lowerCorner")]
        public string LowerCorner { get; set; }

        [XmlElement(ElementName = "upperCorner")]
        public string UpperCorner { get; set; }
    }
}
