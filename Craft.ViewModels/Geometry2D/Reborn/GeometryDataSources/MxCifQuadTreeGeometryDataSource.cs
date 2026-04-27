using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;

namespace Craft.ViewModels.Geometry2D.Reborn.GeometryDataSources;

public class MxCifQuadTreeGeometryDataSource : IGeometryDataSource
{
    private MxCifQuadTree<LineModel> _mxCifQuadTree;

    public MxCifQuadTreeGeometryDataSource()
    {
        _mxCifQuadTree = new MxCifQuadTree<LineModel>(new BoundingBox(-500, 500, -500, 500), new DummyLogger());

        var lines = new List<LineModel>
        {
            new LineModel
            {
                P1 = new System.Windows.Point(0, 0),
                P2 = new System.Windows.Point(0, 200)
            },
            new LineModel
            {
                P1 = new System.Windows.Point(0, 200),
                P2 = new System.Windows.Point(200, 300)
            },
            new LineModel
            {
                P1 = new System.Windows.Point(200, 300),
                P2 = new System.Windows.Point(400, 200)
            },
            new LineModel
            {
                P1 = new System.Windows.Point(400, 200),
                P2 = new System.Windows.Point(400, 0)
            },
            new LineModel
            {
                P1 = new System.Windows.Point(400, 0),
                P2 = new System.Windows.Point(0, 0)
            }
        };

        foreach (var line in lines)
        {
            var bbox = line.ComputeBoundingBox();
            _mxCifQuadTree.Insert(new SpatialItem<LineModel>(bbox, line));
        }
    }

    public IEnumerable<LineModel> Query(
        BoundingBox window)
    {
        var result = _mxCifQuadTree.GetAllIntersecting(window);
        return result.Select(_ => _.Item);
    }
}