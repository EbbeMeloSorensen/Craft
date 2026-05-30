using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class GeometryDataStore : IGeometryDataSource
    {
        private List<object> _geometricObjects;

        public GeometryDataStore()
        {
            _geometricObjects = new List<object>();
        }

        public void AddCircle(
            System.Windows.Point center,
            double radius)
        {
            _geometricObjects.Add(new CircleModel
            {
                Center = center,
                Radius = radius
            });
        }

        public void Clear()
        {
            _geometricObjects.Clear();
        }

        public IEnumerable Query(
            BoundingBox window)
        {
            return _geometricObjects;
        }
    }
}
