using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

namespace Craft.UIElements.Reborn.GuiTest;

public class TimeStampDataSource : IGeometryDataSource
{
    public IEnumerable Query(
        BoundingBox window)
    {
        return Enumerable.Empty<object>();
    }
}

