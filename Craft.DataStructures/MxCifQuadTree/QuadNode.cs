using Craft.Logging;

namespace Craft.DataStructures.MxCifQuadTree;

public class QuadNode<T>
{
    private ILogger _logger;

    public static readonly int[] g_VF = [-1, 1];

    public BinNode<T>[] _axis;
    public QuadNode<T>[] _child;

    public List<SpatialItem<T>> SpatialItems { get; }  // Relevant if operating with max depth

    public QuadNode(
        ILogger logger)
    {
        _axis = new BinNode<T>[2];
        _child = new QuadNode<T>[4];
        SpatialItems = new List<SpatialItem<T>>();
        _logger = logger;
    }

    public void Insert(
        SpatialItem<T> spatialItem)
    {
        SpatialItems.Add(spatialItem);
    }

    public void InsertOnAxis(
        SpatialItem<T> spatialItem,
        double cv,
        double lv,
        AXIS v,
        int maxBinNodeLevel)
    {
        _axis[(int)v] ??= new BinNode<T>();

        var rectangle = spatialItem.Bounds;
        var binNode = _axis[(int)v];
        var d = rectangle.BIN_COMPARE(cv, v);

        var binNodeLevel = 1;

        while (
            d != DIRECTION.BOTH && 
            binNodeLevel < maxBinNodeLevel)
        {
            var index = (int)d;
            binNode.Child[index] ??= new BinNode<T>();
            binNode = binNode.Child[index];
            lv /= 2;
            cv += lv * g_VF[index];

            if (_logger.IsEnabled)
            {
                _logger.WriteLine(
                    LogMessageCategory.Information,
                    $"      No intersection at bin node level {binNodeLevel} => Navigating to the {d}, where bin node is centered at x = {cv}");
            }

            d = rectangle.BIN_COMPARE(cv, v);
            binNodeLevel++;
        }

        if (_logger.IsEnabled)
        {
            if (d == DIRECTION.BOTH)
            {
                _logger.WriteLine(
                    LogMessageCategory.Information,
                    $"        Intersecting at bin node level {binNodeLevel} => inserting rectangle in bin node");
            }
            else
            {
                _logger.WriteLine(
                    LogMessageCategory.Information,
                    $"        No axis intersection but max bin node level ({maxBinNodeLevel}) reached, so inserting rectangle in bin node");
            }
        }

        binNode.Insert(spatialItem);
    }
}