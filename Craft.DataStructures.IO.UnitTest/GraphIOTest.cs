using Craft.DataStructures.Graph;
using Craft.DataStructures.IO.graphml;
using Newtonsoft.Json;
using System.Xml.Serialization;
using FluentAssertions;
using Xunit;

namespace Craft.DataStructures.IO.UnitTest
{
    public class GraphIOTest
    {
        [Fact]
        public void WriteASimpleUndirectedGraphToDotFile()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(false, 5);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(0, 2, 1);
            graph.AddEdge(3, 2, 1);
            graph.AddEdge(4, 2, 1);
            graph.AddEdge(4, 0, 1);

            var outputFile = @"C:\Temp\SimpleUndirectedGraph.dot";

            // Act
            graph.WriteToFile(outputFile, Format.Dot);

            // Assert
            // (Install GraphViz and execute e.g. dot -Tsvg SimpleUndirectedGraph.dot > SimpleUndirectedGraph.svg)
        }

        [Fact]
        public void WriteASimpleDirectedGraphToDotFile()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(true, 4);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(2, 3, 1);
            graph.AddEdge(3, 0, 1);

            var outputFile = @"C:\Temp\SimpleDirectedGraph.dot";

            // Act
            graph.WriteToFile(outputFile, Format.Dot);
        }

        [Fact]
        public void ReadASimpleDirectedGraphFromDotFile()
        {
            var lines = File.ReadAllLines(@"C:\Temp\SimpleDirectedGraph.dot");
            
            foreach (var line in lines)
            {

            }
        }

        [Fact]
        public void WriteGraphAdjacencyMatrixToGraphMLFile()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(true, 4);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(2, 3, 1);
            graph.AddEdge(3, 0, 1);

            var outputFile = @"C:\Temp\GraphAdjacencyMatrix.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToJSONFile_EmptyVertexAndEmptyEdgeDirected()
        {
            // Arrange
            var vertices = Enumerable.Repeat(0, 5).Select(_ => new EmptyVertex());

            var graph = new GraphAdjacencyList<EmptyVertex, EmptyEdge>(vertices, true);

            graph.AddEdge(new EmptyEdge(0, 1));
            graph.AddEdge(new EmptyEdge(1, 2));
            graph.AddEdge(new EmptyEdge(2, 3));
            graph.AddEdge(new EmptyEdge(3, 4));
            graph.AddEdge(new EmptyEdge(4, 0));

            var outputFile = @"C:\Temp\GraphAdjacencyList_directed.json";

            // Act
            graph.WriteToFile(outputFile, Format.JSON);
        }

        [Fact]
        public void ReadGraphAdjacencyListFromJSONFile_EmptyVertexAndEmptyEdgeDirected()
        {
            var inputFile = @"C:\Temp\GraphAdjacencyList_directed.json";
            using var streamReader = new StreamReader(inputFile);
            var json = streamReader.ReadToEnd();

            // Act
            var graph = JsonConvert.DeserializeObject<GraphAdjacencyList<EmptyVertex, EmptyEdge>>(json);

            // Assert
            graph.VertexCount.Should().Be(5);
            graph.Vertices.Count(_ => _.Id == 0).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 1).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 2).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 3).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 4).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 0 && _.VertexId2 == 1).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 1 && _.VertexId2 == 2).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 2 && _.VertexId2 == 3).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 3 && _.VertexId2 == 4).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 4 && _.VertexId2 == 0).Should().Be(1);
            var outgoingEdgeFromVertex0 = graph.OutgoingEdges(0).SingleOrDefault();
            var outgoingEdgeFromVertex1 = graph.OutgoingEdges(1).SingleOrDefault();
            var outgoingEdgeFromVertex2 = graph.OutgoingEdges(2).SingleOrDefault();
            var outgoingEdgeFromVertex3 = graph.OutgoingEdges(3).SingleOrDefault();
            var outgoingEdgeFromVertex4 = graph.OutgoingEdges(4).SingleOrDefault();
            outgoingEdgeFromVertex0.Should().NotBeNull();
            outgoingEdgeFromVertex1.Should().NotBeNull();
            outgoingEdgeFromVertex2.Should().NotBeNull();
            outgoingEdgeFromVertex3.Should().NotBeNull();
            outgoingEdgeFromVertex4.Should().NotBeNull();
        }

        [Fact]
        public void WriteGraphAdjacencyListToJSONFile_LabelledVertexAndEmptyEdgeDirected()
        {
            // Arrange
            var vertices = Enumerable.Range(0, 5).Select(_ => new LabelledVertex(_.ToString()));

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(new EmptyEdge(0, 1));
            graph.AddEdge(new EmptyEdge(1, 2));
            graph.AddEdge(new EmptyEdge(2, 3));
            graph.AddEdge(new EmptyEdge(3, 4));
            graph.AddEdge(new EmptyEdge(4, 0));

            var outputFile = @"C:\Temp\GraphAdjacencyList_directed_2.json";

            // Act
            graph.WriteToFile(outputFile, Format.JSON);
        }

        [Fact]
        public void ReadGraphAdjacencyListFromJSONFile_LabelledVertexAndEmptyEdgeDirected()
        {
            var inputFile = @"C:\Temp\GraphAdjacencyList_directed_2.json";
            using var streamReader = new StreamReader(inputFile);
            var json = streamReader.ReadToEnd();

            // Act
            var graph = JsonConvert.DeserializeObject<GraphAdjacencyList<LabelledVertex, EmptyEdge>>(json);

            // Assert
            graph.VertexCount.Should().Be(5);
            graph.Vertices.Count(_ => _.Id == 0).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 1).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 2).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 3).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 4).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 0 && _.VertexId2 == 1).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 1 && _.VertexId2 == 2).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 2 && _.VertexId2 == 3).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 3 && _.VertexId2 == 4).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 4 && _.VertexId2 == 0).Should().Be(1);
            (graph.GetVertex(0) as LabelledVertex)!.Label.Should().Be("0");
            (graph.GetVertex(1) as LabelledVertex)!.Label.Should().Be("1");
            (graph.GetVertex(2) as LabelledVertex)!.Label.Should().Be("2");
            (graph.GetVertex(3) as LabelledVertex)!.Label.Should().Be("3");
            (graph.GetVertex(4) as LabelledVertex)!.Label.Should().Be("4");
            var outgoingEdgeFromVertex0 = graph.OutgoingEdges(0).SingleOrDefault();
            var outgoingEdgeFromVertex1 = graph.OutgoingEdges(1).SingleOrDefault();
            var outgoingEdgeFromVertex2 = graph.OutgoingEdges(2).SingleOrDefault();
            var outgoingEdgeFromVertex3 = graph.OutgoingEdges(3).SingleOrDefault();
            var outgoingEdgeFromVertex4 = graph.OutgoingEdges(4).SingleOrDefault();
            outgoingEdgeFromVertex0.Should().NotBeNull();
            outgoingEdgeFromVertex1.Should().NotBeNull();
            outgoingEdgeFromVertex2.Should().NotBeNull();
            outgoingEdgeFromVertex3.Should().NotBeNull();
            outgoingEdgeFromVertex4.Should().NotBeNull();
        }

        [Fact]
        public void WriteGraphAdjacencyListToJSONFile_LabelledVertexAndLabelledEdgeDirected()
        {
            // Arrange
            var vertices = Enumerable.Range(0, 5).Select(_ => new LabelledVertex(_.ToString()));

            var graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(vertices, true);

            graph.AddEdge(new LabelledEdge(0, 1, "A"));
            graph.AddEdge(new LabelledEdge(1, 2, "B"));
            graph.AddEdge(new LabelledEdge(2, 3, "C"));
            graph.AddEdge(new LabelledEdge(3, 4, "D"));
            graph.AddEdge(new LabelledEdge(4, 0, "E"));

            var outputFile = @"C:\Temp\GraphAdjacencyList_directed_3.json";

            // Act
            graph.WriteToFile(outputFile, Format.JSON);
        }

        [Fact]
        public void ReadGraphAdjacencyListFromJSONFile_LabelledVertexAndLabelledEdgeDirected()
        {
            var inputFile = @"C:\Temp\GraphAdjacencyList_directed_3.json";
            using var streamReader = new StreamReader(inputFile);
            var json = streamReader.ReadToEnd();

            // Act
            var graph = JsonConvert.DeserializeObject<GraphAdjacencyList<LabelledVertex, LabelledEdge>>(json);

            // Assert
            graph.VertexCount.Should().Be(5);
            graph.Vertices.Count(_ => _.Id == 0).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 1).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 2).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 3).Should().Be(1);
            graph.Vertices.Count(_ => _.Id == 4).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 0 && _.VertexId2 == 1).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 1 && _.VertexId2 == 2).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 2 && _.VertexId2 == 3).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 3 && _.VertexId2 == 4).Should().Be(1);
            graph.Edges.Count(_ => _.VertexId1 == 4 && _.VertexId2 == 0).Should().Be(1);
            (graph.GetVertex(0) as LabelledVertex)!.Label.Should().Be("0");
            (graph.GetVertex(1) as LabelledVertex)!.Label.Should().Be("1");
            (graph.GetVertex(2) as LabelledVertex)!.Label.Should().Be("2");
            (graph.GetVertex(3) as LabelledVertex)!.Label.Should().Be("3");
            (graph.GetVertex(4) as LabelledVertex)!.Label.Should().Be("4");
            var outgoingEdgeFromVertex0 = graph.OutgoingEdges(0).SingleOrDefault();
            var outgoingEdgeFromVertex1 = graph.OutgoingEdges(1).SingleOrDefault();
            var outgoingEdgeFromVertex2 = graph.OutgoingEdges(2).SingleOrDefault();
            var outgoingEdgeFromVertex3 = graph.OutgoingEdges(3).SingleOrDefault();
            var outgoingEdgeFromVertex4 = graph.OutgoingEdges(4).SingleOrDefault();
            outgoingEdgeFromVertex0.Should().NotBeNull();
            outgoingEdgeFromVertex1.Should().NotBeNull();
            outgoingEdgeFromVertex2.Should().NotBeNull();
            outgoingEdgeFromVertex3.Should().NotBeNull();
            outgoingEdgeFromVertex4.Should().NotBeNull();
            (outgoingEdgeFromVertex0 as LabelledEdge)?.Label.Should().Be("A");
            (outgoingEdgeFromVertex1 as LabelledEdge)?.Label.Should().Be("B");
            (outgoingEdgeFromVertex2 as LabelledEdge)?.Label.Should().Be("C");
            (outgoingEdgeFromVertex3 as LabelledEdge)?.Label.Should().Be("D");
            (outgoingEdgeFromVertex4 as LabelledEdge)?.Label.Should().Be("E");
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_EmptyVertexAndEmptyEdgeDirected()
        {
            // Arrange
            var vertices = Enumerable.Repeat(0, 5).Select(_ => new EmptyVertex());

            var graph = new GraphAdjacencyList<EmptyVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(new EmptyEdge(0, 1));
            graph.AddEdge(new EmptyEdge(1, 2));
            graph.AddEdge(new EmptyEdge(2, 3));
            graph.AddEdge(new EmptyEdge(3, 4));
            graph.AddEdge(new EmptyEdge(4, 0));

            var outputFile = @"C:\Temp\GraphAdjacencyList_directed.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_EmptyVertexAndEmptyEdgeUndirected()
        {
            // Arrange
            var vertices = Enumerable.Repeat(0, 5).Select(_ => new EmptyVertex());

            var graph = new GraphAdjacencyList<EmptyVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(new EmptyEdge(0, 1));
            graph.AddEdge(new EmptyEdge(1, 2));
            graph.AddEdge(new EmptyEdge(2, 3));
            graph.AddEdge(new EmptyEdge(3, 4));
            graph.AddEdge(new EmptyEdge(4, 0));

            var outputFile = @"C:\Temp\GraphAdjacencyList_undirected.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_LabelledVertexAndEmptyEdge_1()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Denmark"),        //  0
                new LabelledVertex("Sweden"),         //  1
                new LabelledVertex("Norway"),         //  2
                new LabelledVertex("Germany"),        //  3
                new LabelledVertex("United Kingdom"), //  4
                new LabelledVertex("Ireland"),        //  5
                new LabelledVertex("Netherlands"),    //  6
                new LabelledVertex("Belgium"),        //  7
                new LabelledVertex("France"),         //  8
                new LabelledVertex("Luxembourg"),     //  9
                new LabelledVertex("Finland"),        // 10
                new LabelledVertex("Spain"),          // 11
                new LabelledVertex("Portugal"),       // 12
                new LabelledVertex("Italy"),          // 13
                new LabelledVertex("Switzerland"),    // 14
                new LabelledVertex("Austria"),        // 15
                new LabelledVertex("Czech Republic"), // 16
                new LabelledVertex("Poland"),         // 17
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);
            graph.AddEdge(new EmptyEdge(0, 1));
            graph.AddEdge(new EmptyEdge(0, 3));
            graph.AddEdge(new EmptyEdge(1, 2));
            graph.AddEdge(new EmptyEdge(1, 10));
            graph.AddEdge(new EmptyEdge(2, 10));
            graph.AddEdge(new EmptyEdge(3, 6));
            graph.AddEdge(new EmptyEdge(3, 7));
            graph.AddEdge(new EmptyEdge(3, 8));
            graph.AddEdge(new EmptyEdge(3, 9));
            graph.AddEdge(new EmptyEdge(3, 14));
            graph.AddEdge(new EmptyEdge(3, 15));
            graph.AddEdge(new EmptyEdge(3, 16));
            graph.AddEdge(new EmptyEdge(3, 17));
            graph.AddEdge(new EmptyEdge(4, 5));
            graph.AddEdge(new EmptyEdge(6, 7));
            graph.AddEdge(new EmptyEdge(7, 8));
            graph.AddEdge(new EmptyEdge(7, 9));
            graph.AddEdge(new EmptyEdge(8, 9));
            graph.AddEdge(new EmptyEdge(8, 11));
            graph.AddEdge(new EmptyEdge(8, 13));
            graph.AddEdge(new EmptyEdge(8, 14));
            graph.AddEdge(new EmptyEdge(11, 12));
            graph.AddEdge(new EmptyEdge(13, 14));
            graph.AddEdge(new EmptyEdge(13, 15));
            graph.AddEdge(new EmptyEdge(14, 15));
            graph.AddEdge(new EmptyEdge(15, 16));
            graph.AddEdge(new EmptyEdge(16, 17));

            var outputFile = @"C:\Temp\GraphAdjacencyList_labelledVertices.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_LabelledVertexAndEmptyEdge_2()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                // North America
                new LabelledVertex("Alaska"),                //  0
                new LabelledVertex("Northwest Territory"),   //  1
                new LabelledVertex("Greenland"),             //  2
                new LabelledVertex("Alberta"),               //  3
                new LabelledVertex("Ontario"),               //  4
                new LabelledVertex("Eastern Canada"),        //  5
                new LabelledVertex("Western United States"), //  6
                new LabelledVertex("Eastern United States"), //  7
                new LabelledVertex("Central America"),       //  8

                // South America
                new LabelledVertex("Venezuela"),   //  9
                new LabelledVertex("Peru"),        // 10
                new LabelledVertex("Argentina"),   // 11
                new LabelledVertex("Brazil"),      // 12
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);

            graph.AddEdge(new EmptyEdge(0, 1));
            graph.AddEdge(new EmptyEdge(0, 3));
            graph.AddEdge(new EmptyEdge(1, 2));
            graph.AddEdge(new EmptyEdge(1, 3));
            graph.AddEdge(new EmptyEdge(1, 4));
            graph.AddEdge(new EmptyEdge(2, 4));
            graph.AddEdge(new EmptyEdge(2, 5));
            graph.AddEdge(new EmptyEdge(3, 4));
            graph.AddEdge(new EmptyEdge(3, 6));
            graph.AddEdge(new EmptyEdge(4, 5));
            graph.AddEdge(new EmptyEdge(4, 6));
            graph.AddEdge(new EmptyEdge(4, 7));
            graph.AddEdge(new EmptyEdge(5, 7));
            graph.AddEdge(new EmptyEdge(6, 7));
            graph.AddEdge(new EmptyEdge(6, 8));
            graph.AddEdge(new EmptyEdge(7, 8));
            graph.AddEdge(new EmptyEdge(8, 9));
            graph.AddEdge(new EmptyEdge(9, 10));
            graph.AddEdge(new EmptyEdge(9, 12));
            graph.AddEdge(new EmptyEdge(10, 11));
            graph.AddEdge(new EmptyEdge(10, 12));
            graph.AddEdge(new EmptyEdge(11, 12));

            var outputFile = @"C:\Temp\RISK_BoardGame.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_LabelledVertexAndEmptyEdge_Directed()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("ARNE"),                            //  0
                new LabelledVertex("Autovand"),                        //  1
                new LabelledVertex("Frie Data"),                       //  2
                new LabelledVertex("FTKImpact"),                       //  3
                new LabelledVertex("FTKmil"),                          //  4
                new LabelledVertex("Glatføre"),                        //  5
                new LabelledVertex("Handover"),                        //  6
                new LabelledVertex("InCaseIT"),                        //  7
                new LabelledVertex("Klimadatabaser"),                  //  8
                new LabelledVertex("Lyndatabaser"),                    //  9
                new LabelledVertex("METAFbrowser"),                    // 10 
                new LabelledVertex("METAF database"),                  // 11 
                new LabelledVertex("Nexus"),                           // 12 
                new LabelledVertex("Ninjo WebRequestHandler ekstern"), // 13 
                new LabelledVertex("NorthAviMet"),                     // 14 
                new LabelledVertex("OBSdb Observationsdata"),          // 15 
                new LabelledVertex("OSM"),                             // 16 
                new LabelledVertex("Pakkesystem"),                     // 17 
                new LabelledVertex("Seadb Observationsdata"),          // 18 
                new LabelledVertex("Skrivesystemet inkl split/flet"),  // 19 
                new LabelledVertex("SPOT"),                            // 20 
                new LabelledVertex("Station Management System"),       // 21
                new LabelledVertex("TAF Planner"),                     // 22 
                new LabelledVertex("Telegram"),                        // 23 
                new LabelledVertex("VAO Varsling af oversvømmelser"),  // 24 
                new LabelledVertex("VolcanoAsh"),                      // 25 
                new LabelledVertex("VolcanoAsh model"),                // 26 
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(new EmptyEdge(15, 0));
            graph.AddEdge(new EmptyEdge(8, 2));
            graph.AddEdge(new EmptyEdge(9, 2));
            graph.AddEdge(new EmptyEdge(15, 2));
            graph.AddEdge(new EmptyEdge(18, 2));
            graph.AddEdge(new EmptyEdge(21, 2));
            graph.AddEdge(new EmptyEdge(13, 3));
            graph.AddEdge(new EmptyEdge(3, 4));
            graph.AddEdge(new EmptyEdge(16, 5));
            graph.AddEdge(new EmptyEdge(16, 6));
            graph.AddEdge(new EmptyEdge(22, 7));
            graph.AddEdge(new EmptyEdge(11, 10));
            graph.AddEdge(new EmptyEdge(17, 12));
            graph.AddEdge(new EmptyEdge(1, 19));
            graph.AddEdge(new EmptyEdge(14, 20));
            graph.AddEdge(new EmptyEdge(22, 20));
            graph.AddEdge(new EmptyEdge(20, 23));
            graph.AddEdge(new EmptyEdge(0, 24));
            graph.AddEdge(new EmptyEdge(26, 25));

            var outputFile = @"C:\Temp\SystemDependencies.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        [Fact]
        public void WriteGraphAdjacencyListToGraphMLFile_LabelledVertexAndLabelledEdge()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Ebbe"),   //  0
                new LabelledVertex("Ana"),    //  1
                new LabelledVertex("Anton"),  //  2
                new LabelledVertex("Cecilie") //  3
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, true);
            graph.AddEdge(new LabelledEdge(0, 1, "spouse"));
            graph.AddEdge(new LabelledEdge(2, 0, "parent"));
            graph.AddEdge(new LabelledEdge(2, 1, "parent"));
            graph.AddEdge(new LabelledEdge(3, 0, "parent"));
            graph.AddEdge(new LabelledEdge(3, 1, "parent"));

            var outputFile = @"C:\Temp\GraphAdjacencyList_labelledVertices_2.graphml";

            // Act
            graph.WriteToFile(outputFile, Format.GraphML);
        }

        // Eksperimenteren med deserialisering af en graphml fil
        [Fact]
        public void ReadGraphAdjacencyListFromGraphMLFile()
        {
            // Arrange
            // Vi laver en serializer fuldstændigt som hvis vi skulle SKRIVE til en fil
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

            var attrOverrides = new XmlAttributeOverrides();
            attrOverrides.Add(typeof(graphml.graphml), "graphmlElements", attrs1);
            attrOverrides.Add(typeof(graph), "graphElements", attrs2);
            attrOverrides.Add(typeof(node), "nodeElements", attrs3);

            var serializer = new XmlSerializer(typeof(graphml.graphml), attrOverrides);

            var inputFileName = @"C:\Temp\GraphAdjacencyList_labelledVertices.graphml";

            var fs = new FileStream(inputFileName, FileMode.Open);
            var g = (graphml.graphml)serializer.Deserialize(fs);

            // Det kan man tilsyneladende fint - så kan vi lige så godt placere den Serializer et centralt sted
        }
    }
}