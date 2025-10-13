using System.Collections;
using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml
{
    public class edge
    {
        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public string source { get; set; }

        [XmlAttribute]
        public string target { get; set; }

        [XmlAttribute]
        public string sourceport { get; set; }

        [XmlAttribute]
        public string targetport { get; set; }

        [XmlElement(Type = typeof(data))]
        public ArrayList edgeElements { get; set; }
    }
}