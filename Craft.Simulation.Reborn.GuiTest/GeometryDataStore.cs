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
                    _mxCifQuadTree.Insert(new SpatialItem<object>(line.ComputeBoundingBox(), line));
                    break;
                case PointModel point:
                    _mxCifQuadTree.Insert(new SpatialItem<object>(point.ComputeBoundingBox(), point));
                    break;
                default:
                    throw new ArgumentException("Object is not a geometry object");
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
