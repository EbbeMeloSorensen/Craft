using System.Xml.Serialization;

namespace Craft.DataStructures.IO.graphml.y
{
    public class Json
    {
        [XmlText]
        public string value { get; set; }
    }
}