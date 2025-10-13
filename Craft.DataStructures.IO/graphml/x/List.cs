using System.Xml.Serialization;
using Craft.DataStructures.IO.graphml.y;

namespace Craft.DataStructures.IO.graphml.x
{
    public class List
    {
        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-common/3.0")]
        public Label Label { get; set; }
    }
}
