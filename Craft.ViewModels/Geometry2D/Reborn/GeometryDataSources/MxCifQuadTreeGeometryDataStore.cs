using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources
{
    public class MxCifQuadTreeGeometryDataStore : IGeometryDataStore
    {
        private MxCifQuadTree<object> _mxCifQuadTree;
        private Dictionary<object, SpatialItem<object>> _spatialItemMap;

        public MxCifQuadTreeGeometryDataStore(
            BoundingBox region,
            int maxDepth = 8)
        {
            _mxCifQuadTree = new MxCifQuadTree<object>(region, maxDepth, new DummyLogger());
            _spatialItemMap = new Dictionary<object, SpatialItem<object>>();
        }

        public void AddGeometricObject(
            object geometricObject,
            BoundingBox boundingBox)
        {
            var line = geometricObject as Math.LineSegment2D;
            var bbox = line.ComputeBoundingBox();
            var spatialItem = new SpatialItem<object>(bbox, line);
            _mxCifQuadTree.Insert(spatialItem);
            _spatialItemMap[geometricObject] = spatialItem;
        }

        public void RemoveGeometricObjects(
            IEnumerable<object> geometricObjects)
        {
            foreach (var geometricObject in geometricObjects)
            {
                var spatialItem = _spatialItemMap[geometricObject];
                _mxCifQuadTree.Remove(spatialItem);
                _spatialItemMap.Remove(geometricObject);
            }
        }

        public IEnumerable GetAll()
        {
            return _mxCifQuadTree
                .GetAll()
                .Select(_ => _.Item);
        }

        public IEnumerable GetIntersecting(
            BoundingBox window)
        {
            return _mxCifQuadTree
                .GetIntersecting(window)
                .Select(_ => _.Item);
        }
    }
}