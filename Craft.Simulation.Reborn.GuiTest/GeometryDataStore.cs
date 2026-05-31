using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class GeometryDataStore : IGeometryDataSource
    {
        private List<object> _staticGeometricObjects;
        private List<object> _dynamicGeometricObjects;

        public GeometryDataStore()
        {
            _staticGeometricObjects = new List<object>();
            _dynamicGeometricObjects = new List<object>();
        }

        public void AddStaticGeometryObject(
            object geometryObject)
        {
            _staticGeometricObjects.Add(geometryObject);
        }

        public void AddDynamicGeometryObject(
            object geometryObject)
        {
            _dynamicGeometricObjects.Add(geometryObject);
        }

        public void ClearStaticGeometryObjects()
        {
            _staticGeometricObjects.Clear();
        }

        public void ClearDynamicGeometryObjects()
        {
            _dynamicGeometricObjects.Clear();
        }

        public IEnumerable Query(
            BoundingBox window)
        {
            return _staticGeometricObjects;
        }
    }
}
