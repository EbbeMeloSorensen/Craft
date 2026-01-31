namespace Craft.DataStructures.Graph
{
    // A state machine is a graph that has a node which is currently active.
    public class StateMachine : GraphAdjacencyList<State, EmptyEdge>
    {
        private int _currentStateIndex;

        public State CurrentState { get; private set; }

        public StateMachine(
            State initialState) : base(true)
        {
            AddState(initialState);
            CurrentState = initialState;
        }

        public StateMachine(
            IEnumerable<State> states) : base(states, true)
        {
            CurrentState = states.First();
        }

        public void AddState(
            State state)
        {
            AddVertex(state);
        }

        public void AddTransition(
            State from,
            State to)
        {
            AddEdge(new EmptyEdge(from.Id, to.Id));
        }

        public void RemoveTransition(
            State from,
            State to)
        {
            RemoveEdges(from.Id, to.Id);
        }

        public IEnumerable<string> ExitsFromCurrentState()
        {
            return OutgoingEdges(_currentStateIndex)
                .Select(_ => Vertices[_.VertexId2].Name);
        }

        public void SwitchState(
            string transitionName = null)
        {
            _currentStateIndex = transitionName == null
                ? OutgoingEdges(_currentStateIndex).Single().VertexId2
                : OutgoingEdges(_currentStateIndex).Single(_ => Vertices[_.VertexId2].Name == transitionName).VertexId2;

            CurrentState = Vertices[_currentStateIndex];
        }
    }
}
