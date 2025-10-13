using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class PosList
    {
        [XmlText]
        public string value { get; set; }
    }
}
