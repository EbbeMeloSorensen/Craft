namespace Craft.DataStructures.Graph
{
    // This is a simple graph class that represents either a directed or an undirected graph.
    // It stores the graph info as a matrix where a matrix entry indicates the "cost" of getting
    // from one vertex to another. A cost of 0 indicates that there is no edge between two vertices.
    // The class is mostly suitable for graphs with a small number of vertices.
    public class GraphAdjacencyMatrix : IGraph
    {
        protected bool _directed;
        protected double[,] _adjacencyMatrix;
        protected int _vertexCount;

        public int VertexCount
        {
            get { return _vertexCount; }
        }

        public IEnumerable<int> NeighborIds(int vertexId)
        {
            throw new NotImplementedException();
        }

        public bool IsDirected => _directed;

        public GraphAdjacencyMatrix(
            bool directed,
            int numberOfVertices)
        {
            _directed = directed;
            _adjacencyMatrix = new double[numberOfVertices, numberOfVertices];
            _vertexCount = numberOfVertices;
        }

        public void AddEdge(
            int vertexId1,
            int vertexId2,
            double cost)
        {
            _adjacencyMatrix[vertexId1, vertexId2] = cost;

            if (!_directed)
            {
                _adjacencyMatrix[vertexId2, vertexId1] = cost;
            }
        }

        public IVertex GetVertex(
            int vertexId)
        {
            return new EmptyVertex();
        }

        public IEnumerable<IEdge> OutgoingEdges(
            int vertexId)
        {
            for (var i = 0; i < VertexCount; i++)
            {
                var cost = _adjacencyMatrix[vertexId, i];

                if (cost > 0)
                {
                    yield return new EdgeWithCost(vertexId, i, cost);
                }
            }
        }
    }
}