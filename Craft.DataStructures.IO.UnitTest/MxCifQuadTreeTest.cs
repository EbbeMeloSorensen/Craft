using Craft.DataStructures.MxCifQuadTree;
using Xunit;

namespace Craft.DataStructures.IO.UnitTest;

public class MxCifQuadTreeTest
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var rectangle1 = new Rectangle(25, 25, 5, 5);
        var rectangle2 = new Rectangle(50, 6.25, 1, 1);
        var rectangle3 = new Rectangle(53.125, 12.5, 2.0, 2.0);
        var rectangle4 = new Rectangle(62.5, 12.5, 3.0, 3.0);
        var rectangle5 = new Rectangle(71.875, 12.5, 2.0, 2.0);
        var rectangle6 = new Rectangle(50.0, 37.5, 3.0, 3.0);
        var rectangle7 = new Rectangle(18.75, 50.0, 3.0, 3.0);
        var rectangle8 = new Rectangle(50.0, 50.0, 5.0, 5.0);
        var rectangle9 = new Rectangle(75.0, 50.0, 5.0, 5.0);
        var rectangle10 = new Rectangle(50.0, 75.0, 5.0, 5.0);
        var rectangle11 = new Rectangle(75.0, 75.0, 5.0, 5.0);
        var rectangle12 = new Rectangle(87.5, 93.75, 2.0, 2.0);

        var mxCifQuadTree1 = new MxCifQuadTree.MxCifQuadTree(new Rectangle(50, 50, 50, 50));

        mxCifQuadTree1.Insert(rectangle1);
        mxCifQuadTree1.Insert(rectangle2);
        mxCifQuadTree1.Insert(rectangle3);
        mxCifQuadTree1.Insert(rectangle4);
        mxCifQuadTree1.Insert(rectangle5);
        mxCifQuadTree1.Insert(rectangle6);
        mxCifQuadTree1.Insert(rectangle7);
        mxCifQuadTree1.Insert(rectangle8);
        mxCifQuadTree1.Insert(rectangle9);
        mxCifQuadTree1.Insert(rectangle10);
        mxCifQuadTree1.Insert(rectangle11);
        mxCifQuadTree1.Insert(rectangle12);

        var rectangleQ = new Rectangle(50.0, 75.0, 5.0, 5.0);

        var a = rectangleQ.Intersects(rectangle1);
        var b = rectangleQ.Intersects(rectangle2);
        var c = rectangleQ.Intersects(rectangle3);
        var d = rectangleQ.Intersects(rectangle4);
        var e = rectangleQ.Intersects(rectangle5);
        var f = rectangleQ.Intersects(rectangle6);
        var g = rectangleQ.Intersects(rectangle7);
        var h = rectangleQ.Intersects(rectangle8);
        var i = rectangleQ.Intersects(rectangle9);
        var j = rectangleQ.Intersects(rectangle10);
        var k = rectangleQ.Intersects(rectangle11);
        var l = rectangleQ.Intersects(rectangle12);

        if (mxCifQuadTree1.Intersects(rectangleQ))
        {
            var x = 0;
        }
        else
        {
            var x = 0;
        }

        // Act
        // Assert
    }
}