using System.Collections;
using Craft.Logging;
using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class MxCifQuadTreeGeometryDataSource : IGeometryDataSource
{
    private MxCifQuadTree<object> _mxCifQuadTree;

    public MxCifQuadTreeGeometryDataSource(
        BoundingBox region,
        int maxDepth = 8)
    {
        _mxCifQuadTree = new MxCifQuadTree<object>(region, maxDepth, new DummyLogger());

        var polyLines = new List<PolyLineModel>
        {
            new PolyLineModel
            {
                Points = new List<System.Windows.Point>
                {
                    new(0, 0),
                    new(100, 100),
                    new(200, 50),
                    new(300, 150)
                }
            }
        };

        foreach (var polyLine in polyLines)
        {
            var bbox = polyLine.ComputeBoundingBox();
            _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, polyLine));
        }

        var circles = new List<Math.Circle2D>
        {
            new Math.Circle2D(new Craft.Math.Point2D(150, 150), 40),
            new Math.Circle2D(new Craft.Math.Point2D(250, 150), 40)
        };

        //foreach (var circle in circles)
        //{
        //    var bbox = circle.ComputeBoundingBox();
        //    _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, circle));
        //}

        //var points = new List<PointModel>();

        //for (var x = -100; x <= 500; x += 20)
        //{
        //    for (var y = -100; y <= 500; y += 20)
        //    {
        //        points.Add(new PointModel
        //        {
        //            P = new System.Windows.Point(x, y),
        //        });
        //    }
        //}

        //foreach (var point in points)
        //{
        //    var bbox = point.ComputeBoundingBox();
        //    _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, point));
        //}

        //var lines = new List<LineModel>
        //{
        //    new LineModel
        //    {
        //        P1 = new System.Windows.Point(0, 0),
        //        P2 = new System.Windows.Point(0, 200)
        //    },
        //    new LineModel
        //    {
        //        P1 = new System.Windows.Point(0, 200),
        //        P2 = new System.Windows.Point(200, 300)
        //    },
        //    new LineModel
        //    {
        //        P1 = new System.Windows.Point(200, 300),
        //        P2 = new System.Windows.Point(400, 200)
        //    },
        //    new LineModel
        //    {
        //        P1 = new System.Windows.Point(400, 200),
        //        P2 = new System.Windows.Point(400, 0)
        //    },
        //    new LineModel
        //    {
        //        P1 = new System.Windows.Point(400, 0),
        //        P2 = new System.Windows.Point(0, 0)
        //    }
        //};

        //foreach (var line in lines)
        //{
        //    var bbox = line.ComputeBoundingBox();
        //    _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, line));
        //}
    }

    public IEnumerable Query(
        BoundingBox window)
    {
        var result = _mxCifQuadTree.GetAllIntersecting(window);
        return result.Select(_ => _.Item);
    }
}