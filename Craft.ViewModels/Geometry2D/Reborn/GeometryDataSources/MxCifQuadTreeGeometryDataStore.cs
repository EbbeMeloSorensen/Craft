using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;
using System.Collections;
using System.Windows.Shapes;

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
            switch (geometricObject)
            {
                case Math.Point2D point:
                    var spatialItemPoint = new SpatialItem<object>(point.ComputeBoundingBox(), point);
                    _mxCifQuadTree.Insert(spatialItemPoint);
                    _spatialItemMap[geometricObject] = spatialItemPoint;
                    break;
                case Math.LineSegment2D line:
                    var spatialItemLine = new SpatialItem<object>(line.ComputeBoundingBox(), line);
                    _mxCifQuadTree.Insert(spatialItemLine);
                    _spatialItemMap[geometricObject] = spatialItemLine;
                    break;
            }
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