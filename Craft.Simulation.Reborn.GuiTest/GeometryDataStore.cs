using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;
using System.Collections;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class GeometryDataStore : IGeometryDataSource
    {
        private List<object> _staticGeometricObjects;
        private MxCifQuadTree<object> _mxCifQuadTree;

        public GeometryDataStore(
            BoundingBox region,
            int maxDepth = 8)
        {
            _staticGeometricObjects = new List<object>();

            _mxCifQuadTree = new MxCifQuadTree<object>(
                region, maxDepth, new DummyLogger());
        }

        public void AddStaticGeometryObject(
            object geometryObject)
        {
            _staticGeometricObjects.Add(geometryObject);

            switch (geometryObject)
            {
                case LineModel line:
                    var bbox = line.ComputeBoundingBox();
                    _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, line));
                    break;
                default:
                    // Handle objects that do not have a bounding box if necessary
                    break;
            }
        }

        public IEnumerable Query(
            BoundingBox window)
        {
            //return _staticGeometricObjects;

            var result = _mxCifQuadTree.GetAllIntersecting(window);
            return result.Select(_ => _.Item);
        }
    }
}
