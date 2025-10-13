using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.yjs
{
    public class Arrow
    {
        [XmlAttribute]
        public string type { get; set; }

        [XmlAttribute]
        public double scale { get; set; }

        [XmlAttribute]
        public string stroke { get; set; }

        [XmlAttribute]
        public string fill { get; set; }

        [XmlAttribute]
        public int cropLength { get; set; }
    }
}
