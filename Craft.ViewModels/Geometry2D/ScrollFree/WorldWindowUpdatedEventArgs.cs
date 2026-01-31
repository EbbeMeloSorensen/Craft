using System.Windows;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class WorldWindowUpdatedEventArgs : EventArgs
    {
        public readonly Point WorldWindowUpperLeft;
        public readonly Size WorldWindowSize;

        public WorldWindowUpdatedEventArgs(
            Point worldWindowUpperLeft, 
            Size worldWindowSize)
        {
            WorldWindowUpperLeft = worldWindowUpperLeft;
            WorldWindowSize = worldWindowSize;
        }
    }
}
