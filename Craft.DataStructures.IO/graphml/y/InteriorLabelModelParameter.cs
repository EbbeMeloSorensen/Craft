using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class InteriorLabelModelParameter
    {
        [XmlAttribute]
        public string Position { get; set; }

        [XmlAttribute]
        public string Model { get; set; }
    }
}