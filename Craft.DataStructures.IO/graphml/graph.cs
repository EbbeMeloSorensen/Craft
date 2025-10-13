using System.Collections;
using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml
{
    public class graph
    {
        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public edgedefault edgedefault { get; set; }

        [XmlElement(Type = typeof(data)),
         XmlElement(Type = typeof(node)),
         XmlElement(Type = typeof(edge))]
        public ArrayList graphElements { get; set; }
    }
}