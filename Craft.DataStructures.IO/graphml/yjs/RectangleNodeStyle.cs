using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.yjs
{
    public class RectangleNodeStyle
    {
        [XmlAttribute]
        public string stroke { get; set; }

        [XmlAttribute]
        public string fill { get; set; }
    }
}
