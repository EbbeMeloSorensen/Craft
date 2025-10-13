using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class LineString : AbstractCurve
    {
        [XmlElement(ElementName = "coordinates")]
        public Coordinates Coordinates { get; set; }
    }
}
