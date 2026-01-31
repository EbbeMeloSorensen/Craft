using Xunit;
using FluentAssertions;
using Craft.DataStructures.Graph;

namespace Craft.Algorithms.UnitTest
{
    public class DijkstraShortestPathTest
    {
        [Fact]
        public void TestDijkstraAlgorithmOnGraphAdjancencyMatrix()
        {
            // Arrange
            var graph = new GraphAdjacencyMatrix(false, 3);
            graph.AddEdge(0, 1, 5);
            graph.AddEdge(0, 2, 10);
            graph.AddEdge(1, 2, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 0.0, 5, 8 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphAdjancencyList()
        {
            // Arrange
            var graph = new GraphAdjacencyList<EmptyVertex, EdgeWithCost>(false);
            graph.AddVertex(new EmptyVertex());
            graph.AddVertex(new EmptyVertex());
            graph.AddVertex(new EmptyVertex());
            graph.AddEdge(new EdgeWithCost(0, 1, 5));
            graph.AddEdge(new EdgeWithCost(0, 2, 10));
            graph.AddEdge(new EdgeWithCost(1, 2, 3));

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 0.0, 5, 8 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix4Connectivity_1SourceVertex_1()
        {
            // Arrange
            var graph = new GraphMatrix4Connectivity(3, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 0.0, 1, 2, 1, 2, 3, 2, 3, 4 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphHexMesh_1SourceVertex_1()
        {
            // Arrange
            var graph = new GraphHexMesh(3, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 0.0, 1, 2, 1, 2, 3, 2, 2, 3 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphHexMesh_1SourceVertex_2()
        {
            // Arrange
            var graph = new GraphHexMesh(3, 3);

            // Act
            graph.ComputeDistances(
                new[] { 4 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 2.0, 1, 1, 1, 0, 1, 2, 1, 1 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix4Connectivity_1SourceVertex_2()
        {
            // Arrange
            var graph = new GraphMatrix4Connectivity(3, 3);

            // Act
            graph.ComputeDistances(
                new[] { 4 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 2.0, 1, 2, 1, 0, 1, 2, 1, 2 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix4Connectivity_2SourceVertices()
        {
            // Arrange
            var graph = new GraphMatrix4Connectivity(3, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0, 8 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 0.0, 1, 2, 1, 2, 1, 2, 1, 0 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix4Connectivity_WithForbiddenVertices()
        {
            // Arrange
            var graph = new GraphMatrix4Connectivity(2, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                new HashSet<int> { 1 },
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances.SequenceEqual(new[] { 0.0, double.MaxValue, 4, 1, 2, 3 }).Should().BeTrue();
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix8Connectivity_1SourceVertex()
        {
            // Arrange
            var graph = new GraphMatrix8Connectivity(3, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances[0].Should().BeApproximately(0.0, double.Epsilon);
            distances[1].Should().BeApproximately(1.0, double.Epsilon);
            distances[2].Should().BeApproximately(2.0, double.Epsilon);
            distances[3].Should().BeApproximately(1.0, double.Epsilon);
            distances[4].Should().BeApproximately(System.Math.Sqrt(2.0), double.Epsilon);
            distances[5].Should().BeApproximately(1 + System.Math.Sqrt(2.0), double.Epsilon);
            distances[6].Should().BeApproximately(2.0, double.Epsilon);
            distances[7].Should().BeApproximately(1 + System.Math.Sqrt(2.0), double.Epsilon);
            distances[8].Should().BeApproximately(2 * System.Math.Sqrt(2.0), double.Epsilon);
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix8Connectivity_2SourceVertices()
        {
            // Arrange
            var graph = new GraphMatrix8Connectivity(3, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0, 8 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances[0].Should().BeApproximately(0.0, double.Epsilon);
            distances[1].Should().BeApproximately(1.0, double.Epsilon);
            distances[2].Should().BeApproximately(2.0, double.Epsilon);
            distances[3].Should().BeApproximately(1.0, double.Epsilon);
            distances[4].Should().BeApproximately(System.Math.Sqrt(2.0), double.Epsilon);
            distances[5].Should().BeApproximately(1.0, double.Epsilon);
            distances[6].Should().BeApproximately(2.0, double.Epsilon);
            distances[7].Should().BeApproximately(1.0, double.Epsilon);
            distances[8].Should().BeApproximately(0.0, double.Epsilon);
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix8Connectivity_WithMaxCost()
        {
            // Arrange
            var graph = new GraphMatrix8Connectivity(2, 2);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                null,
                1.2,
                out var distances,
                out var previous);

            // Assert
            distances[0].Should().BeApproximately(0.0, double.Epsilon);
            distances[1].Should().BeApproximately(1.0, double.Epsilon);
            distances[2].Should().BeApproximately(1.0, double.Epsilon);
            distances[3].Should().BeApproximately(double.MaxValue, double.Epsilon);
        }

        [Fact]
        public void ComputeDistances_Given2x3Matrix_ReturnsCorrectResult()
        {
            // Arrange
            var graph = new GraphMatrix8Connectivity(2, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                null,
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances[0].Should().BeApproximately(0.0, double.Epsilon);
            distances[1].Should().BeApproximately(1.0, double.Epsilon);
            distances[2].Should().BeApproximately(2.0, double.Epsilon);
            distances[3].Should().BeApproximately(1.0, double.Epsilon);
            distances[4].Should().BeApproximately(System.Math.Sqrt(2.0), double.Epsilon);
            distances[5].Should().BeApproximately(1.0 + System.Math.Sqrt(2.0), double.Epsilon);

            previous[0].Should().Be(-1);
            previous[1].Should().Be(0);
            previous[2].Should().Be(1);
            previous[3].Should().Be(0);
            previous[4].Should().Be(0);
            previous[5].Should().Be(1);
        }

        [Fact]
        public void TestDijkstraAlgorithmOnGraphMatrix8Connectivity_WithForbiddenVertices()
        {
            // Arrange
            var graph = new GraphMatrix8Connectivity(2, 3);

            // Act
            graph.ComputeDistances(
                new[] { 0 },
                new HashSet<int> { 1 },
                double.MaxValue,
                out var distances,
                out var previous);

            // Assert
            distances[0].Should().BeApproximately(0.0, double.Epsilon);
            distances[1].Should().BeApproximately(double.MaxValue, double.Epsilon);
            distances[2].Should().BeApproximately(2.0 * System.Math.Sqrt(2.0), double.Epsilon);
            distances[3].Should().BeApproximately(1.0, double.Epsilon);
            distances[4].Should().BeApproximately(System.Math.Sqrt(2.0), double.Epsilon);
            distances[5].Should().BeApproximately(1.0 + System.Math.Sqrt(2.0), double.Epsilon);

            previous[0].Should().Be(-1);
            previous[2].Should().Be(4);
            previous[3].Should().Be(0);
            previous[4].Should().Be(0);
            previous[5].Should().Be(4);
        }

        [Fact]
        public void DeterminePath_ReturnsCorrectResult()
        {
            // Arrange
            var previous = new int[] { -1, 0, 4, 0, 0, 4 };

            // Act
            var path = previous.DeterminePath(2);

            // Assert
            path.SequenceEqual(new int[] { 0, 4, 2 }).Should().BeTrue();
        }
    }
}
