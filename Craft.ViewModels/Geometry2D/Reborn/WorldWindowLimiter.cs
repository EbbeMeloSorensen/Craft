using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn;

public class WorldWindowLimiter
{
    private readonly BoundingBox _bounds;
    private bool _disable = true;

    public WorldWindowLimiter(
        BoundingBox bounds)
    {
        _bounds = bounds;
    }

    /// <summary>
    /// Enforces both zoom and pan constraints.
    /// </summary>
    public BoundingBox Limit(
        BoundingBox worldWindow)
    {
        if (_disable)
        {
            return worldWindow;
        }

        // Step 1: Enforce minimum zoom (fit inside bounds)
        var fitted = EnforceMinimumZoom(worldWindow);

        // Step 2: Clamp position (panning)
        var clamped = ClampToBounds(fitted);

        return clamped;
    }

    private BoundingBox EnforceMinimumZoom(BoundingBox worldWindow)
    {
        var width = worldWindow.Width;
        var height = worldWindow.Height;

        var boundsWidth = _bounds.Width;
        var boundsHeight = _bounds.Height;

        // If already small enough, do nothing
        if (width <= boundsWidth && height <= boundsHeight)
            return worldWindow;

        // Compute scale needed to fit inside bounds
        var scaleX = boundsWidth / width;
        var scaleY = boundsHeight / height;
        var scale = System.Math.Min(scaleX, scaleY);

        // Apply scale around center
        var centerX = (worldWindow.MinX + worldWindow.MaxX) / 2;
        var centerY = (worldWindow.MinY + worldWindow.MaxY) / 2;

        var newWidth = width * scale;
        var newHeight = height * scale;

        return BoundingBox.FromCenter(centerX, centerY, newWidth, newHeight);
    }

    private BoundingBox ClampToBounds(BoundingBox worldWindow)
    {
        var minX = worldWindow.MinX;
        var minY = worldWindow.MinY;
        var maxX = worldWindow.MaxX;
        var maxY = worldWindow.MaxY;

        // Clamp horizontally
        if (minX < _bounds.MinX)
        {
            var shift = _bounds.MinX - minX;
            minX += shift;
            maxX += shift;
        }
        else if (maxX > _bounds.MaxX)
        {
            var shift = _bounds.MaxX - maxX;
            minX += shift;
            maxX += shift;
        }

        // Clamp vertically
        if (minY < _bounds.MinY)
        {
            var shift = _bounds.MinY - minY;
            minY += shift;
            maxY += shift;
        }
        else if (maxY > _bounds.MaxY)
        {
            var shift = _bounds.MaxY - maxY;
            minY += shift;
            maxY += shift;
        }

        return new BoundingBox(minX, maxX, minY, maxY);
    }
}