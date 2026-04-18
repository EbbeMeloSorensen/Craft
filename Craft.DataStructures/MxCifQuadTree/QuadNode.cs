using Craft.Logging;

namespace Craft.DataStructures.MxCifQuadTree;

public class QuadNode<T>
{
    private ILogger _logger;

    public static readonly int[] g_VF = [-1, 1];

    public BinNode<T>[] _axis;
    public QuadNode<T>[] _child;

    public QuadNode(
        ILogger logger)
    {
        _axis = new BinNode<T>[2];
        _child = new QuadNode<T>[4];
        _logger = logger;
    }

    public void InsertOnAxis(
        SpatialItem<T> spatialItem,
        double cv,
        double lv,
        AXIS v)
    {
        _axis[(int)v] ??= new BinNode<T>();

        var rectangle = spatialItem.Bounds;
        var binNode = _axis[(int)v];
        var d = rectangle.BIN_COMPARE(cv, v);

        var binNodeLevel = 1;

        while (d != DIRECTION.BOTH)
        {
            var index = (int)d;
            binNode.Child[index] ??= new BinNode<T>();
            binNode = binNode.Child[index];
            lv /= 2;
            cv += lv * g_VF[index];

            if (_logger.IsEnabled)
            {
                _logger.WriteLineGoddammit(
                    LogMessageCategory.Information,
                    $"      No intersection at bin node level {binNodeLevel} => Navigating to the {d}, where bin node is centered at x = {cv}");
            }

            d = rectangle.BIN_COMPARE(cv, v);
            binNodeLevel++;
        }

        if (_logger.IsEnabled)
        {
            _logger.WriteLineGoddammit(
                LogMessageCategory.Information,
                $"        Intersecting at bin node level {binNodeLevel} => inserting rectangle in bin node");
        }

        binNode.Insert(spatialItem);
    }
}