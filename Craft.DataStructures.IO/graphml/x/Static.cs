using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.x
{
    public class Static
    {
        [XmlAttribute]
        public string Member { get; set; }
    }
}