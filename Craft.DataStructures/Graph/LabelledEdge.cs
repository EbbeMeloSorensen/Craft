namespace Craft.DataStructures.Graph
{
    public class LabelledEdge : EmptyEdge
    {
        public string Label { get; }

        public LabelledEdge(
            int vertexId1,
            int vertexId2) : base(vertexId1, vertexId2)
        {
        }

        public LabelledEdge(
            int vertexId1,
            int vertexId2,
            string label) : base(vertexId1, vertexId2)
        {
            Label = label;
        }

        public override string ToString()
        {
            return $"{Label}";
        }
    }
}