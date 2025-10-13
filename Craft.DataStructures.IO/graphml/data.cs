using Craft.DataStructures.IO.graphml.x;
using Craft.DataStructures.IO.graphml.y;
using Craft.DataStructures.IO.graphml.yjs;
using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml
{
    public class data
    {
        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-common/3.0")]
        public SharedData SharedData { get; set; }

        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-common/3.0")]
        public Json Json { get; set; }

        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-common/markup/3.0")]
        public List List { get; set; }

        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-common/3.0")]
        public RectD RectD { get; set; }

        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-for-html/2.0/xaml")]
        public ShapeNodeStyle ShapeNodeStyle { get; set; }

        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-for-html/2.0/xaml")]
        public RectangleNodeStyle RectangleNodeStyle { get; set; }

        [XmlElement(Namespace = "http://www.yworks.com/xml/yfiles-for-html/2.0/xaml")]
        public PolylineEdgeStyle PolylineEdgeStyle { get; set; }

        [XmlAttribute]
        public string key { get; set; }

        [XmlText]
        public string value { get; set; }
    }
}
