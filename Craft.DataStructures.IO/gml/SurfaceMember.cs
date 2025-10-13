using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class SurfaceMember
    {
        [XmlElement(Namespace = "http://www.opengis.net/gml/3.2")]
        public AbstractSurface AbstractSurface { get; set; }
    }
}
