using System.Diagnostics;
using Craft.DataStructures.MxCifQuadTree;
using FluentAssertions;
using System.Globalization;
using Xunit;

namespace Craft.DataStructures.UnitTest;

public class MxCifQuadTreeTest
{
    [Fact]
    public void Test1_Insert12Rectangles_And_CheckForIntersectionWithAnother()
    {
        // Arrange
        var rectangle1 = new Rectangle(12.5, 25, 5, 5);
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
        mxCifQuadTree1.Remove(rectangle1); 
    }

    [Fact]
    public void Test2_InsertSomeRectangles_ThenRemoveThemAgain()
    {
        var logger = new TestLogger();
        logger.IsEnabled = true;

        // Arrange
        var rectangle1 = new Rectangle(12.5, 25, 5, 5);
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
        var mxCifQuadTree1 = new MxCifQuadTree.MxCifQuadTree(new Rectangle(50, 50, 50, 50), logger);

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

        // Act
        mxCifQuadTree1.Remove(rectangle1);
        mxCifQuadTree1.Remove(rectangle2);
        mxCifQuadTree1.Remove(rectangle3);
        mxCifQuadTree1.Remove(rectangle4);
        mxCifQuadTree1.Remove(rectangle5);
        mxCifQuadTree1.Remove(rectangle6);
        mxCifQuadTree1.Remove(rectangle7);
        mxCifQuadTree1.Remove(rectangle8);
        mxCifQuadTree1.Remove(rectangle9);
        mxCifQuadTree1.Remove(rectangle10);
        mxCifQuadTree1.Remove(rectangle11);
        mxCifQuadTree1.Remove(rectangle12);

        logger.Complete();
    }

    [Fact]
    public void Test3_Replicate_TheCPPImplementation()
    {
        // In this test, we verify that we get the same result as in the C++ implementation,
        // When trying to add a the same range of rectangles to an mxcifquadtree and rejecting those that overlap with already inserted rectangles.

        var logger = new TestLogger();

        var mxCifQuadTree = new MxCifQuadTree.MxCifQuadTree(new Rectangle(50, 50, 50, 50), logger);
        var areaOfIntereset = new Rectangle(60.0, 40.0, 20.0, 20.0);

        var lines = File.ReadAllLines(@"C:\Temp\all_rectangles.txt");

        using var sw = new StreamWriter(@"C:\Temp\replication_test.svg");
        sw.WriteLine("<svg width=\"100\" height=\"100\" xmlns=\"http://www.w3.org/2000/svg\">");
        sw.WriteLine("  <rect width=\"100\" height=\"100\" x=\"0\" y=\"0\" fill=\"gray\" />");

        sw.WriteLine($"  <rect width=\"{areaOfIntereset.HalfWidth * 2}\" height=\"{areaOfIntereset.HalfHeight * 2}\" x=\"{areaOfIntereset.CenterX - areaOfIntereset.HalfWidth}\" y=\"{areaOfIntereset.CenterY - areaOfIntereset.HalfHeight}\" fill=\"black\" />");

        var count = 0;

        foreach (var line in lines)
        {
            count++;

            // Read the next rectangle in the range
            var temp = line.Split(',');
            var centerX = double.Parse(temp[0].Trim(), CultureInfo.InvariantCulture);
            var centerY = double.Parse(temp[1].Trim(), CultureInfo.InvariantCulture);
            var halfWidth = double.Parse(temp[2].Trim(), CultureInfo.InvariantCulture);
            var halfHeight = double.Parse(temp[3].Trim(), CultureInfo.InvariantCulture);
            var intersectingInCppImplementation = temp[4].Trim() == "0";

            var rectangle = new Rectangle(centerX, centerY, halfWidth, halfHeight);

            var intersectsPreviouslyInsertedRectangles = mxCifQuadTree.Intersects(rectangle);

            intersectsPreviouslyInsertedRectangles.Should().Be(intersectingInCppImplementation);

            if (intersectsPreviouslyInsertedRectangles) continue;

            sw.WriteLine($"  <rect width=\"{halfWidth * 2}\" height=\"{halfHeight * 2}\" x=\"{centerX - halfWidth}\" y=\"{centerY - halfHeight}\" fill=\"green\" />");
            mxCifQuadTree.Insert(rectangle);
        }

        var intersectingRectangles = mxCifQuadTree
            .GetAllIntersecting(areaOfIntereset)
            .ToList();

        foreach (var rect in intersectingRectangles)
        {
            sw.WriteLine($"  <rect width=\"{rect.HalfWidth * 2}\" height=\"{rect.HalfHeight * 2}\" x=\"{rect.CenterX - rect.HalfWidth}\" y=\"{rect.CenterY - rect.HalfHeight}\" fill=\"yellow\" />");
        }

        sw.WriteLine("</svg>");
        logger.Complete();
    }

    [Fact]
    public void Test4_TimingTest()
    {
        var stopWatch = new Stopwatch();
        var logger = new TestLogger();
        logger.IsEnabled = false;

        var random = new Random(0);
        var rectanglesInTotal = 1000000;
        var maxNumberOfRectanglesInTree = 100000;
        var rectangleQueue = new Queue<Rectangle>();
        var mxCifQuadTree = new MxCifQuadTree.MxCifQuadTree(new Rectangle(50.0, 50.0, 50.0, 50.0), logger);
        var controlList = new List<int>();
        var rectanglesInMxCifQuadTree = 0;

        stopWatch.Start();

        for (var i = 0; i < rectanglesInTotal + maxNumberOfRectanglesInTree; i++)
        {
            var width = 10.0;
            var height = 10.0;
            var cx = random.NextDouble() * (100 - width) + 0.5 * width;
            var cy = random.NextDouble() * (100 - height) + 0.5 * height;

            if (i < rectanglesInTotal)
            {
                var rectangle = new Rectangle(cx, cy, width * 0.5, height * 0.5);
                mxCifQuadTree.Insert(rectangle);
                rectanglesInMxCifQuadTree++;
                rectangleQueue.Enqueue(rectangle);
            }

            if (i >= maxNumberOfRectanglesInTree && rectangleQueue.Count > 0)
            {
                var rectangleFromQueue = rectangleQueue.Dequeue();
                mxCifQuadTree.Remove(rectangleFromQueue);
                rectanglesInMxCifQuadTree--;
            }

            controlList.Add(rectanglesInMxCifQuadTree);
        }

        stopWatch.Stop();
        var elapsed = stopWatch.Elapsed;

        logger.Complete();
    }
}
