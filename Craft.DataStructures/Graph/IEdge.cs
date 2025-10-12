namespace Craft.DataStructures.Graph
{
    public interface IEdge
    {
        int VertexId1 { get; }
        int VertexId2 { get; }
    }
}