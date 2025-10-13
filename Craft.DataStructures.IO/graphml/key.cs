using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml
{
    public class key
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "for")]
        //public Domain Domain { get; set; }
        public string Domain { get; set; }

        [XmlAttribute(AttributeName = "attr.type")]
        //public GraphMLAttributeType GraphMLAttributeType { get; set; }
        public string GraphMLAttributeType { get; set; }

        [XmlAttribute(AttributeName = "attr.name")]
        public string GraphMLAttributeName { get; set; }

        [XmlAttribute(AttributeName = "attr.uri", Namespace = "http://www.yworks.com/xml/yfiles-common/3.0")]
        public string AttributeURI { get; set; }

        public @default @default { get; set; }
    }
}