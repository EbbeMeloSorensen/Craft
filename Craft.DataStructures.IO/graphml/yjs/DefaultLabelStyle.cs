using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.yjs
{
    public class DefaultLabelStyle
    {
        [XmlAttribute]
        public string verticalTextAlignment { get; set; }

        [XmlAttribute]
        public string horizontalTextAlignment { get; set; }

        [XmlAttribute]
        public string textFill { get; set; }

        [XmlElement(ElementName = "DefaultLabelStyle.font")]
        public DefaultLabelStyleFont DefaultLabelStyleFont { get; set; }
    }
}