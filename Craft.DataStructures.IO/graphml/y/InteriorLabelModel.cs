using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class InteriorLabelModel
    {
        [XmlAttribute(Namespace = "http://www.yworks.com/xml/yfiles-common/markup/3.0")]
        public int Key { get; set; }
    }
}