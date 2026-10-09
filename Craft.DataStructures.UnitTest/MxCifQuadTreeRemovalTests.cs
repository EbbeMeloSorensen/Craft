using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;
using Xunit;

namespace Craft.DataStructures.UnitTest;

public class MxCifQuadTreeRemovalTests
{
    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 1)]
    [InlineData(true, 3)]
    [InlineData(false, 3)]
    [InlineData(true, 8)]
    [InlineData(false, 8)]
    public void RemovingAxisItemPreservesItemsStoredDirectlyInSameQuadNode(bool removeAxisFirst, int maxDepth)
    {
        var tree = new MxCifQuadTree<object>(new BoundingBox(0, 100, 0, 100), maxDepth, new DummyLogger());
        var scale = System.Math.Pow(0.5, maxDepth - 1);
        var axisItem = new SpatialItem<object>(new BoundingBox(40 * scale, 60 * scale, 20 * scale, 20 * scale), new object());
        var directItem = new SpatialItem<object>(new BoundingBox(10 * scale, 20 * scale, 10 * scale, 10 * scale), new object());
        tree.Insert(axisItem);
        tree.Insert(directItem);

        var removed = removeAxisFirst ? axisItem : directItem;
        var survivor = removeAxisFirst ? directItem : axisItem;
        tree.Remove(removed);

        Assert.Same(survivor, Assert.Single(tree.GetAll()));
        Assert.Same(survivor, Assert.Single(tree.GetIntersecting(survivor.Bounds)));
        tree.Remove(survivor);
        Assert.True(tree.IsEmpty());
    }
}
