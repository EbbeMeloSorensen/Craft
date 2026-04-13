using System.Diagnostics;
using FluentAssertions;
using System.Globalization;
using Craft.Logging;
using Xunit;
using Craft.DataStructures.Geometry;

namespace Craft.DataStructures.UnitTest;

public class MxCifQuadTreeTest
{
    [Fact]
    public void Test1_Insert12Rectangles_And_CheckForIntersectionWithAnother()
    {
        // Arrange
        var logger = new DummyLogger();

        var rectangle1 = new BoundingBox(7.5, 17.5, 20, 30);
        var rectangle2 = new BoundingBox(49, 51, 5.25, 7.25);
        var rectangle3 = new BoundingBox(51.125, 55.125, 10.5, 14.5);
        var rectangle4 = new BoundingBox(59.5, 65.5, 9.5, 15.5);
        var rectangle5 = new BoundingBox(69.875, 73.875, 10.5, 14.5);
        var rectangle6 = new BoundingBox(47, 53, 34.5, 40.5);
        var rectangle7 = new BoundingBox(15.75, 21.75, 47, 53);
        var rectangle8 = new BoundingBox(45, 55, 45, 55);
        var rectangle9 = new BoundingBox(0, 80, 45, 55);
        var rectangle10 = new BoundingBox(45, 55, 70, 80);
        var rectangle11 = new BoundingBox(70, 80, 70, 80);
        var rectangle12 = new BoundingBox(85.5, 89.5, 91.75, 95.75);

        // Act
        var mxCifQuadTree1 = new MxCifQuadTree.MxCifQuadTree(new BoundingBox(0, 100, 0, 100), logger);

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

        var rectangleQ = new BoundingBox(45, 55, 70, 80);

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
        var rectangle1 = new BoundingBox(7.5, 17.5, 20, 30);
        var rectangle2 = new BoundingBox(49, 51, 5.25, 7.25);
        var rectangle3 = new BoundingBox(51.125, 55.125, 10.5, 14.5);
        var rectangle4 = new BoundingBox(59.5, 65.5, 9.5, 15.5);
        var rectangle5 = new BoundingBox(69.875, 73.875, 10.5, 14.5);
        var rectangle6 = new BoundingBox(47, 53, 34.5, 40.5);
        var rectangle7 = new BoundingBox(15.75, 21.75, 47, 53);
        var rectangle8 = new BoundingBox(45, 55, 45, 55);
        var rectangle9 = new BoundingBox(0, 80, 45, 55);
        var rectangle10 = new BoundingBox(45, 55, 70, 80);
        var rectangle11 = new BoundingBox(70, 80, 70, 80);
        var rectangle12 = new BoundingBox(85.5, 89.5, 91.75, 95.75);

        // Act
        var mxCifQuadTree1 = new MxCifQuadTree.MxCifQuadTree(new BoundingBox(0 ,100, 0, 100), logger);

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

        var mxCifQuadTree = new MxCifQuadTree.MxCifQuadTree(new BoundingBox(0, 100, 0, 100), logger);
        var areaOfIntereset = new BoundingBox(40, 80, 20, 60);

        var lines = File.ReadAllLines(".//Data//all_rectangles.txt");

        using var sw = new StreamWriter(@"C:\Temp\replication_test.svg");
        sw.WriteLine("<svg width=\"100\" height=\"100\" xmlns=\"http://www.w3.org/2000/svg\">");
        sw.WriteLine("  <rect width=\"100\" height=\"100\" x=\"0\" y=\"0\" fill=\"gray\" />");

        sw.WriteLine($"  <rect width=\"{areaOfIntereset.MaxX - areaOfIntereset.MinX}\" height=\"{areaOfIntereset.MaxY - areaOfIntereset.MinY}\" x=\"{areaOfIntereset.MinX}\" y=\"{areaOfIntereset.MinY}\" fill=\"black\" />");

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

            var rectangle = new BoundingBox(centerX - halfWidth, centerX + halfWidth, centerY - halfHeight, centerY + halfHeight);

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
            sw.WriteLine($"  <rect width=\"{rect.MaxX - rect.MinX}\" height=\"{rect.MaxY - rect.MinY}\" x=\"{rect.MinX}\" y=\"{rect.MinY}\" fill=\"yellow\" />");
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
        var rectangleQueue = new Queue<BoundingBox>();
        var mxCifQuadTree = new MxCifQuadTree.MxCifQuadTree(new BoundingBox(0, 100, 0, 100), logger);
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
                var rectangle = new BoundingBox(cx - 0.5 * width, cx + 0.5 * width, cy - 0.5 * height, cy + 0.5 * height);
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
