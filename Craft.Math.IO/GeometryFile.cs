using System.Collections;
using Newtonsoft.Json;
using Craft.IO.Utils;

namespace Craft.Math.IO;

public static class GeometryFile
{
    public static string Serialize(IEnumerable geometry)
    {
        var objects = new List<StoredObject>();
        foreach (var item in geometry)
        {
            objects.Add(item switch
            {
                OrientedPoint2D point => new StoredObject
                {
                    Point = new Point2D(point.X, point.Y),
                    AngleDegrees = point.AngleDegrees,
                    Labels = (point as ILabeledGeometry)?.Labels.ToArray()
                },
                LabeledPoint2D point => new StoredObject
                {
                    Point = new Point2D(point.X, point.Y),
                    Labels = point.Labels.ToArray()
                },
                Point2D point => new StoredObject { Point = point },
                LineSegment2D segment => new StoredObject
                {
                    Point1 = segment.Point1,
                    Point2 = segment.Point2,
                    Labels = (segment as ILabeledGeometry)?.Labels.ToArray()
                },
                _ => throw new InvalidDataException("Unsupported geometry type.")
            });
        }

        return JsonConvert.SerializeObject(objects, Formatting.Indented, new DoubleJsonConverter());
    }

    public static IReadOnlyList<object> Deserialize(string json)
    {
        var storedObjects = JsonConvert.DeserializeObject<List<StoredObject>>(json,
            new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error })
            ?? throw new InvalidDataException("Expected a geometry array.");
        var result = new List<object>();
        foreach (var stored in storedObjects)
        {
            if (stored == null)
                throw new InvalidDataException("Geometry entries cannot be null.");

            if (stored.Labels != null && (stored.Labels.Length == 0 || stored.Labels.Any(string.IsNullOrWhiteSpace)))
                throw new InvalidDataException("Labels require a nonblank identifier and nonblank information strings.");

            if (stored.Point != null && stored.Point1 == null && stored.Point2 == null)
                result.Add(stored.AngleDegrees.HasValue
                    ? stored.Labels != null
                        ? new LabeledOrientedPoint2D(stored.Point.X, stored.Point.Y, stored.AngleDegrees.Value, stored.Labels)
                        : new OrientedPoint2D(stored.Point.X, stored.Point.Y, stored.AngleDegrees.Value)
                    : stored.Labels != null
                        ? new LabeledPoint2D(stored.Point.X, stored.Point.Y, stored.Labels)
                        : stored.Point);
            else if (stored.Point == null && stored.Point1 != null && stored.Point2 != null && !stored.AngleDegrees.HasValue)
                result.Add(stored.Labels != null
                    ? new LabeledLineSegment2D(stored.Point1, stored.Point2, stored.Labels)
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? AngleDegrees { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[]? Labels { get; set; }
    }
}
