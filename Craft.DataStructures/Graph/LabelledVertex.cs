namespace Craft.DataStructures.Graph
{
    public class LabelledVertex : EmptyVertex
    {
        public string Label { get; set; }

        public LabelledVertex(
            string label)
        {
            Label = label;
        }

        public override string ToString()
        {
            return $"{Label}";
        }
    }
}
