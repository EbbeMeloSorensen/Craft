using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class SimpleGeometryDataSource : IGeometryDataSource
{
    public IEnumerable GetAll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable GetIntersecting(BoundingBox window)
    {
        yield return new VerticalLineModel
        {
            X = 175
        };

        yield return new VerticalLineModel
        {
            X = 225
        };

        yield return new HorizontalLineModel
        {
            Y = 50
        };

        yield return new HorizontalLineModel
        {
            Y = 100
        };
    }
}