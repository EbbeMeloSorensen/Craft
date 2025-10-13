using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.yjs
{
    public class PolylineEdgeStyle
    {
        [XmlAttribute]
        public string stroke { get; set; }

        [XmlElement(ElementName = "PolylineEdgeStyle.targetArrow")]
        public PolylineEdgeStyleTargetArrow PolylineEdgeStyleTargetArrow { get; set; }

        [XmlElement(ElementName = "PolylineEdgeStyle.stroke")]
        public PolylineEdgeStyleStroke PolylineEdgeStyleStroke { get; set; }

    }
}