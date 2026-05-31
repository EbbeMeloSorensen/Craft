using System.Collections;
namespace Craft.ViewModels.Geometry2D.Reborn;

public class GeometryLayer
{
    public IEnumerable GeometricObjects { get; }

    public GeometryLayer(
        IEnumerable geometricObjects)
    {
        GeometricObjects = geometricObjects;
    }
}
