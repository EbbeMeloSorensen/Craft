using System.Xml.Serialization;
using Craft.DataStructures.IO.graphml.yjs;

namespace Craft.DataStructures.IO.graphml.y
{
    public class Style
    {
        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-for-html/2.0/xaml")]
        public DefaultLabelStyle DefaultLabelStyle { get; set; }
    }
}