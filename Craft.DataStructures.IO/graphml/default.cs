using System.Xml.Serialization;
using Craft.DataStructures.IO.graphml.x;

namespace Craft.DataStructures.IO.graphml
{
    public class @default
    {
        [XmlText]
        public string value { get; set; }

        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-common/markup/3.0")]
        public Static Static { get; set; }
    }
}