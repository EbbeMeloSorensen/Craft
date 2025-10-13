using System.Collections;
using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml
{
    public class node
    {
        [XmlAttribute]
        public string id { get; set; }

        [XmlElement(Type = typeof(data)),
         XmlElement(Type = typeof(port))]
        public ArrayList nodeElements { get; set; }

        public node()
        {}

        public node(string id)
        {
            this.id = id;
            nodeElements = new ArrayList();
        }
    }
}