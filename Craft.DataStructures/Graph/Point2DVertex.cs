namespace Craft.DataStructures.Graph
{
    public class Point2DVertex : IVertex
    {
        public int Id { get; set; }

        public double X { get; }

        public double Y { get; }

        public Point2DVertex(
            double x, 
            double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
