using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public abstract class AbstractGeometricPrimitive
    {
        [XmlAttribute(AttributeName = "srsName")]
        public string SrsName { get; set; }

        [XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
        public string Id { get; set; }
    }
}
