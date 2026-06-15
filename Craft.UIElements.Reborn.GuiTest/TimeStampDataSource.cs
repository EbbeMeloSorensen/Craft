using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.UIElements.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;

namespace Craft.UIElements.Reborn.GuiTest;

public class TimeStampDataSource : IGeometryDataSource
{
    private List<DateTime> _birthdays;

    public TimeStampDataSource()
    {
        _birthdays = new List<DateTime>
        {
            new DateTime(1944, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(1948, 5, 14, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(1972, 3, 17, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(1975, 7, 24, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(1980, 6, 13, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2011, 4, 3, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2014, 5, 20, 0, 0, 0, DateTimeKind.Utc),
        };
    }

    public IEnumerable Query(
        BoundingBox window)
    {
        return _birthdays.Select(birthday => new VerticalLineModel
        {
            X = TimeCoordinates.ToWorldTicks(birthday)
        });
    }
}