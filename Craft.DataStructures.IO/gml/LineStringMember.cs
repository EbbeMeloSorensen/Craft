using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class LineStringMember
    {
        [XmlElement(Namespace = "http://www.opengis.net/gml/3.2")]
        public LineString LineString { get; set; }
    }
}
