using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class Label
    {
        [XmlElement(ElementName = "Label.Text")]
        public string LabelText { get; set; }

        [XmlElement(ElementName = "Label.LayoutParameter")]
        public LayoutParameter LayoutParameter { get; set; }

        [XmlElement(ElementName = "Label.Style")]
        public Style Style { get; set; }
    }
}