using Craft.DataStructures.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Craft.DataStructures.IO;

public sealed class GraphJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
        => objectType.IsGenericType &&
           objectType.GetGenericTypeDefinition() == typeof(GraphAdjacencyList<,>);

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        var jo = JObject.Load(reader);

        var isDirected = jo["IsDirected"]?.Value<bool>()
                         ?? throw new JsonSerializationException("IsDirected missing");

        var graph = Activator.CreateInstance(
            objectType,
            new object[] { isDirected }
        ) ?? throw new JsonSerializationException(
            $"Could not create {objectType}"
        );

        serializer.Populate(jo.CreateReader(), graph);

        ((IGraphInternal)graph).RebuildInternalState();

        return graph;
    }

    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}