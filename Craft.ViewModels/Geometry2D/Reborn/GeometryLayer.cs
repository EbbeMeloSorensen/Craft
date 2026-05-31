using System.Collections;

namespace Craft.ViewModels.Geometry2D.Reborn;

public class GeometryLayer
{
    public IEnumerable GeometricObjects { get; }

    public bool IsFrameDependent { get; }

    public GeometryLayer(
        IEnumerable geometricObjects,
        bool isFrameDependent)
    {
        GeometricObjects = geometricObjects;
        IsFrameDependent = isFrameDependent;
    }
}
