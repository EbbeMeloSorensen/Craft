using System.Xml.Serialization;
using Craft.DataStructures.IO.gml;

namespace Craft.DataStructures.IO.ogr
{
    // Alle de GML-filer, jeg har kigget på indtil videre, har et custom element, som er child af FeatureCollection og
    // parent for noget geometri
    public class Dummy
    {
        [XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
        public string Id { get; set; }

        [XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
        public BoundedBy BoundedBy { get; set; }

        [XmlElement(ElementName = "geometryProperty")]
        public GeometryProperty GeometryProperty { get; set; }

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }
    }
}
