namespace Craft.DataStructures.Graph
{
    public class GraphHexMesh : IGraph
    {
        private int _rows;
        private int _cols;

        public int VertexCount { get; }

        public IEnumerable<int> NeighborIds(int vertexId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDirected => false;

        public GraphHexMesh(
            int rows,
            int columns)
        {
            _rows = rows;
            _cols = columns;
            VertexCount = rows * columns;
        }

        public IVertex GetVertex(
            int vertexId)
        {
            return new EmptyVertex();
        }

        public IEnumerable<IEdge> OutgoingEdges(
            int vertexId)
        {
            var rowIndex = vertexId / _cols;
            var colIndex = vertexId % _cols;

            if (colIndex > 0)
            {
                // You can go left
                yield return new EdgeWithCost(vertexId, vertexId - 1, 1);
            }

            if (colIndex < _cols - 1)
            {
                // You can go right
                yield return new EdgeWithCost(vertexId, vertexId + 1, 1);
            }

            if (colIndex > 0 || rowIndex % 2 == 1)
            {
                if (rowIndex > 0)
                {
                    // You can go to the upper left
                    yield return new EdgeWithCost(vertexId, vertexId - _cols - (rowIndex + 1) % 2, 1);
                }

                if (rowIndex < _rows - 1)
                {
                    // You can go to the lower left
                    yield return new EdgeWithCost(vertexId, vertexId + _cols - (rowIndex + 1) % 2, 1);
                }
            }

            if (colIndex < _cols - 1 || rowIndex % 2 == 0)
            {
                if (rowIndex > 0)
                {
                    // You can go to the upper right
                    yield return new EdgeWithCost(vertexId, vertexId - _cols + 1 - (rowIndex + 1) % 2, 1);
                }

                if (rowIndex < _rows - 1)
                {
                    // You can go to the lower right
                    yield return new EdgeWithCost(vertexId, vertexId + _cols + 1 - (rowIndex + 1) % 2, 1);
                }
            }
        }
    }
}
