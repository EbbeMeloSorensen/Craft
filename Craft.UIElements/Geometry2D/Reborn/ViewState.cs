using System.Windows;

namespace Craft.UIElements.Geometry2D.Reborn;

public struct ViewState
{
    public Point WorldOrigin{ get; }
    public Size Scaling { get; }

    public ViewState(
        Point worldOrigin,
        Size scaling)
    {
        WorldOrigin = worldOrigin;
        Scaling = scaling;
    }
}