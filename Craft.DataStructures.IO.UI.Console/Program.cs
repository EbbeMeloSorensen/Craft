using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;

void ParseDotFile(
    string dotFile,
    out GraphAdjacencyList<LabelledVertex, EmptyEdge> graph)
{
    var lines = File.ReadAllLines(dotFile);

    var graphType = lines.First().Split(' ').First();
    Console.WriteLine(graphType);

    graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(true);
    var dictionary = new Dictionary<string, int>();

    foreach (var line in lines.Skip(1))
    {
        if (line.Contains('}'))
        {
            break;
        }

        Console.WriteLine(line);

        var temp = line.TrimStart().TrimEnd(';').Split(' ');

        var sourceNodeName = temp.First();
        var targetNodeName = temp.Last();

        if (!dictionary.ContainsKey(sourceNodeName))
        {
            var vertex = new LabelledVertex(sourceNodeName);
            graph.AddVertex(vertex);
            dictionary[sourceNodeName] = vertex.Id;
        }

        if (!dictionary.ContainsKey(targetNodeName))
        {
            var vertex = new LabelledVertex(targetNodeName);
            graph.AddVertex(vertex);
            dictionary[targetNodeName] = vertex.Id;
        }

        var idOfSourceVertex = dictionary[sourceNodeName];
        var idOfTargetVertex = dictionary[targetNodeName];

        graph.AddEdge(new EmptyEdge(idOfSourceVertex, idOfTargetVertex));
    }
}

void CollectGraphDataFromDotFile(
    string dotFile,
    HashSet<string> vertexLabels,
    List<Tuple<string, string>> edgeData)
{
    var lines = File.ReadAllLines(dotFile);

    foreach (var line in lines.Skip(1))
    {
        if (line.Contains('}'))
        {
            break;
        }

        if (line.Contains(" -> "))
        {
            var temp = line.TrimStart().TrimEnd(';').Split(' ');
            var sourceVertexLabel = temp.First();
            var targetVertexLabel = temp.Last();
            vertexLabels.Add(sourceVertexLabel);
            vertexLabels.Add(targetVertexLabel);
            edgeData.Add(new Tuple<string, string>(sourceVertexLabel, targetVertexLabel));
        }
        else
        {
            var vertexLabel = line.TrimStart().TrimEnd(';').Split(' ').Single();
            vertexLabels.Add(vertexLabel);
        }
    }
}

string StripFromEnd(
    string text,
    string end)
{
    if (text.EndsWith(end))
    {
        return text.Substring(0, text.Length - end.Length);
    }

    return text;
}

string ExtractACIName(
    string epgName)
{
    if (epgName.StartsWith("AVINT"))
    {
        var a = 0;
    }

    epgName = StripFromEnd(epgName, "DEV");
    epgName = StripFromEnd(epgName, "-dev");
    epgName = StripFromEnd(epgName, "dev");

    epgName = StripFromEnd(epgName, "TEST");
    epgName = StripFromEnd(epgName, "-test");
    epgName = StripFromEnd(epgName, "test");
    epgName = StripFromEnd(epgName, "tst");

    epgName = StripFromEnd(epgName, "PROD");
    epgName = StripFromEnd(epgName, "-prod");
    epgName = StripFromEnd(epgName, "prod");

    epgName = StripFromEnd(epgName, "-staging");

    epgName = StripFromEnd(epgName, "INT");
    epgName = StripFromEnd(epgName, "INTDB");
    epgName = StripFromEnd(epgName, "APPS");
    //epgName = StripFromEnd(epgName, "DB");

    return epgName;
}

var vertexLabels = new HashSet<string>();
var edgeData = new List<Tuple<string, string>>();

// Windows
var useLinux = false;

var rootDirectory = useLinux
    //? "/home/ebs/Git/enterprise-architecture/catalogs_generated/aci_dot_files"
    ? "/home/ebs/Git/enterprise-architecture/catalogs_generated/dot_files_from_marc"
    //: "C:\\Users\\B053687\\Git\\DMI_Gitlab\\enterprise-architecture\\catalogs_generated\\aci_dot_files";
    : "C:\\Users\\B053687\\Git\\DMI_Gitlab\\enterprise-architecture\\catalogs\\external";

var outputFile = useLinux
    ? "aci_topoplogy_marc.graphml"
    : @"C:\Temp\Take1.graphml";

var directory = new DirectoryInfo(rootDirectory);

Console.WriteLine($"Parsing yml files:");

foreach (var file in directory.GetFiles())
{
    if (file.Extension != ".dot")
    {
        continue;
    }

    Console.WriteLine($"  {file.Name}");
    CollectGraphDataFromDotFile(file.FullName, vertexLabels, edgeData);
}

var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(true);
var vertexIdMap = new Dictionary<string, int>();

foreach (var vertexLabel in vertexLabels)
{
    var vertex = new LabelledVertex(vertexLabel);
    graph.AddVertex(vertex);
    vertexIdMap[vertexLabel] = vertex.Id; 
}

foreach (var item in edgeData)
{
    var vertexId1 = vertexIdMap[item.Item1];
    var vertexId2 = vertexIdMap[item.Item2];

    if (vertexId1 == vertexId2)
    {
        Console.WriteLine($"    WARNING: {item.Item1} apparently connects to itself...");
    }

    graph.AddEdge((new EmptyEdge(vertexId1, vertexId2)));
}

// Kollaps grafen i en mindre graf, hvor man bare ser relationer mellem ACI-navne

// Identificer ACI-navne og lav en mapping fra EPG-navn til ACI-navn
var aciNames = new HashSet<string>();

graph.Vertices.ForEach(v =>
{
    var epgName = v.Label.Replace("\"", "");

    if (epgName.StartsWith("ARNE"))
    {
        var a = 0;
    }

    var aciName = ExtractACIName(epgName);

    if (!aciNames.Contains(aciName))
    {
        aciNames.Add(aciName);
    }
});

Console.WriteLine("ACI Names:");
aciNames.OrderBy(_ => _).ToList().ForEach(_ =>
{
    Console.WriteLine(_);
});

File.WriteAllLines(@"C:\Temp\ACINames.txt", aciNames);

Console.WriteLine($"Wrinting {outputFile}..");
graph.WriteToFile(outputFile, Format.GraphML);
Console.WriteLine("Done");

if (false)
{
    ParseDotFile(@"C:\Temp\SimpleDirectedGraph.dot", out var graph2);
    graph.WriteToFile(@"C:\Temp\Inferno.graphml", Format.GraphML);
    Console.WriteLine("Done");
}
