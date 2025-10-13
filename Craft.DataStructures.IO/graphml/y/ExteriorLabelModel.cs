using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class ExteriorLabelModel
    {
        [XmlAttribute]
        public string Insets { get; set; }
    }
}