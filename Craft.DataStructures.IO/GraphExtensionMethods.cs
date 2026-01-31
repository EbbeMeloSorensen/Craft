using Craft.DataStructures.Graph;

namespace Craft.DataStructures.IO
{
    public enum Format
    {
        Dot,
        GraphML
    }

    public static class GraphExtensionMethods
    {
        private static Dictionary<int, char> _letterMap = new Dictionary<int, char>
        {
            { 0, 'a' },
            { 1, 'b' },
            { 2, 'c' },
            { 3, 'd' },
            { 4, 'e' },
            { 5, 'f' },
            { 6, 'g' },
            { 7, 'h' },
            { 8, 'i' },
            { 9, 'j' },
            { 10, 'k' },
            { 11, 'l' },
            { 12, 'm' },
            { 13, 'n' },
            { 14, 'o' },
            { 15, 'p' },
            { 16, 'q' },
            { 17, 'r' },
            { 18, 's' },
            { 19, 't' },
            { 20, 'u' },
            { 21, 'v' },
            { 22, 'w' },
            { 23, 'x' },
            { 24, 'y' },
            { 25, 'z' },
        };

        public static void WriteToFile(
            this IGraph graph,
            string outputFile,
            Format format)
        {
            switch (format)
            {
                case Format.Dot:
                    using (var streamWriter = new StreamWriter(outputFile))
                    {
                        if (graph.IsDirected)
                        {
                            streamWriter.WriteLine("digraph {");

                            for (var vertexId1 = 0; vertexId1 < graph.VertexCount; vertexId1++)
                            {
                                foreach (var edge in graph.OutgoingEdges(vertexId1))
                                {
                                    streamWriter.WriteLine($"   {_letterMap[vertexId1]} -> {_letterMap[edge.VertexId2]};");
                                }
                            }
                        }
                        else
                        {
                            streamWriter.WriteLine("graph {");

                            for (var vertexId1 = 0; vertexId1 < graph.VertexCount; vertexId1++)
                            {
                                foreach (var edge in graph.OutgoingEdges(vertexId1))
                                {
                                    if (vertexId1 < edge.VertexId2)
                                    {
                                        streamWriter.WriteLine($"   {_letterMap[vertexId1]} -- {_letterMap[edge.VertexId2]};");
                                    }
                                }
                            }
                        }

                        streamWriter.WriteLine("}");
                    }

                    break;
                case Format.GraphML:

                    DataIOHandler.SerializeGraphML(graph, outputFile);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}
