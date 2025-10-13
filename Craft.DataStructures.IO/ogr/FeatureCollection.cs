using System.Collections;
using System.Xml.Serialization;
using Craft.DataStructures.IO.gml;

namespace Craft.DataStructures.IO.ogr
{
    [XmlRoot(Namespace = "http://ogr.maptools.org/")]
    public class FeatureCollection
    {
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }

        [XmlElement(Type = typeof(BoundedBy), Namespace = "http://www.opengis.net/gml/3.2", ElementName = "boundedBy"),
         XmlElement(Type = typeof(FeatureMember), ElementName = "featureMember")]
        public ArrayList FeatureCollectionElements { get; set; }
    }
}
