using Craft.DataStructures.Graph;

namespace Craft.UIElements.GuiTest.Tab5;

public static class GraphGenerator
{
    public static IGraph<LabelledVertex, EmptyEdge> GenerateGraphOfEuropeanCountries()
    {
        var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Denmark"),        //  0
                new LabelledVertex("Sweden"),         //  1
                new LabelledVertex("Norway"),         //  2
                new LabelledVertex("Germany"),        //  3
                new LabelledVertex("United Kingdom"), //  4
                new LabelledVertex("Ireland"),        //  5
                new LabelledVertex("Netherlands"),    //  6
                new LabelledVertex("Belgium"),        //  7
                new LabelledVertex("France"),         //  8
                new LabelledVertex("Luxembourg"),     //  9
                new LabelledVertex("Finland"),        // 10
                new LabelledVertex("Spain"),          // 11
                new LabelledVertex("Portugal"),       // 12
                new LabelledVertex("Italy"),          // 13
                new LabelledVertex("Switzerland"),    // 14
                new LabelledVertex("Austria"),        // 15
                new LabelledVertex("Czech Republic"), // 16
                new LabelledVertex("Poland"),         // 17
            };

        var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);

        graph.AddEdge(new EmptyEdge(0, 1));
        graph.AddEdge(new EmptyEdge(0, 3));
        graph.AddEdge(new EmptyEdge(1, 2));
        graph.AddEdge(new EmptyEdge(1, 10));
        graph.AddEdge(new EmptyEdge(2, 10));
        graph.AddEdge(new EmptyEdge(3, 6));
        graph.AddEdge(new EmptyEdge(3, 7));
        graph.AddEdge(new EmptyEdge(3, 8));
        graph.AddEdge(new EmptyEdge(3, 9));
        graph.AddEdge(new EmptyEdge(3, 14));
        graph.AddEdge(new EmptyEdge(3, 15));
        graph.AddEdge(new EmptyEdge(3, 16));
        graph.AddEdge(new EmptyEdge(3, 17));
        graph.AddEdge(new EmptyEdge(4, 5));
        graph.AddEdge(new EmptyEdge(6, 7));
        graph.AddEdge(new EmptyEdge(7, 8));
        graph.AddEdge(new EmptyEdge(7, 9));
        graph.AddEdge(new EmptyEdge(8, 9));
        graph.AddEdge(new EmptyEdge(8, 11));
        graph.AddEdge(new EmptyEdge(8, 13));
        graph.AddEdge(new EmptyEdge(8, 14));
        graph.AddEdge(new EmptyEdge(11, 12));
        graph.AddEdge(new EmptyEdge(13, 14));
        graph.AddEdge(new EmptyEdge(13, 15));
        graph.AddEdge(new EmptyEdge(14, 15));
        graph.AddEdge(new EmptyEdge(15, 16));
        graph.AddEdge(new EmptyEdge(16, 17));

        return graph;
    }

    public static IGraph<LabelledVertex, EmptyEdge> GenerateGraphOfRiskTerritories()
    {
        var vertices = new List<LabelledVertex>
        {
            // North America
            new LabelledVertex("Alaska"),                //  0
            new LabelledVertex("Northwest Territory"),   //  1
            new LabelledVertex("Greenland"),             //  2
            new LabelledVertex("Alberta"),               //  3
            new LabelledVertex("Ontario"),               //  4
            new LabelledVertex("Quebec"),                //  5
            new LabelledVertex("Western United States"), //  6
            new LabelledVertex("Eastern United States"), //  7
            new LabelledVertex("Central America"),       //  8

            // South America
            new LabelledVertex("Venezuela"),   //  9
            new LabelledVertex("Peru"),        // 10
            new LabelledVertex("Argentina"),   // 11
            new LabelledVertex("Brazil"),      // 12

            // Europe
            new LabelledVertex("Iceland"),         // 13
            new LabelledVertex("Scandinavia"),     // 14
            new LabelledVertex("Great Britain"),   // 15
            new LabelledVertex("Northern Europe"), // 16
            new LabelledVertex("Ukraine"),         // 17
            new LabelledVertex("Western Europe"),  // 18
            new LabelledVertex("Southern Europe"), // 19

            // Africa
            new LabelledVertex("North Africa"), // 20
            new LabelledVertex("Egypt"),        // 21
            new LabelledVertex("East Africa"),  // 22
            new LabelledVertex("Congo"),        // 23
            new LabelledVertex("South Africa"), // 24
            new LabelledVertex("Madagascar"),   // 25

            // Asia
            new LabelledVertex("Siberia"),     // 26
            new LabelledVertex("Ural"),        // 27
            new LabelledVertex("Yakutsk"),     // 28
            new LabelledVertex("Kamchatka"),   // 29
            new LabelledVertex("Irkutsk"),     // 30
            new LabelledVertex("Afghanistan"), // 31
            new LabelledVertex("Mongolia"),    // 32
            new LabelledVertex("Japan"),       // 33
            new LabelledVertex("China"),       // 34
            new LabelledVertex("Middle East"), // 35
            new LabelledVertex("India"),       // 36
            new LabelledVertex("Siam"),        // 37

            // Oceania
            new LabelledVertex("Indonesia"),         // 38
            new LabelledVertex("New Guinea"),        // 39
            new LabelledVertex("Western Australia"), // 40
            new LabelledVertex("Eastern Australia"), // 41
        };

        var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);

        graph.AddEdge(new EmptyEdge(0, 1));
        graph.AddEdge(new EmptyEdge(0, 3));
        graph.AddEdge(new EmptyEdge(0, 29));
        graph.AddEdge(new EmptyEdge(1, 2));
        graph.AddEdge(new EmptyEdge(1, 3));
        graph.AddEdge(new EmptyEdge(1, 4));
        graph.AddEdge(new EmptyEdge(2, 4));
        graph.AddEdge(new EmptyEdge(2, 5));
        graph.AddEdge(new EmptyEdge(2, 13));
        graph.AddEdge(new EmptyEdge(3, 4));
        graph.AddEdge(new EmptyEdge(3, 6));
        graph.AddEdge(new EmptyEdge(4, 5));
        graph.AddEdge(new EmptyEdge(4, 6));
        graph.AddEdge(new EmptyEdge(4, 7));
        graph.AddEdge(new EmptyEdge(5, 7));
        graph.AddEdge(new EmptyEdge(6, 7));
        graph.AddEdge(new EmptyEdge(6, 8));
        graph.AddEdge(new EmptyEdge(7, 8));
        graph.AddEdge(new EmptyEdge(8, 9));
        graph.AddEdge(new EmptyEdge(9, 10));
        graph.AddEdge(new EmptyEdge(9, 12));
        graph.AddEdge(new EmptyEdge(10, 11));
        graph.AddEdge(new EmptyEdge(10, 12));
        graph.AddEdge(new EmptyEdge(11, 12));
        graph.AddEdge(new EmptyEdge(12, 20));
        graph.AddEdge(new EmptyEdge(13, 14));
        graph.AddEdge(new EmptyEdge(13, 15));
        graph.AddEdge(new EmptyEdge(14, 15));
        graph.AddEdge(new EmptyEdge(14, 16));
        graph.AddEdge(new EmptyEdge(14, 17));
        graph.AddEdge(new EmptyEdge(15, 16));
        graph.AddEdge(new EmptyEdge(15, 18));
        graph.AddEdge(new EmptyEdge(16, 17));
        graph.AddEdge(new EmptyEdge(16, 18));
        graph.AddEdge(new EmptyEdge(16, 19));
        graph.AddEdge(new EmptyEdge(17, 19));
        graph.AddEdge(new EmptyEdge(18, 19));
        graph.AddEdge(new EmptyEdge(18, 20));
        graph.AddEdge(new EmptyEdge(19, 20));
        graph.AddEdge(new EmptyEdge(19, 21));
        graph.AddEdge(new EmptyEdge(19, 35));
        graph.AddEdge(new EmptyEdge(17, 27));
        graph.AddEdge(new EmptyEdge(17, 31));
        graph.AddEdge(new EmptyEdge(17, 35));
        graph.AddEdge(new EmptyEdge(20, 21));
        graph.AddEdge(new EmptyEdge(20, 22));
        graph.AddEdge(new EmptyEdge(20, 23));
        graph.AddEdge(new EmptyEdge(21, 22));
        graph.AddEdge(new EmptyEdge(21, 35));
        graph.AddEdge(new EmptyEdge(22, 23));
        graph.AddEdge(new EmptyEdge(22, 24));
        graph.AddEdge(new EmptyEdge(22, 25));
        graph.AddEdge(new EmptyEdge(22, 35));
        graph.AddEdge(new EmptyEdge(23, 24));
        graph.AddEdge(new EmptyEdge(24, 25));
        graph.AddEdge(new EmptyEdge(26, 27));
        graph.AddEdge(new EmptyEdge(26, 28));
        graph.AddEdge(new EmptyEdge(26, 30));
        graph.AddEdge(new EmptyEdge(26, 32));
        graph.AddEdge(new EmptyEdge(26, 34));
        graph.AddEdge(new EmptyEdge(27, 31));
        graph.AddEdge(new EmptyEdge(27, 34));
        graph.AddEdge(new EmptyEdge(28, 29));
        graph.AddEdge(new EmptyEdge(28, 30));
        graph.AddEdge(new EmptyEdge(29, 30));
        graph.AddEdge(new EmptyEdge(29, 32));
        graph.AddEdge(new EmptyEdge(29, 33));
        graph.AddEdge(new EmptyEdge(30, 32));
        graph.AddEdge(new EmptyEdge(31, 34));
        graph.AddEdge(new EmptyEdge(31, 35));
        graph.AddEdge(new EmptyEdge(31, 36));
        graph.AddEdge(new EmptyEdge(32, 33));
        graph.AddEdge(new EmptyEdge(32, 34));
        graph.AddEdge(new EmptyEdge(34, 36));
        graph.AddEdge(new EmptyEdge(34, 37));
        graph.AddEdge(new EmptyEdge(35, 36));
        graph.AddEdge(new EmptyEdge(36, 37));
        graph.AddEdge(new EmptyEdge(37, 38));
        graph.AddEdge(new EmptyEdge(38, 39));
        graph.AddEdge(new EmptyEdge(38, 40));
        graph.AddEdge(new EmptyEdge(39, 40));
        graph.AddEdge(new EmptyEdge(39, 41));
        graph.AddEdge(new EmptyEdge(40, 41));

        return graph;
    }
}