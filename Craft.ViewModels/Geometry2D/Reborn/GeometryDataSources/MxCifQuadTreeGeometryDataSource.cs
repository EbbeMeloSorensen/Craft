using System.Collections;
using Craft.Logging;
using Craft.Math;
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

        var circles = new List<Circle2D>
        {
            new Circle2D(new Point2D(150, 150), 40),
            new Circle2D(new Point2D(250, 150), 40)
        };

        foreach (var circle in circles)
        {
            var bbox = circle.ComputeBoundingBox();
            _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, circle));
        }

        var points = new List<Point2D>();

        for (var x = -100; x <= 500; x += 20)
        {
            for (var y = -100; y <= 500; y += 20)
            {
                points.Add(new Point2D(x, y));
            }
        }

        foreach (var point in points)
        {
            var bbox = point.ComputeBoundingBox();
            _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, point));
        }

        var lineSegments = new List<LineSegment2D>
        {
            new LineSegment2D(
                new Point2D(0, 0),
                new Point2D(0, 200)),
            new LineSegment2D(
                new Point2D(0, 200),
                new Point2D(200, 300)),
            new LineSegment2D(
                new Point2D(200, 300),
                new Point2D(400, 200)),
            new LineSegment2D(
                new Point2D(400, 200),
                new Point2D(400, 0)),
            new LineSegment2D(
                new Point2D(400, 0),
                new Point2D(0, 0))
        };

        foreach (var lineSegment in lineSegments)
        {
            var bbox = lineSegment.ComputeBoundingBox();
            _mxCifQuadTree.Insert(new SpatialItem<object>(bbox, lineSegment));
        }
    }

    public IEnumerable GetGeometries(
        BoundingBox window)
    {
        var result = _mxCifQuadTree.GetAllIntersecting(window);
        return result.Select(_ => _.Item);
    }
}