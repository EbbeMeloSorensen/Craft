namespace Craft.DataStructures.Graph
{
    public class EdgeWithCost : EmptyEdge
    {
        public double Cost { get; set; }

        public EdgeWithCost(
            int vertexId1,
            int vertexId2) : base(vertexId1, vertexId2)
        {
            Cost = 0.0;
        }

        public EdgeWithCost(
            int vertexId1,
            int vertexId2,
            double cost) : base(vertexId1, vertexId2)
        {
            Cost = cost;
        }
    }
}
