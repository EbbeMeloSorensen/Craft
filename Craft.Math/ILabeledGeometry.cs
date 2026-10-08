namespace Craft.Math;

/// <summary>Ordered strings: an identifier followed by arbitrary information.</summary>
public interface ILabeledGeometry
{
    IReadOnlyList<string> Labels { get; }
    string Identifier { get; }
}

internal static class GeometryLabelStrings
{
    internal static IReadOnlyList<string> Copy(IEnumerable<string> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);
        var copy = labels.ToArray();
        if (copy.Length == 0 || copy.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Labels require a nonblank identifier and nonblank information strings.", nameof(labels));
        return Array.AsReadOnly(copy);
    }
}
