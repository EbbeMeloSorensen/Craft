using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class CompositeLabelModelParameter
    {
        [XmlElement(ElementName = "CompositeLabelModelParameter.Parameter")]
        public CompositeLabelModelParameterParameter CompositeLabelModelParameterParameter { get; set; }

        [XmlElement(ElementName = "CompositeLabelModelParameter.Model")]
        public CompositeLabelModelParameterModel CompositeLabelModelParameterModel { get; set; }
    }
}