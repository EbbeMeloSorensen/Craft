using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Craft.DataStructures.IO.gml;
using Craft.DataStructures.IO.ogr;
using FluentAssertions;
using Xunit;

namespace Craft.DataStructures.IO.UnitTest
{
    public class GeographyIOTest
    {
        [Fact]
        public void WriteDataToGMLFile_BigCities()
        {
            // Arrange
            var featureCollection = new FeatureCollection
            {
                FeatureCollectionElements = new ArrayList
                {
                    new BoundedBy
                    {
                        Envelope = new Envelope
                        {
                            SrsName = "urn:ogc:def:crs:EPSG::4326",
                            LowerCorner = "-34.600556 -118.181926",
                            UpperCorner = "55.75411 139.749462"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.0",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "24.871938 66.988063",
                                    UpperCorner = "24.871938 66.988063"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new Point
                                {
                                    Id = "Dummy.geom.0",
                                    Position = "24.871938 66.988063"
                                }
                            },
                            Name = "Karachi"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.1",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "28.671939 77.228058",
                                    UpperCorner = "28.671939 77.228058"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new Point
                                {
                                    Id = "Dummy.geom.1",
                                    Position = "28.671939 77.228058"
                                }
                            },
                            Name = "Delhi"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.2",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "23.725006 90.406634",
                                    UpperCorner = "23.725006 90.406634"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new Point
                                {
                                    Id = "Dummy.geom.2",
                                    Position = "23.725006 90.406634"
                                }
                            },
                            Name = "Dhaka"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.3",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "40.751925 -73.981963",
                                    UpperCorner = "40.751925 -73.981963"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new Point
                                {
                                    Id = "Dummy.geom.3",
                                    Position = "40.751925 -73.981963"
                                }
                            },
                            Name = "New York"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.4",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "30.051906 31.248022",
                                    UpperCorner = "30.051906 31.248022"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new Point
                                {
                                    Id = "Dummy.geom.4",
                                    Position = "30.051906 31.248022"
                                }
                            },
                            Name = "Cairo"
                        }
                    }
                }
            };

            var outputFile = @"C:\Temp\WorldsBiggestCities.gml";

            // Act
            DataIOHandler.SerializeGMLFeatureCollection(featureCollection, outputFile);
        }

        [Fact]
        public void WriteDataToGMLFile_Luxembourg()
        {
            // Arrange
            var featureCollection = new FeatureCollection
            {
                FeatureCollectionElements = new ArrayList
                {
                    new BoundedBy
                    {
                        Envelope = new Envelope
                        {
                            SrsName = "urn:ogc:def:crs:EPSG::4326",
                            LowerCorner = "-34.600556 -118.181926",
                            UpperCorner = "55.75411 139.749462"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.0",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "49.445458984375 5.72500000000002",
                                    UpperCorner = "50.1671875 6.49375000000001"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new MultiSurface
                                {
                                    Id = "Dummy.geom.0",
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    SurfaceMembers = new List<SurfaceMember>
                                    {
                                        new SurfaceMember
                                        {
                                            AbstractSurface = new Polygon
                                            {
                                                Id = "Dummy.geom.0.0",
                                                Exterior = new Exterior
                                                {
                                                    LinearRing = new LinearRing
                                                    {
                                                        PosList = new PosList
                                                        {
                                                            value =
                                                                "50.12099609375 6.11650390625002 50.09423828125 6.10830078125002 50.034375 6.10976562500002 49.97431640625 6.13818359375 49.91513671875 6.20488281250002 49.87216796875 6.25605468750001 49.837890625 6.32460937500002 49.805322265625 6.44091796875 49.798486328125 6.4873046875 49.75439453125 6.49375000000001 49.7078125 6.48476562500002 49.68203125 6.44462890625002 49.644970703125 6.40673828125 49.599609375 6.37832031250002 49.5126953125 6.34843750000002 49.452734375 6.34433593750001 49.4775390625 6.27734375 49.4943359375 6.2421875 49.49892578125 6.18105468750002 49.485205078125 6.11992187500002 49.454638671875 6.07412109375002 49.445458984375 6.01142578125001 49.454638671875 5.95947265625 49.4775390625 5.92890625000001 49.48974609375 5.9013671875 49.505078125 5.82343750000001 49.53828125 5.78974609375001 49.55380859375 5.8154296875 49.5783203125 5.83759765625001 49.612841796875 5.85654296875001 49.644775390625 5.88037109375 49.732177734375 5.8037109375 49.75888671875 5.78798828125002 49.80830078125 5.72500000000002 49.833349609375 5.72578125000001 49.857177734375 5.74082031250001 49.875634765625 5.73525390625002 49.91962890625 5.74404296875002 49.96123046875 5.7880859375 50.0126953125 5.8173828125 50.0828125 5.86689453125001 50.1671875 5.97626953125001 50.154296875 6.05478515625001 50.15458984375 6.08906250000001 50.123779296875 6.11005859375001 50.12099609375 6.11650390625002"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Name = "Luxembourg"
                        }
                    }
                }
            };

            var outputFile = @"C:\Temp\Luxembourg.gml";

            // Act
            DataIOHandler.SerializeGMLFeatureCollection(featureCollection, outputFile);
        }

        [Fact]
        public void WriteDataToGMLFile_Denmark()
        {
            // Arrange
            var featureCollection = new FeatureCollection
            {
                FeatureCollectionElements = new ArrayList
                {
                    new BoundedBy
                    {
                        Envelope = new Envelope
                        {
                            SrsName = "urn:ogc:def:crs:EPSG::4326",
                            LowerCorner = "54.5685897308149 8.09522545663074",
                            UpperCorner = "57.7475040838931 15.1513777996996"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.0",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "54.5685897308149 8.09522545663074",
                                    UpperCorner = "57.7475040838931 15.1513777996996"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new MultiSurface
                                {
                                    Id = "Dummy.geom.0",
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    SurfaceMembers = new List<SurfaceMember>
                                    {
                                        new SurfaceMember
                                        {
                                            AbstractSurface = new Polygon
                                            {
                                                Id = "Dummy.geom.0.0",
                                                Exterior = new Exterior
                                                {
                                                    LinearRing = new LinearRing
                                                    {
                                                        PosList = new PosList
                                                        {
                                                            value =
                                                                "55.1332054686372 15.1513777996996 55.0871035865293 15.1447046234481 55.0446638033151 15.1066186859134 55.0182948742212 15.113242231028 54.9916379760827 15.0726662222297 55.0136918820187 14.9272651740024 55.0511283276872 14.7808327260851 55.101346806941 14.6841726868696 55.2358259138106 14.7092391289766 55.3092715427146 14.7609155598443 55.2481998608834 14.8514222052358 55.2157987261239 14.9267990988856 55.2161318986776 14.9733992844557 55.1884219443294 15.0004988942938 55.1332054686372 15.1513777996996"
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        new SurfaceMember
                                        {
                                            AbstractSurface = new Polygon
                                            {
                                                Id = "Dummy.geom.0.1",
                                                Exterior = new Exterior
                                                {
                                                    LinearRing = new LinearRing
                                                    {
                                                        PosList = new PosList
                                                        {
                                                            value =
                                                                "55.3454450542716 8.4764103520195 55.3397484402929 8.45167076902097 55.3800316317211 8.40235436219948 55.4274356150119 8.37094160207172 55.4632836005446 8.37435957132228 55.4684105334975 8.40821373689635 55.4387067557721 8.4119572257739 55.4274356150119 8.46338951907252 55.3933779967167 8.45020592497985 55.3454450542716 8.4764103520195"
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        new SurfaceMember
                                        {
                                            AbstractSurface = new Polygon
                                            {
                                                Id = "Dummy.geom.0.2",
                                                Exterior = new Exterior
                                                {
                                                    LinearRing = new LinearRing
                                                    {
                                                        PosList = new PosList
                                                        {
                                                            value =
                                                                "55.0302188160742 12.4171655609038 55.0023867860642 12.5376082690068 54.9571800534988 12.5517684222214 54.9600284845324 12.3483992950617 54.886175940012 12.2077742943654 54.9072938916941 12.115000834841 54.9408226232353 12.1490991514734 54.9544130667591 12.1291609952626 54.9680849698803 12.1770125600863 54.9892031271413 12.1770125593969 54.9755312377468 12.2321883388978 54.9892031271413 12.2868758403373 55.0233828810922 12.2942000661125 55.0438500009283 12.2526961598901 55.0607363938512 12.2509871748822 55.0642764355563 12.2800399101602 55.0332705745751 12.3352970709562 55.0302188160742 12.4171655609038"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Name = "Denmark"
                        }
                    }
                }
            };

            var outputFile = @"C:\Temp\Denmark.gml";

            // Act
            DataIOHandler.SerializeGMLFeatureCollection(featureCollection, outputFile);
        }

        [Fact]
        public void WriteDataToGMLFile_Line()
        {
            // Arrange
            var featureCollection = new FeatureCollection
            {
                FeatureCollectionElements = new ArrayList
                {
                    new BoundedBy
                    {
                        Envelope = new Envelope
                        {
                            SrsName = "urn:ogc:def:crs:EPSG::4326",
                            LowerCorner = "54.816337 9.995348",
                            UpperCorner = "54.971677 10.422617"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.0",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "54.816337 9.995348",
                                    UpperCorner = "54.971677 10.422617"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                AbstractGeometricPrimitive = new MultiLineString()
                                {
                                    Id = "Dummy.geom.0",
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LineStringMembers = new List<LineStringMember>
                                    {
                                        new LineStringMember()
                                        {
                                            LineString = new LineString
                                            {
                                                Coordinates = new Coordinates
                                                {
                                                    Decimal = ".",
                                                    CS = ",",
                                                    TS = " ",
                                                    value = "54.857424,9.995348 54.971677,10.205530 54.816337,10.422617"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Name = "Line"
                        }
                    }
                }
            };

            var outputFile = @"C:\Temp\Line.gml";

            // Act
            DataIOHandler.SerializeGMLFeatureCollection(featureCollection, outputFile);
        }

        // This only works when deserializing a gml file that was named "Dummy" when exported from QGIS
        [Fact]
        public void Given_GmlFileExportedFromQGIS_When_DeserializingFile_Then_ItSucceeds()
        {
            var inputFile = @".\Data\From QGIS\Dummy.gml";

            var featureCollection = DataIOHandler.DeserializeGMLFile(inputFile);

            featureCollection.FeatureCollectionElements.Count.Should().Be(2);

            (featureCollection.FeatureCollectionElements[1] is FeatureMember).Should().BeTrue();

            var featureMember = featureCollection.FeatureCollectionElements[1] as FeatureMember;
            (featureMember.Dummy.GeometryProperty.AbstractGeometricPrimitive is MultiSurface).Should().BeTrue();

            var multiSurface = featureMember.Dummy.GeometryProperty.AbstractGeometricPrimitive as MultiSurface;
            multiSurface.SurfaceMembers.Count.Should().Be(15);

            (multiSurface.SurfaceMembers[0].AbstractSurface is Polygon).Should().BeTrue();

            var polygon = multiSurface.SurfaceMembers[0].AbstractSurface as Polygon;
            polygon.Exterior.LinearRing.PosList.value.Substring(0, 10).Should().Be("55.1332054");
        }

        [Fact]
        public void Given_GmlFileSerializedFromCode_When_DeserializingFile_Then_ItSucceeds()
        {
            var inputFile = @".\Data\Serialized\Denmark.gml";

            var featureCollection = DataIOHandler.DeserializeGMLFile(inputFile);

            featureCollection.FeatureCollectionElements.Count.Should().Be(2);

            (featureCollection.FeatureCollectionElements[1] is FeatureMember).Should().BeTrue();

            var featureMember = featureCollection.FeatureCollectionElements[1] as FeatureMember;
            (featureMember.Dummy.GeometryProperty.AbstractGeometricPrimitive is MultiSurface).Should().BeTrue();

            var multiSurface = featureMember.Dummy.GeometryProperty.AbstractGeometricPrimitive as MultiSurface;
            multiSurface.SurfaceMembers.Count.Should().Be(3);

            (multiSurface.SurfaceMembers[0].AbstractSurface is Polygon).Should().BeTrue();

            var polygon = multiSurface.SurfaceMembers[0].AbstractSurface as Polygon;
            polygon.Exterior.LinearRing.PosList.value.Substring(0, 10).Should().Be("55.1332054");
        }

        [Fact]
        public void ReadDataFromGMLFileIntoXmlDocument()
        {
            var fileName = @".\Data\From QGIS\Dummy.gml";
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            var xRoot = new XmlRootAttribute
            {
                ElementName = "geometryProperty",
                Namespace = "http://ogr.maptools.org/",
                IsNullable = true
            };

            var serializer = DataIOHandler.GenerateGmlSerializer(typeof(GeometryProperty), xRoot);

            var nodeList = xmlDoc.GetElementsByTagName("ogr:FeatureCollection");

            foreach (var node in nodeList[0].ChildNodes)
            {
                // We're only interested in the feature members, i.e. not the boundedBy member
                if (!(node is XmlElement xmlElement) ||
                    xmlElement.LocalName != "featureMember")
                {
                    continue;
                }

                var geometryProperties = xmlElement.GetElementsByTagName("ogr:geometryProperty");

                foreach (var temp in geometryProperties)
                {
                    using var xmlNodeReader = new XmlNodeReader(temp as XmlNode);

                    // Act
                    var geometryProperty = serializer.Deserialize(xmlNodeReader) as GeometryProperty;

                    // Assert
                    (geometryProperty.AbstractGeometricPrimitive is MultiSurface).Should().BeTrue();

                    var multiSurface = geometryProperty.AbstractGeometricPrimitive as MultiSurface;
                    multiSurface.SurfaceMembers.Count.Should().Be(15);

                    (multiSurface.SurfaceMembers[0].AbstractSurface is Polygon).Should().BeTrue();

                    var polygon = multiSurface.SurfaceMembers[0].AbstractSurface as Polygon;
                    var posListText = polygon.Exterior.LinearRing.PosList.value;
                    posListText.Substring(0, 10).Should().Be("55.1332054");
                }
            }
        }

        [Fact]
        public void ParseAGMLFileExportedFromQGIS_1()
        {
            var fileName = @".\Data\From QGIS\Dummy.gml";
            DataIOHandler.ExtractGeometricPrimitivesFromGMLFile(
                fileName, out var polygons);

            polygons.Count.Should().Be(15);
            polygons[12].Count.Should().Be(306);
        }

        [Fact]
        public void ParseAGMLFileExportedFromQGIS_2()
        {
            var fileName = @".\Data\From QGIS\MultipleCountries.gml";
            DataIOHandler.ExtractGeometricPrimitivesFromGMLFile(
                fileName, out var polygons);

            polygons.Count.Should().Be(14);
            polygons[12].Count.Should().Be(194);
        }
    }
}