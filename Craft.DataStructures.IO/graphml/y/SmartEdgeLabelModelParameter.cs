using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class SmartEdgeLabelModelParameter
    {
        [XmlAttribute]
        public int Distance { get; set; }

        [XmlElement(ElementName = "SmartEdgeLabelModelParameter.Model")]
        public SmartEdgeLabelModelParameterModel SmartEdgeLabelModelParameterModel { get; set; }
    }
}
