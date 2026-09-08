using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using System.Collections;
using System.Windows.Shapes;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources
{
    public class MxCifQuadTreeGeometryDataStore : IGeometryDataStore
    {
        private MxCifQuadTree<object> _mxCifQuadTree;

        public MxCifQuadTreeGeometryDataStore(
            BoundingBox region,
            int maxDepth = 8)
        {
            _mxCifQuadTree = new MxCifQuadTree<object>(region, maxDepth, new DummyLogger());
        }

        public void AddGeometricObject(
            object geometricObject,
            BoundingBox boundingBox)
        {
            var temp = geometricObject as PolyLineModel;
            var points = new List<System.Windows.Point>();

            foreach (var point in temp.Points)
            {
                points.Add(point);
            }

            var polyLine = new PolyLineModel
            {
                Points = points
            };

            var bbox = polyLine.ComputeBoundingBox();
            _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, polyLine));
        }

        public IEnumerable Query(
            BoundingBox window)
        {
            var result = _mxCifQuadTree.GetAllIntersecting(window);
            return result.Select(_ => _.Item);
        }
    }
}