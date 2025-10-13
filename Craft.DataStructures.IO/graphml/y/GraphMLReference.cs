using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class GraphMLReference
    {
        [XmlAttribute]
        public string ResourceKey { get; set; }
    }
}