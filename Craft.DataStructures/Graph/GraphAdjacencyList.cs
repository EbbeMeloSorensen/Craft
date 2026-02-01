using System.Reflection;
using System.Runtime.Serialization;

namespace Craft.DataStructures.Graph
{
    // This is a template class for representing graphs where we want to associate some information
    // with vertices and/or edges such as a location of a vertex.
    // This class is suitable for graphs with many vertices.

    public class GraphAdjacencyList<TV, TE> : IGraph<TV, TE> 
        where TV : IVertex
        where TE : IEdge
    {
        private static readonly ConstructorInfo _edgeConstructorInfo;
        private List<Tuple<int, TE>>[] _adjacencyList; // Todo: Udfordr lige dit rationale for at gøre det sådan her i stedet for at bruge et dictionary
                                                       // Bemærk, at den altså ER et array (af lister) - du har nok tænkt, at det har en god performance..
        private int _vertexCount;

        public bool IsDirected { get; }

        public int VertexCount => _vertexCount;

        public List<TV> Vertices { get; private set; }

        public List<TE> Edges { get; private set; }

        static GraphAdjacencyList()
        {
            _edgeConstructorInfo = typeof(TE).GetConstructor(new[] { typeof(int), typeof(int) });

            if (_edgeConstructorInfo == null)
            {
                throw new InvalidOperationException("Edge class doesn't have a constructor that takes two integer arguments");
            }
        }

        public GraphAdjacencyList() : this(false)
        {
        }

        public GraphAdjacencyList(
            bool directed)
        {
            IsDirected = directed;
            Vertices = new List<TV>();
            Edges = new List<TE>();

            _adjacencyList = Array.Empty<List<Tuple<int, TE>>>();
        }

        public GraphAdjacencyList(
            IEnumerable<TV> vertices,
            bool directed)
        {
            IsDirected = directed;
            Vertices = vertices.ToList();
            _vertexCount = Vertices.Count;

            // Traverse vertices and assign each of them a unique Id
            for (var i = 0; i < _vertexCount; i++)
            {
                Vertices[i].Id = i;
            }

            Edges = new List<TE>();
            _adjacencyList = new List<Tuple<int, TE>>[_vertexCount];
        }

        public void AddVertex(
            TV vertex)
        {
            vertex.Id = _vertexCount++;
            Vertices.Add(vertex);

            // I og med at _adjacencyList er et array, er dette næppe særligt hensigtsmæssigt
            _adjacencyList = _adjacencyList.Append(new List<Tuple<int, TE>>()).ToArray();
        }

        public void AddEdge(
            int vertexId1,
            int vertexId2)
        {
            var edge = (TE) _edgeConstructorInfo.Invoke(new object[] { vertexId1, vertexId2 });
            Edges.Add(edge);

            if (_adjacencyList[vertexId1] == null)
            {
                _adjacencyList[vertexId1] = new List<Tuple<int, TE>>();
            }

            _adjacencyList[vertexId1].Add(new Tuple<int, TE>(vertexId2, edge));

            if (IsDirected)
            {
                return;
            }

            if (_adjacencyList[vertexId2] == null)
            {
                _adjacencyList[vertexId2] = new List<Tuple<int, TE>>();
            }

            _adjacencyList[vertexId2].Add(new Tuple<int, TE>(vertexId1, edge));
        }

        public void AddEdge(
            TE edge)
        {
            Edges.Add(edge);
            AddEdgeToAdjacencyList(edge);
        }

        public void RemoveEdges(
            int vertexId1,
            int vertexId2)
        {
            if (_adjacencyList[vertexId1] != null)
            {
                _adjacencyList[vertexId1] = _adjacencyList[vertexId1]
                    .Where(_ => _.Item1 != vertexId2)
                    .ToList();

                Edges = Edges
                    .Where(_ => _.VertexId1 == vertexId1 && _.VertexId2 == vertexId2)
                    .ToList();
            }
        }

        // Det her er ikke særligt pænt... Du skal kunne opdatere en vertex i grafen uden at fucke vertex id'er op
        // Husk de tildeles i constructoren
        // Hvad bruger vi overhovedet den her til? -> Den kaldes af MovePoint i en gui test.. så den bør nok fjernes og erstattes med noget andet
        public void UpdateVertex(
            int vertexId, 
            TV vertex)
        {
            vertex.Id = vertexId;
            Vertices[vertexId] = vertex;
        }

        public IVertex GetVertex(
            int vertexId)
        {
            return Vertices[vertexId];
        }

        public IEnumerable<IEdge> OutgoingEdges(
            int vertexId)
        {
            if (_adjacencyList[vertexId] == null)
            {
                return new List<IEdge>();
            }

            return (IEnumerable<IEdge>)_adjacencyList[vertexId].Select(_ => _.Item2).Where(_ => _.VertexId1 == vertexId);
        }

        // Returns the edges that go to or from a given vertex
        public IEnumerable<TE> GetAdjacentEdges(
            int vertexId)
        {
            if (_adjacencyList[vertexId] == null)
            {
                return new List<TE>();
            }

            return _adjacencyList[vertexId].Select(_ => _.Item2);
        }

        public IEnumerable<int> NeighborIds(
            int vertexId)
        {
            return GetAdjacentEdges(vertexId)
                .Select(_ => _.VertexId1 == vertexId ? _.VertexId2 : _.VertexId1);
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RebuildInternalState();
        }

        private void AddEdgeToAdjacencyList(
            TE edge)
        {
            if (_adjacencyList[edge.VertexId1] == null)
            {
                _adjacencyList[edge.VertexId1] = new List<Tuple<int, TE>>();
            }

            _adjacencyList[edge.VertexId1].Add(new Tuple<int, TE>(edge.VertexId2, edge));
        }

        private void RebuildInternalState()
        {
            _vertexCount = Vertices.Count;
            Edges.ForEach(AddEdgeToAdjacencyList);
        }
    }
}