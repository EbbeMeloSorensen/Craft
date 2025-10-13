using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class LinearRing
    {
        [XmlElement(ElementName = "posList")]
        public PosList PosList { get; set; }
    }
}
