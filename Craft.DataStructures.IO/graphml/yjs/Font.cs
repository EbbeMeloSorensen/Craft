using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.yjs
{
    public class Font
    {
        [XmlAttribute]
        public int fontSize { get; set; }

        [XmlAttribute]
        public string fontFamily { get; set; }
    }
}