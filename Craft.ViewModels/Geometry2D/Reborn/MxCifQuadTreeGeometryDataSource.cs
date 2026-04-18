using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;

namespace Craft.ViewModels.Geometry2D.Reborn;

public class MxCifQuadTreeGeometryDataSource : IGeometryDataSource
{
    private MxCifQuadTree<LineModel> _mxCifQuadTree;

    public MxCifQuadTreeGeometryDataSource()
    {
        _mxCifQuadTree = new MxCifQuadTree<LineModel>(new BoundingBox(0, 1000, 0, 1000), new DummyLogger());
    }

    public IEnumerable<LineModel> Query(
        BoundingBox window)
    {
        yield return new LineModel
        {
            P1 = new System.Windows.Point(200, 300),
            P2 = new System.Windows.Point(400, 200)
        };
    }
}