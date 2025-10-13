using System.Collections;
using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml
{
    [XmlRoot(Namespace = "http://graphml.graphdrawing.org/xmlns")]
    public class graphml
    {
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }

        [XmlElement(Type = typeof(key)),
         XmlElement(Type = typeof(data)),
         XmlElement(Type = typeof(graph))]
        public ArrayList graphmlElements { get; set; }
    }
}