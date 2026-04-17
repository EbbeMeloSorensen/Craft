using System.Windows;

namespace Craft.ViewModels.Geometry2D.Reborn;

public struct ViewState
{
    public Point WorldOrigin { get; }
    public Size Scaling { get; }
    public int ZoomLevelX { get; }
    public int ZoomLevelY { get; }

    public ViewState(
        Point worldOrigin,
        Size scaling)
    {
        WorldOrigin = worldOrigin;
        Scaling = scaling;
    }
}