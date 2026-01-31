using System.Windows;

namespace Craft.ViewModels.Geometry2D.ScrollFree;

public class MouseEventArgs : EventArgs
{
    public readonly Point CursorWorldPosition;

    public MouseEventArgs(Point cursorWorldPosition)
    {
        CursorWorldPosition = cursorWorldPosition;
    }
}
