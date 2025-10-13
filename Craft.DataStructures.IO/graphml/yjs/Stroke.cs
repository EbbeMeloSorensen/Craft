using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.yjs
{
    public class Stroke
    {
        [XmlAttribute]
        public string fill { get; set; }

        [XmlAttribute]
        public double thickness { get; set; }
    }
}