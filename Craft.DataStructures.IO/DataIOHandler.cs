using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Craft.DataStructures.Graph;
using Craft.DataStructures.IO.gml;
using Craft.DataStructures.IO.graphml;
using Craft.DataStructures.IO.graphml.x;
using Craft.DataStructures.IO.graphml.y;
using Craft.DataStructures.IO.graphml.yjs;
using Craft.DataStructures.IO.ogr;
using Craft.Utils.Linq;

namespace Craft.DataStructures.IO
{
    public static class DataIOHandler
    {
        private static XmlSerializer _graphmlSerializer;
        private static XmlSerializerNamespaces _graphmlSerializerNamespaces;

        private static XmlAttributeOverrides _gmlAttributeOverrides;
        private static XmlSerializerNamespaces _gmlSerializerNamespaces;

        public static void SerializeGraphML(
            IGraph graph,
            string fileName)
        {
            var graphml = ConvertGraphToGraphML(graph);

            using var writer = new StreamWriter(fileName);
            GraphmlSerializer.Serialize(writer, graphml, GraphMLSerializerNamespaces);
            writer.Close();
        }

        public static void SerializeGMLFeatureCollection(
            FeatureCollection featureCollection,
            string fileName)
        {
            using var writer = new StreamWriter(fileName);
            GenerateGmlSerializer().Serialize(writer, featureCollection, GMLSerializerNamespaces);
            writer.Close();
        }

        public static FeatureCollection DeserializeGMLFile(
            string fileName)
        {
            FeatureCollection? result;

            using (var streamReader = new StreamReader(fileName))
            {
                result = GenerateGmlSerializer().Deserialize(streamReader) as FeatureCollection;
            }

            if (result == null)
            {
                throw new InvalidDataException();
            }

            return result;
        }

        public static void ExtractGeometricPrimitivesFromGMLFile(
            string fileName,
            out List<List<double[]>> polygons)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            var xRoot = new XmlRootAttribute
            {
                ElementName = "geometryProperty",
                Namespace = "http://ogr.maptools.org/",
                IsNullable = true
            };

            var serializer = GenerateGmlSerializer(typeof(GeometryProperty), xRoot);

            var nodeList = xmlDoc.GetElementsByTagName("ogr:FeatureCollection");

            polygons = new List<List<double[]>>();

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

                    var geometryProperty = serializer.Deserialize(xmlNodeReader) as GeometryProperty;

                    switch (geometryProperty.AbstractGeometricPrimitive)
                    {
                        case Point point:
                        {
                            throw new NotImplementedException();
                        }
                        case MultiLineString multiLineString:
                        {
                            throw new NotImplementedException();
                        }
                        case MultiSurface multiSurface:
                        {
                            foreach (var surfaceMember in multiSurface.SurfaceMembers)
                            {
                                switch (surfaceMember.AbstractSurface)
                                {
                                    case Polygon polygon:
                                    {
                                        polygons.Add(polygon.Exterior.LinearRing.PosList.value
                                            .Split(' ')
                                            .Select(number => double.Parse(
                                                number, CultureInfo.InvariantCulture))
                                            .Partition(2)
                                            .ToList());

                                        break;
                                    }
                                }
                            }

                            break;
                        }
                        case AbstractCurve abstractCurve:
                        {
                            throw new NotImplementedException();
                        }
                        case AbstractSurface abstractSurface:
                        {
                            throw new NotImplementedException();
                        }
                        case AbstractSolid abstractSolid:
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        private static XmlSerializer GraphmlSerializer
        {
            get
            {
                if (_graphmlSerializer != null)
                    return _graphmlSerializer;

                var attrs1 = new XmlAttributes();
                attrs1.XmlElements.Add(new XmlElementAttribute { ElementName = "key", Type = typeof(key) });
                attrs1.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });
                attrs1.XmlElements.Add(new XmlElementAttribute { ElementName = "graph", Type = typeof(graph) });

                var attrs2 = new XmlAttributes();
                attrs2.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });
                attrs2.XmlElements.Add(new XmlElementAttribute { ElementName = "node", Type = typeof(node) });
                attrs2.XmlElements.Add(new XmlElementAttribute { ElementName = "edge", Type = typeof(edge) });

                var attrs3 = new XmlAttributes();
                attrs3.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });
                attrs3.XmlElements.Add(new XmlElementAttribute { ElementName = "port", Type = typeof(port) });

                var attrs4 = new XmlAttributes();
                attrs4.XmlElements.Add(new XmlElementAttribute { ElementName = "data", Type = typeof(data) });

                var attrOverrides = new XmlAttributeOverrides();
                attrOverrides.Add(typeof(graphml.graphml), "graphmlElements", attrs1);
                attrOverrides.Add(typeof(graph), "graphElements", attrs2);
                attrOverrides.Add(typeof(node), "nodeElements", attrs3);
                attrOverrides.Add(typeof(edge), "edgeElements", attrs4);

                _graphmlSerializer = new XmlSerializer(typeof(graphml.graphml), attrOverrides);

                return _graphmlSerializer;
            }
        }

        private static XmlSerializerNamespaces GraphMLSerializerNamespaces
        {
            get
            {
                if (_graphmlSerializerNamespaces != null)
                    return _graphmlSerializerNamespaces;

                _graphmlSerializerNamespaces = new XmlSerializerNamespaces();
                _graphmlSerializerNamespaces.Add("demostyle2", "http://www.yworks.com/yFilesHTML/demos/FlatDemoStyle/2.0");
                _graphmlSerializerNamespaces.Add("demostyle", "http://www.yworks.com/yFilesHTML/demos/FlatDemoStyle/1.0");
                _graphmlSerializerNamespaces.Add("icon-style", "http://www.yworks.com/yed-live/icon-style/1.0");
                _graphmlSerializerNamespaces.Add("bpmn", "http://www.yworks.com/xml/yfiles-bpmn/2.0");
                _graphmlSerializerNamespaces.Add("demotablestyle", "http://www.yworks.com/yFilesHTML/demos/FlatDemoTableStyle/1.0");
                _graphmlSerializerNamespaces.Add("uml", "http://www.yworks.com/yFilesHTML/demos/UMLDemoStyle/1.0");
                _graphmlSerializerNamespaces.Add("GraphvizNodeStyle", "http://www.yworks.com/yFilesHTML/graphviz-node-style/1.0");
                _graphmlSerializerNamespaces.Add("VuejsNodeStyle", "http://www.yworks.com/demos/yfiles-vuejs-node-style/1.0");
                _graphmlSerializerNamespaces.Add("explorer-style", "http://www.yworks.com/data-explorer/1.0");
                _graphmlSerializerNamespaces.Add("y", "http://www.yworks.com/xml/yfiles-common/3.0");
                _graphmlSerializerNamespaces.Add("x", "http://www.yworks.com/xml/yfiles-common/markup/3.0");
                _graphmlSerializerNamespaces.Add("yjs", "http://www.yworks.com/xml/yfiles-for-html/2.0/xaml");
                _graphmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                return _graphmlSerializerNamespaces;
            }
        }

        private static XmlAttributeOverrides GmlAttributeOverrides
        {
            get
            {
                if (_gmlAttributeOverrides != null)
                {
                    return _gmlAttributeOverrides;
                }

                var attrs1 = new XmlAttributes();

                attrs1.XmlElements.Add(new XmlElementAttribute
                {
                    ElementName = "Point",
                    Type = typeof(Point),
                    Namespace = "http://www.opengis.net/gml/3.2"
                });

                attrs1.XmlElements.Add(new XmlElementAttribute
                {
                    ElementName = "MultiSurface",
                    Type = typeof(MultiSurface),
                    Namespace = "http://www.opengis.net/gml/3.2"
                });

                attrs1.XmlElements.Add(new XmlElementAttribute
                {
                    ElementName = "MultiLineString",
                    Type = typeof(MultiLineString),
                    Namespace = "http://www.opengis.net/gml/3.2"
                });

                var attrs2 = new XmlAttributes();
                attrs2.XmlElements.Add(new XmlElementAttribute
                {
                    ElementName = "surfaceMember",
                    Type = typeof(SurfaceMember)
                });

                var attrs3 = new XmlAttributes();
                attrs3.XmlElements.Add(new XmlElementAttribute
                {
                    ElementName = "Polygon",
                    Type = typeof(Polygon),
                    Namespace = "http://www.opengis.net/gml/3.2"
                });

                var attrs4 = new XmlAttributes();
                attrs4.XmlElements.Add(new XmlElementAttribute
                {
                    ElementName = "lineStringMember",
                    Type = typeof(LineStringMember)
                });

                _gmlAttributeOverrides = new XmlAttributeOverrides();
                _gmlAttributeOverrides.Add(typeof(GeometryProperty), "AbstractGeometricPrimitive", attrs1);
                _gmlAttributeOverrides.Add(typeof(MultiSurface), "SurfaceMembers", attrs2);
                _gmlAttributeOverrides.Add(typeof(SurfaceMember), "AbstractSurface", attrs3);
                _gmlAttributeOverrides.Add(typeof(MultiLineString), "LineStringMembers", attrs4);

                return _gmlAttributeOverrides;
            }
        }

        private static XmlSerializer GenerateGmlSerializer()
        {
            return new XmlSerializer(typeof(FeatureCollection), GmlAttributeOverrides);
        }

        public static XmlSerializer GenerateGmlSerializer(
            Type type,
            XmlRootAttribute xRoot)
        {
            return new XmlSerializer(type, GmlAttributeOverrides, null, xRoot, null);
        }

        private static XmlSerializerNamespaces GMLSerializerNamespaces
        {
            get
            {
                if (_gmlSerializerNamespaces != null)
                    return _gmlSerializerNamespaces;

                _gmlSerializerNamespaces = new XmlSerializerNamespaces();
                _gmlSerializerNamespaces.Add("gml", "http://www.opengis.net/gml/3.2");
                _gmlSerializerNamespaces.Add("ogr", "http://ogr.maptools.org/");
                _gmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                return _gmlSerializerNamespaces;
            }
        }

        private static graphml.graphml ConvertGraphToGraphML(
            IGraph graph)
        {
            var graphml = new graphml.graphml
            {
                graphmlElements = new ArrayList
                {
                    new key
                    {
                        Id = "d0",
                        Domain = "node",
                        GraphMLAttributeType = "int",
                        GraphMLAttributeName = "zOrder",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-z-order/1.0/zOrder"
                    },
                    new key
                    {
                        Id = "d1",
                        Domain = "node",
                        GraphMLAttributeType = "boolean",
                        GraphMLAttributeName = "Expanded",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/Expanded",
                        @default = new @default()
                        {
                            value = "true"
                        }
                    },
                    new key
                    {
                        Id = "d2",
                        Domain = "node",
                        GraphMLAttributeType = "string",
                        GraphMLAttributeName = "url"
                    },
                    new key
                    {
                        Id = "d3",
                        Domain = "node",
                        GraphMLAttributeType = "string",
                        GraphMLAttributeName = "description"
                    },
                    new key
                    {
                        Id = "d4",
                        Domain = "node",
                        GraphMLAttributeName = "NodeLabels",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/NodeLabels"
                    },
                    new key
                    {
                        Id = "d5",
                        Domain = "node",
                        GraphMLAttributeName = "NodeGeometry",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/NodeGeometry"
                    },
                    new key
                    {
                        Id = "d6",
                        Domain = "node",
                        GraphMLAttributeName = "UserTags",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/UserTags"
                    },
                    new key
                    {
                        Id = "d7",
                        Domain = "node",
                        GraphMLAttributeName = "NodeStyle",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/NodeStyle"
                    },
                    new key
                    {
                        Id = "d8",
                        Domain = "node",
                        GraphMLAttributeName = "NodeViewState",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/1.1/NodeViewState"
                    },
                    new key
                    {
                        Id = "d9",
                        Domain = "edge",
                        GraphMLAttributeType = "string",
                        GraphMLAttributeName = "url"
                    },
                    new key
                    {
                        Id = "d10",
                        Domain = "edge",
                        GraphMLAttributeType = "string",
                        GraphMLAttributeName = "description"
                    },
                    new key
                    {
                        Id = "d11",
                        Domain = "edge",
                        GraphMLAttributeName = "EdgeLabels",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/EdgeLabels"
                    },
                    new key
                    {
                        Id = "d12",
                        Domain = "edge",
                        GraphMLAttributeName = "EdgeGeometry",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/EdgeGeometry"
                    },
                    new key
                    {
                        Id = "d13",
                        Domain = "edge",
                        GraphMLAttributeName = "EdgeStyle",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/EdgeStyle"
                    },
                    new key
                    {
                        Id = "d14",
                        Domain = "edge",
                        GraphMLAttributeName = "EdgeViewState",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/1.1/EdgeViewState"
                    },
                    new key
                    {
                        Id = "d15",
                        Domain = "port",
                        GraphMLAttributeName = "PortLabels",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/PortLabels"
                    },
                    new key
                    {
                        Id = "d16",
                        Domain = "port",
                        GraphMLAttributeName = "PortLocationParameter",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/PortLocationParameter",
                        @default = new @default
                        {
                            Static = new Static
                            {
                                Member = "y:FreeNodePortLocationModel.NodeCenterAnchored"
                            }
                        }
                    },
                    new key
                    {
                        Id = "d17",
                        Domain = "port",
                        GraphMLAttributeName = "PortStyle",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/PortStyle",
                        @default = new @default
                        {
                            Static = new Static
                            {
                                Member = "y:VoidPortStyle.Instance"
                            }
                        }
                    },
                    new key
                    {
                        Id = "d18",
                        Domain = "port",
                        GraphMLAttributeName = "PortViewState",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/folding/1.1/PortViewState"
                    },
                    new key
                    {
                        Id = "d19",
                        GraphMLAttributeName = "SharedData",
                        AttributeURI = "http://www.yworks.com/xml/yfiles-common/2.0/SharedData"
                    },
                    new data
                    {
                        key = "d19",
                        SharedData = new SharedData
                        {
                            InteriorLabelModel = new InteriorLabelModel
                            {
                                Key = 1
                            }
                        }
                    }
                }
            };

            var g = new graph
            {
                id = "G",
                edgedefault = graph.IsDirected ? edgedefault.directed : edgedefault.undirected,
                graphElements = new ArrayList
                {
                    new data
                    {
                        key = "d6",
                        Json = new Json
                        {
                            value =
                                "{\"version\":\"2.0.0\",\"__mg\":\"yed-live\",\"theme\":{\"name\":\"light\",\"version\":\"1.0.0\"}}"
                        }
                    },
                }
            };

            var edges = new List<edge>();
            var edgeId = 0;

            for (var i = 0; i < graph.VertexCount; i++)
            {
                var vertex = graph.GetVertex(i);

                g.graphElements.Add(generateNode($"n{i}", -40, -163.5, null, vertex is LabelledVertex labelledVertex ? labelledVertex.Label : null));

                edges.AddRange(graph.OutgoingEdges(i)
                    .Select(edge => generateEdge(
                        $"e{edgeId++}",
                        $"n{i}",
                        $"n{edge.VertexId2}",
                        graph.IsDirected,
                        edge is LabelledEdge edge1 ? edge1.Label : null)));
            }

            foreach (var edge in edges)
            {
                g.graphElements.Add(edge);
            }

            graphml.graphmlElements.Add(g);


            return graphml;
        }

        private static node generateNode(
            string nodeId,
            double X,
            double Y,
            int? zOrder = null,
            string? label = null)
        {
            var node = new node(nodeId);

            if (zOrder.HasValue)
            {
                node.AddZOrder(zOrder.Value);
            }

            if (label != null)
            {
                node.AddLabel(label);
            }

            node.AddGeometry(new RectD{ X = X, Y = Y, Width = 30, Height = 30});
            node.AddStyle(NodeStyle.RoundRectangle, "#FF663800", "#FFFF8C00");
            node.AddPort();

            return node;
        }

        private static edge generateEdge(
            string edgeId,
            string source,
            string target,
            bool directed,
            string? label = null)
        {
            var edge = new edge
            {
                id = edgeId,
                source = source,
                target = target,
                sourceport = "p0",
                targetport = "p0",
                edgeElements = new ArrayList()
            };

            if (label != null)
            {
                edge.edgeElements.Add(
                new data
                {
                    key = "d11",
                    List = new List
                    {
                        Label = new Label
                        {
                            LabelText = label,
                            LayoutParameter = new LayoutParameter
                            {
                                SmartEdgeLabelModelParameter = new SmartEdgeLabelModelParameter
                                {
                                    Distance = 5,
                                    SmartEdgeLabelModelParameterModel = new SmartEdgeLabelModelParameterModel
                                    {
                                        SmartEdgeLabelModel = new SmartEdgeLabelModel()
                                    }
                                }
                            },
                            Style = new Style
                            {
                                DefaultLabelStyle = new DefaultLabelStyle
                                {
                                    verticalTextAlignment = "CENTER",
                                    horizontalTextAlignment = "CENTER",
                                    textFill = "BLACK",
                                    DefaultLabelStyleFont = new DefaultLabelStyleFont
                                    {
                                        Font = new Font
                                        {
                                            fontSize = 12,
                                            fontFamily = "'Arial'"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }

            if (directed)
            {
                edge.edgeElements.Add(new data
                {
                    key = "d13",
                    PolylineEdgeStyle = new PolylineEdgeStyle
                    {
                        stroke = "#FF663800",
                        PolylineEdgeStyleTargetArrow = new PolylineEdgeStyleTargetArrow
                        {
                            Arrow = new Arrow
                            {
                                type = "TRIANGLE",
                                scale = 0.75,
                                stroke = "#FF663800",
                                fill = "#FF663800",
                                cropLength = 1
                            }
                        }
                    }
                });
            }
            else
            {
                edge.edgeElements.Add(new data
                {
                    key = "d13",
                    PolylineEdgeStyle = new PolylineEdgeStyle
                    {
                        PolylineEdgeStyleStroke = new PolylineEdgeStyleStroke
                        {
                            Stroke = new Stroke
                            {
                                fill = "#FF000000",
                                thickness = 0.75
                            }
                        }
                    }
                });
            }

            return edge;
        }
    }
}
