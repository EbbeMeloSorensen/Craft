using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.UIElements.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

namespace Craft.UIElements.Reborn.GuiTest;

public class TemperatureDataSource : IGeometryDataSource
{
    private List<Tuple<DateTime, double>> _temperatureMeasurements;

    public TemperatureDataSource()
    {
        _temperatureMeasurements = new List<Tuple<DateTime, double>>
        {
            new Tuple<DateTime, double>(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), 17.2),
            new Tuple<DateTime, double>(new DateTime(2025, 1, 1, 1, 0, 0, DateTimeKind.Utc), 17.3),
            new Tuple<DateTime, double>(new DateTime(2025, 1, 1, 2, 0, 0, DateTimeKind.Utc), 17.1),
        };
    }

    public IEnumerable Query(
        BoundingBox window)
    {
        return _temperatureMeasurements.Select(tm => new PointModel
        {
            P = new System.Windows.Point(
                TimeCoordinates.ToWorldTicks(tm.Item1),
                tm.Item2)
        });
    }
}