using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class CompositeLabelModel
    {
        [XmlElement(ElementName = "CompositeLabelModel.LabelModels")]
        public CompositeLabelModelLabelModels CompositeLabelModelLabelModels { get; set; }
    }
}
