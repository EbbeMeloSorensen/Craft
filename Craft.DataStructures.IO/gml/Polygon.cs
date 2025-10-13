using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class Polygon : AbstractSurface
    {
        [XmlElement(ElementName = "exterior")]
        public Exterior Exterior { get; set; }
    }
}
