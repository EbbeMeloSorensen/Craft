namespace Craft.DataStructures.Graph
{
    public class State : LabelledVertex
    {
        public string Name => Label;

        public State(
            string name) : base(name)
        {
        }
    }
}