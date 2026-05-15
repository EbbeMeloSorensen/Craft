using System.Windows;

namespace Craft.ViewModels.Geometry2D.Reborn;

// Todo: Gør viewport ratio nullable
public class WorldFocusRequest
{
    public Point WorldPoint;
    public Size ViewportRatio;
    public Size? Scaling;
}