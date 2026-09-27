using Craft.IO.Utils;
using Craft.Math;
using Newtonsoft.Json;
using System.Collections;
using System.IO;

namespace Craft.UIElements.Reborn.GuiTest;

internal static class GeometryFile
{
    public static string Serialize(IEnumerable geometry)
    {
        var objects = new List<StoredObject>();
        foreach (var item in geometry)
        {
            objects.Add(item switch
            {
                Point2D point => new StoredObject { Point = point },
                LineSegment2D segment => new StoredObject
                {
                    Point1 = segment.Point1,
                    Point2 = segment.Point2,
                    IsArrow = segment is Arrow2D
                },
                _ => throw new InvalidDataException("Unsupported geometry type.")
            });
        }

        return JsonConvert.SerializeObject(objects, Formatting.Indented, new DoubleJsonConverter());
    }

    public static IReadOnlyList<object> Deserialize(string json)
    {
        var storedObjects = JsonConvert.DeserializeObject<List<StoredObject>>(json)
            ?? throw new InvalidDataException("Expected a geometry array.");
        var result = new List<object>();
        foreach (var stored in storedObjects)
        {
            if (stored == null)
                throw new InvalidDataException("Geometry entries cannot be null.");

            if (stored.Point != null && stored.Point1 == null && stored.Point2 == null && !stored.IsArrow)
                result.Add(stored.Point);
            else if (stored.Point == null && stored.Point1 != null && stored.Point2 != null)
                result.Add(stored.IsArrow
                    ? new Arrow2D(stored.Point1, stored.Point2)
                    : new LineSegment2D(stored.Point1, stored.Point2));
            else
                throw new InvalidDataException("Expected a point or two segment endpoints.");
        }

        return result;
    }

    private sealed class StoredObject
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Point2D? Point { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Point2D? Point1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Point2D? Point2 { get; set; }
        public bool IsArrow { get; set; }
    }
}
