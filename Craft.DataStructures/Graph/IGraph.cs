namespace Craft.DataStructures.Graph
{
    public interface IGraph
    {
        int VertexCount { get; }

        IVertex GetVertex(
            int vertexId);

        IEnumerable<IEdge> OutgoingEdges(
            int vertexId);

        IEnumerable<int> NeighborIds(
            int vertexId);

        bool IsDirected { get; }
    }

    // The 'out' keyword indicates that a type parameter is declared as covariant,
    // which is apparently recommended when possible. In this case, TE cannot be covariant,
    // since it constitutes an input parameter in a method of the class
    public interface IGraph<TV, TE> : IGraph 
        where TV : IVertex 
        where TE : IEdge 
    {
        List<TV> Vertices { get; }

        List<TE> Edges { get; }

        //void AddEdge(
        //    int vertexId1,
        //    int vertexId2);

        void AddEdge(
            TE edge);

        public IEnumerable<TE> GetAdjacentEdges(
            int vertexId);
    }
}
