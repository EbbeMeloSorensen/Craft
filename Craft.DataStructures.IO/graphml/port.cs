using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml
{
    public class port
    {
        [XmlAttribute]
        public string name { get; set; }
    }
}