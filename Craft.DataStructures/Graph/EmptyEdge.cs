namespace Craft.DataStructures.Graph
{
    public class EmptyEdge : IEdge
    {
        public int VertexId1 { get; }
        public int VertexId2 { get; }

        public EmptyEdge(
            int vertexId1,
            int vertexId2)
        {
            VertexId1 = vertexId1;
            VertexId2 = vertexId2;
        }

        public int GetOppositeVertexId(int vertexId)
        {
            return vertexId == VertexId1 ? VertexId2 : VertexId1;
        }

        public override string ToString()
        {
            return null;
        }
    }
}
