using System;
using System.Collections.Generic;

namespace Craft.DataStructures.Graph
{
    public class GraphMatrix8Connectivity : IGraph
    {
        private int _rows;
        private int _cols;

        public int VertexCount { get; }

        public IEnumerable<int> NeighborIds(int vertexId)
        {
            throw new NotImplementedException();
        }

        public bool IsDirected => false;

        public GraphMatrix8Connectivity(
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

                if (rowIndex > 0)
                {
                    // You can go left and up
                    yield return new EdgeWithCost(vertexId, vertexId - _cols - 1, Math.Sqrt(2.0));
                }

                if (rowIndex < _rows - 1)
                {
                    // You can go left and down
                    yield return new EdgeWithCost(vertexId, vertexId + _cols - 1, Math.Sqrt(2.0));
                }
            }

            if (rowIndex > 0)
            {
                // You can go up
                yield return new EdgeWithCost(vertexId, vertexId - _cols, 1);

                // You can go right and up
                if (colIndex < _cols - 1)
                {
                    yield return new EdgeWithCost(vertexId, vertexId - _cols + 1, Math.Sqrt(2.0));
                }
            }

            if (colIndex < _cols - 1)
            {
                // You can go right
                yield return new EdgeWithCost(vertexId, vertexId + 1, 1);

                // You can go right and down
                if (rowIndex < _rows - 1)
                {
                    yield return new EdgeWithCost(vertexId, vertexId + _cols + 1, Math.Sqrt(2));
                }
            }

            if (rowIndex < _rows - 1)
            {
                // You can go down
                yield return new EdgeWithCost(vertexId, vertexId + _cols, 1);
            }
        }
    }
}