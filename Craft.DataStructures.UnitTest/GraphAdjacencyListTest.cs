using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Craft.DataStructures.Graph;

namespace Craft.DataStructures.UnitTest
{
    public class GraphAdjacencyListTest
    {
        [Fact]
        public void BuildAGraphAdjancencyList_Directed_UsingConstructorThatTakesListOfPredefinedVertices()
        {
            // Arrange
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Ebbe"),
                new LabelledVertex("Ana"),
                new LabelledVertex("Anton"),
                new LabelledVertex("Cecilie")
            };

            // Act
            var graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(vertices, true);
            graph.AddEdge(new LabelledEdge(0, 1, "wife"));
            graph.AddEdge(new LabelledEdge(2, 0, "parent"));
            graph.AddEdge(new LabelledEdge(2, 1, "parent"));
            graph.AddEdge(new LabelledEdge(3, 0, "parent"));
            graph.AddEdge(new LabelledEdge(3, 1, "parent"));

            // Assert
            graph.Vertices.Count.Should().Be(4);
            graph.GetAdjacentEdges(0).Count().Should().Be(1);
            graph.GetAdjacentEdges(1).Count().Should().Be(0);
            graph.GetAdjacentEdges(2).Count().Should().Be(2);
            graph.GetAdjacentEdges(3).Count().Should().Be(2);
            graph.GetAdjacentEdges(0).First().Label.Should().Be("wife");
        }

        [Fact]
        public void BuildAGraphAdjancencyList_Directed_ByAddingVerticesAfterInstantiatingTheGraph()
        {
            // Arrange
            var graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(true);

            // Act
            graph.AddVertex(new LabelledVertex("Ebbe"));
            graph.AddVertex(new LabelledVertex("Ana"));
            graph.AddVertex(new LabelledVertex("Anton"));
            graph.AddVertex(new LabelledVertex("Cecilie"));
            graph.AddEdge(new LabelledEdge(0, 1, "wife"));
            graph.AddEdge(new LabelledEdge(2, 0, "parent"));
            graph.AddEdge(new LabelledEdge(2, 1, "parent"));
            graph.AddEdge(new LabelledEdge(3, 0, "parent"));
            graph.AddEdge(new LabelledEdge(3, 1, "parent"));

            // Assert
            graph.Vertices.Count.Should().Be(4);
            graph.GetAdjacentEdges(0).Count().Should().Be(1);
            graph.GetAdjacentEdges(1).Count().Should().Be(0);
            graph.GetAdjacentEdges(2).Count().Should().Be(2);
            graph.GetAdjacentEdges(3).Count().Should().Be(2);
            graph.GetAdjacentEdges(0).First().Label.Should().Be("wife");
        }

        [Fact]
        public void RemovalOfEdges()
        {
            // Arrange
            var graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(true);

            // Act
            graph.AddVertex(new LabelledVertex("Ebbe"));
            graph.AddVertex(new LabelledVertex("Ana"));
            graph.AddVertex(new LabelledVertex("Anton"));
            graph.AddVertex(new LabelledVertex("Cecilie"));
            graph.AddEdge(new LabelledEdge(0, 1, "wife"));
            graph.AddEdge(new LabelledEdge(2, 0, "parent"));
            graph.AddEdge(new LabelledEdge(2, 1, "parent"));
            graph.AddEdge(new LabelledEdge(3, 0, "parent"));
            graph.AddEdge(new LabelledEdge(3, 1, "parent"));

            graph.RemoveEdges(0, 1);

            // Assert
            graph.Vertices.Count.Should().Be(4);
            graph.GetAdjacentEdges(0).Count().Should().Be(0);
            graph.GetAdjacentEdges(1).Count().Should().Be(0);
            graph.GetAdjacentEdges(2).Count().Should().Be(2);
            graph.GetAdjacentEdges(3).Count().Should().Be(2);
        }

        [Fact]
        public void GettingNeighbourIds()
        {
            // Arrange
            var graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(true);
            graph.AddVertex(new LabelledVertex("Ebbe"));
            graph.AddVertex(new LabelledVertex("Ana"));
            graph.AddVertex(new LabelledVertex("Anton"));
            graph.AddVertex(new LabelledVertex("Cecilie"));
            graph.AddEdge(new LabelledEdge(0, 1, "wife"));
            graph.AddEdge(new LabelledEdge(2, 0, "parent"));
            graph.AddEdge(new LabelledEdge(2, 1, "parent"));
            graph.AddEdge(new LabelledEdge(3, 0, "parent"));
            graph.AddEdge(new LabelledEdge(3, 1, "parent"));

            // Act
            var neighborIds = graph.NeighborIds(2);

            // Assert
            neighborIds.SequenceEqual(new[] {0, 1}).Should().BeTrue();
        }
    }
}
