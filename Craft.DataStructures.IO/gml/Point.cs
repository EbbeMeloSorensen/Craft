using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class Point : AbstractGeometricPrimitive
    {
        [XmlElement(ElementName = "pos")] 
        public string Position { get; set; }
    }
}
