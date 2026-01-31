using Xunit;
using FluentAssertions;

namespace Craft.DataStructures.Graph.UnitTest
{
    public class GraphMatrix4ConnectivityTest
    {
        [Fact]
        public void CheckNeigborship()
        {
            var graph = new GraphMatrix4Connectivity(4, 3);

            graph.VertexCount.Should().Be(12);
            graph.OutgoingEdges(0).Select(_ => _.VertexId2).SequenceEqual(new[] { 1, 3 }).Should().BeTrue();
            graph.OutgoingEdges(1).Select(_ => _.VertexId2).SequenceEqual(new[] { 0, 2, 4 }).Should().BeTrue();
            graph.OutgoingEdges(2).Select(_ => _.VertexId2).SequenceEqual(new[] { 1, 5 }).Should().BeTrue();
            graph.OutgoingEdges(3).Select(_ => _.VertexId2).SequenceEqual(new[] { 4, 0, 6 }).Should().BeTrue();
            graph.OutgoingEdges(4).Select(_ => _.VertexId2).SequenceEqual(new[] { 3, 5, 1, 7 }).Should().BeTrue();
            graph.OutgoingEdges(5).Select(_ => _.VertexId2).SequenceEqual(new[] { 4, 2, 8 }).Should().BeTrue();
            graph.OutgoingEdges(6).Select(_ => _.VertexId2).SequenceEqual(new[] { 7, 3, 9 }).Should().BeTrue();
            graph.OutgoingEdges(7).Select(_ => _.VertexId2).SequenceEqual(new[] { 6, 8, 4, 10 }).Should().BeTrue();
            graph.OutgoingEdges(8).Select(_ => _.VertexId2).SequenceEqual(new[] { 7, 5, 11 }).Should().BeTrue();
            graph.OutgoingEdges(9).Select(_ => _.VertexId2).SequenceEqual(new[] { 10, 6 }).Should().BeTrue();
            graph.OutgoingEdges(10).Select(_ => _.VertexId2).SequenceEqual(new[] { 9, 11, 7 }).Should().BeTrue();
            graph.OutgoingEdges(11).Select(_ => _.VertexId2).SequenceEqual(new[] { 10, 8 }).Should().BeTrue();
        }
    }
}