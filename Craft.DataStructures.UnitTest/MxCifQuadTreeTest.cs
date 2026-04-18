using System.Diagnostics;
using FluentAssertions;
using System.Globalization;
using Craft.Logging;
using Xunit;
using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;

namespace Craft.DataStructures.UnitTest;

public class MxCifQuadTreeTest
{
    [Fact]
    public void Test1_Insert12Rectangles_And_CheckForIntersectionWithAnother()
    {
        // Arrange
        var logger = new DummyLogger();

        var spatialItem1 = new SpatialItem<Line>(new BoundingBox(7.5, 17.5, 20, 30), new Line());
        var spatialItem2 = new SpatialItem<Line>(new BoundingBox(49, 51, 5.25, 7.25), new Line());
        var spatialItem3 = new SpatialItem<Line>(new BoundingBox(51.125, 55.125, 10.5, 14.5), new Line());
        var spatialItem4 = new SpatialItem<Line>(new BoundingBox(59.5, 65.5, 9.5, 15.5), new Line());
        var spatialItem5 = new SpatialItem<Line>(new BoundingBox(69.875, 73.875, 10.5, 14.5), new Line());
        var spatialItem6 = new SpatialItem<Line>(new BoundingBox(47, 53, 34.5, 40.5), new Line());
        var spatialItem7 = new SpatialItem<Line>(new BoundingBox(15.75, 21.75, 47, 53), new Line());
        var spatialItem8 = new SpatialItem<Line>(new BoundingBox(45, 55, 45, 55), new Line());
        var spatialItem9 = new SpatialItem<Line>(new BoundingBox(0, 80, 45, 55), new Line());
        var spatialItem10 = new SpatialItem<Line>(new BoundingBox(45, 55, 70, 80), new Line());
        var spatialItem11 = new SpatialItem<Line>(new BoundingBox(70, 80, 70, 80), new Line());
        var spatialItem12 = new SpatialItem<Line>(new BoundingBox(85.5, 89.5, 91.75, 95.75), new Line());

        // Act
        var mxCifQuadTree1 = new MxCifQuadTree.MxCifQuadTree<Line>(new BoundingBox(0, 100, 0, 100), logger);

        mxCifQuadTree1.Insert(spatialItem1);
        mxCifQuadTree1.Insert(spatialItem2);
        mxCifQuadTree1.Insert(spatialItem3);
        mxCifQuadTree1.Insert(spatialItem4);
        mxCifQuadTree1.Insert(spatialItem5);
        mxCifQuadTree1.Insert(spatialItem6);
        mxCifQuadTree1.Insert(spatialItem7);
        mxCifQuadTree1.Insert(spatialItem8);
        mxCifQuadTree1.Insert(spatialItem9);
        mxCifQuadTree1.Insert(spatialItem10);
        mxCifQuadTree1.Insert(spatialItem11);
        mxCifQuadTree1.Insert(spatialItem12);

        var rectangleQ = new BoundingBox(45, 55, 70, 80);

        // Assert
        mxCifQuadTree1.Intersects(rectangleQ).Should().BeTrue(); // It intersects rectangle 10

        // Act
        mxCifQuadTree1.Remove(spatialItem1); 
    }

    [Fact]
    public void Test2_InsertSomeRectangles_ThenRemoveThemAgain()
    {
        var logger = new TestLogger();
        logger.IsEnabled = true;

        // Arrange
        var spatialItem1 = new SpatialItem<Line>(new BoundingBox(7.5, 17.5, 20, 30), new Line());
        var spatialItem2 = new SpatialItem<Line>(new BoundingBox(49, 51, 5.25, 7.25), new Line());
        var spatialItem3 = new SpatialItem<Line>(new BoundingBox(51.125, 55.125, 10.5, 14.5), new Line());
        var spatialItem4 = new SpatialItem<Line>(new BoundingBox(59.5, 65.5, 9.5, 15.5), new Line());
        var spatialItem5 = new SpatialItem<Line>(new BoundingBox(69.875, 73.875, 10.5, 14.5), new Line());
        var spatialItem6 = new SpatialItem<Line>(new BoundingBox(47, 53, 34.5, 40.5), new Line());
        var spatialItem7 = new SpatialItem<Line>(new BoundingBox(15.75, 21.75, 47, 53), new Line());
        var spatialItem8 = new SpatialItem<Line>(new BoundingBox(45, 55, 45, 55), new Line());
        var spatialItem9 = new SpatialItem<Line>(new BoundingBox(0, 80, 45, 55), new Line());
        var spatialItem10 = new SpatialItem<Line>(new BoundingBox(45, 55, 70, 80), new Line());
        var spatialItem11 = new SpatialItem<Line>(new BoundingBox(70, 80, 70, 80), new Line());
        var spatialItem12 = new SpatialItem<Line>(new BoundingBox(85.5, 89.5, 91.75, 95.75), new Line());

        // Act
        var mxCifQuadTree1 = new MxCifQuadTree.MxCifQuadTree<Line>(new BoundingBox(0, 100, 0, 100), logger);

        mxCifQuadTree1.Insert(spatialItem1);
        mxCifQuadTree1.Insert(spatialItem2);
        mxCifQuadTree1.Insert(spatialItem3);
        mxCifQuadTree1.Insert(spatialItem4);
        mxCifQuadTree1.Insert(spatialItem5);
        mxCifQuadTree1.Insert(spatialItem6);
        mxCifQuadTree1.Insert(spatialItem7);
        mxCifQuadTree1.Insert(spatialItem8);
        mxCifQuadTree1.Insert(spatialItem9);
        mxCifQuadTree1.Insert(spatialItem10);
        mxCifQuadTree1.Insert(spatialItem11);
        mxCifQuadTree1.Insert(spatialItem12);

        // Act
        mxCifQuadTree1.Remove(spatialItem1);
        mxCifQuadTree1.Remove(spatialItem2);
        mxCifQuadTree1.Remove(spatialItem3);
        mxCifQuadTree1.Remove(spatialItem4);
        mxCifQuadTree1.Remove(spatialItem5);
        mxCifQuadTree1.Remove(spatialItem6);
        mxCifQuadTree1.Remove(spatialItem7);
        mxCifQuadTree1.Remove(spatialItem8);
        mxCifQuadTree1.Remove(spatialItem9);
        mxCifQuadTree1.Remove(spatialItem10);
        mxCifQuadTree1.Remove(spatialItem11);
        mxCifQuadTree1.Remove(spatialItem12);

        logger.Complete();
    }

    [Fact]
    public void Test3_Replicate_TheCPPImplementation()
    {
        // In this test, we verify that we get the same result as in the C++ implementation,
        // When trying to add a the same range of rectangles to an mxcifquadtree and rejecting those that overlap with already inserted rectangles.

        var logger = new TestLogger();

        var mxCifQuadTree = new MxCifQuadTree.MxCifQuadTree<Line>(new BoundingBox(0, 100, 0, 100), logger);
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
            mxCifQuadTree.Insert(new SpatialItem<Line>(rectangle, new Line()));
        }

        var intersectingSpatialItems = mxCifQuadTree
            .GetAllIntersecting(areaOfIntereset)
            .ToList();

        foreach (var spatialItem in intersectingSpatialItems)
        {
            var rect = spatialItem.Bounds;
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
        var spatialItemQueue = new Queue<SpatialItem<Line>>();
        var mxCifQuadTree = new MxCifQuadTree<Line>(new BoundingBox(0, 100, 0, 100), logger);
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
                var boundingBox = new BoundingBox(cx - 0.5 * width, cx + 0.5 * width, cy - 0.5 * height, cy + 0.5 * height);
                var spatialItem = new SpatialItem<Line>(boundingBox, new Line());
                mxCifQuadTree.Insert(spatialItem);
                rectanglesInMxCifQuadTree++;
                spatialItemQueue.Enqueue(spatialItem);
            }

            if (i >= maxNumberOfRectanglesInTree && spatialItemQueue.Count > 0)
            {
                var spatialItemFromQueue = spatialItemQueue.Dequeue();
                mxCifQuadTree.Remove(spatialItemFromQueue);
                rectanglesInMxCifQuadTree--;
            }

            controlList.Add(rectanglesInMxCifQuadTree);
        }

        stopWatch.Stop();
        var elapsed = stopWatch.Elapsed;

        logger.Complete();
    }
}
