using Craft.DataStructures.MxCifQuadTree;
using Xunit;
using FluentAssertions;

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

        // Act
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

        // Assert
        mxCifQuadTree1.Intersects(rectangleQ).Should().BeTrue(); // It intersects rectangle 10

        // Act
        //mxCifQuadTree1.Remove(rectangle3); (Det venter vi lige med)
    }

    [Fact]
    public void Test2()
    {
        var dx = 10.0;
        var random = new Random();

        var rectangles = Enumerable.Repeat(0, 200).Select(_ =>
        {
            var fracX = random.NextDouble();
            var fracY = random.NextDouble();
            var centerX = fracX * (100 - dx) + 0.5 * dx;
            var centerY = fracY * (100 - dx) + 0.5 * dx;

            return new Rectangle(centerX, centerY, dx / 2, dx / 2 );
        }).ToList();

        var mxCifQuadTree1 = new MxCifQuadTree.MxCifQuadTree(new Rectangle(50, 50, 50, 50));

        using var sw = new StreamWriter(@"C:\Temp\rectangles.svg");
        var magnification = 8.0;
        sw.WriteLine($"<svg width=\"{100.0 * magnification}\" height=\"{100.0 * magnification}\" xmlns=\"http://www.w3.org/2000/svg\">");
        sw.WriteLine($"  <rect width=\"{100 * magnification}\" height=\"{100 * magnification}\" x=\"{magnification}\" y=\"{magnification}\" fill=\"gray\" />");
        foreach(var rectangle in rectangles)
        {
            if (mxCifQuadTree1.Intersects(rectangle)) continue;

            mxCifQuadTree1.Insert(rectangle);
            sw.WriteLine($"  <rect width=\"{dx * magnification}\" height=\"{dx * magnification}\" x=\"{(rectangle.CenterX - dx / 2) * magnification}\" y=\"{(rectangle.CenterY - dx / 2) * magnification}\" fill=\"black\" />");
        };
        sw.WriteLine("</svg>");
    }
}