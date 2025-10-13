using Craft.DataStructures.IO.gml;
using System.Xml.Serialization;

namespace Craft.DataStructures.IO.ogr
{
    public class GeometryProperty
    {
        [XmlElement(Namespace = "http://www.opengis.net/gml/3.2")]
        public AbstractGeometricPrimitive AbstractGeometricPrimitive { get; set; }
    }
}
