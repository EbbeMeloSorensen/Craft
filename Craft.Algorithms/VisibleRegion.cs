using Craft.DataStructures.Graph;
using Craft.Math;

namespace Craft.Algorithms
{
    public static class VisibleRegion
    {
        public static List<Triangle2D> IdentifyVisibleRegion(
            GraphAdjacencyList<Point2DVertex, EmptyEdge> wallGraph,
            Point2D viewPoint)
        {
            // Order the wall edge points by angle from the view point
            var ordered = wallGraph.Vertices
                .Select(v => new
                {
                    Vertex = v,
                    new Vector2D(v.X - viewPoint.X, v.Y - viewPoint.Y).AsPolarVector().Angle
                })
                .OrderBy(x => x.Angle)
                .Select(x => x.Vertex)
                .ToList();

            var initialRay = new LineSegment2D(viewPoint, viewPoint - new Vector2D(1000, 0));
            var open = new List<EmptyEdge>();

            // Identify edges that are intersected by the initial ray
            foreach (var edge in wallGraph.Edges)
            {
                var v1 = wallGraph.Vertices[edge.VertexId1];
                var v2 = wallGraph.Vertices[edge.VertexId2];

                if (v1.Y >= viewPoint.Y &&
                    v2.Y >= viewPoint.Y ||
                    !initialRay.Intersects(new LineSegment2D(
                        new Point2D(v1.X, v1.Y),
                        new Point2D(v2.X, v2.Y))))
                {
                    continue;
                }

                open.Add(edge);
            }

            // The last point defines the left edge of the first triangle
            var vBefore = ordered.Last();

            var result = new List<Triangle2D>();

            // Proces each triangle clock wise
            ordered.ForEach(vCurrent =>
            {
                // Ud af de edges, der er i open (som vel at mærke ikke kan intersecte i trekanten)
                // skal vi nu identificere den, der er tættest på view point.
                var lineLeft = new LineSegment2D(viewPoint, new Point2D(vBefore.X, vBefore.Y));
                var lineRight = new LineSegment2D(viewPoint, new Point2D(vCurrent.X, vCurrent.Y));
                var minSqrDistLeft = double.MaxValue;
                var minSqrDistRight = double.MaxValue;
                Point2D closestIntersectionPointLeft = null;
                Point2D closestIntersectionPointRight = null;

                open.ForEach(e =>
                {
                    var vertex1 = wallGraph.Vertices[e.VertexId1];
                    var vertex2 = wallGraph.Vertices[e.VertexId2];

                    var lineCorrespondingToCurrentEdge = new LineSegment2D(
                        new Point2D(vertex1.X, vertex1.Y),
                        new Point2D(vertex2.X, vertex2.Y));

                    var intersectsLeft = lineLeft.IntersectionPointWith(
                        lineCorrespondingToCurrentEdge, out var intersectionPointLeft);

                    var intersectsRight = lineRight.IntersectionPointWith(
                        lineCorrespondingToCurrentEdge, out var intersectionPointRight);

                    if (!intersectsLeft || !intersectsRight)
                    {
                        return;
                    }

                    var sqrDistLeft = intersectionPointLeft.SquaredDistanceTo(viewPoint);
                    var sqrDistRight = intersectionPointRight.SquaredDistanceTo(viewPoint);

                    if (sqrDistLeft < minSqrDistLeft)
                    {
                        minSqrDistLeft = sqrDistLeft;
                        closestIntersectionPointLeft = intersectionPointLeft;
                    }

                    if (sqrDistRight < minSqrDistRight)
                    {
                        minSqrDistRight = sqrDistRight;
                        closestIntersectionPointRight = intersectionPointRight;
                    }
                });

                result.Add(new Triangle2D(
                    viewPoint,
                    closestIntersectionPointLeft,
                    closestIntersectionPointRight));

                // Nu skal vi fjerne de edges fra "open", som har vCurrent som det ene endepunkt,
                // og derudover har sit andet endepunkt liggende bag ved linien.
                var edges = wallGraph.GetAdjacentEdges(vCurrent.Id);
                edges.ToList().ForEach(e =>
                {
                    var oppositeVertexId = e.GetOppositeVertexId(vCurrent.Id);
                    var oppositeVertex = wallGraph.Vertices[oppositeVertexId];
                    var oppositePoint = new Point2D(oppositeVertex.X, oppositeVertex.Y);

                    if (Operations.SideOfLine(oppositePoint, viewPoint, new Point2D(vCurrent.X, vCurrent.Y)) == 1)
                    {
                        open.Add(e);
                    }
                    else
                    {
                        open.Remove(e);
                    }
                });

                vBefore = vCurrent;
            });

            return result;
        }

        public static void Rasterize(
            this Circle2D circle2D,
            int[,] raster,
            int lowestValueToReplace,
            int drawValue)
        {
            // Identify bounding box for the circle
            var x0 = (int)System.Math.Ceiling(circle2D.Center.X - circle2D.Radius);
            var y0 = (int)System.Math.Ceiling(circle2D.Center.Y - circle2D.Radius);
            var x1 = (int)System.Math.Floor(circle2D.Center.X + circle2D.Radius) + 1;
            var y1 = (int)System.Math.Floor(circle2D.Center.Y + circle2D.Radius) + 1;

            // Make sure the bounding box lies within the raster
            x0 = System.Math.Max(x0, 0);
            x1 = System.Math.Min(x1, raster.GetLength(1));
            y0 = System.Math.Max(y0, 0);
            y1 = System.Math.Min(y1, raster.GetLength(0));

            var squaredRadius = System.Math.Pow(circle2D.Radius, 2);

            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    if (raster[y, x] < lowestValueToReplace)
                    {
                        continue;
                    }

                    var rasterPoint = new Point2D(x, y);

                    if (rasterPoint.SquaredDistanceTo(circle2D.Center) <= squaredRadius)
                    {
                        raster[y, x] = drawValue;
                    }
                }
            }
        }

        public static void Rasterize(
            this Triangle2D triangle,
            int[,] raster,
            int lowestValueToReplace,
            int drawValue)
        {
            // The triangle points need to be in a clockwise sequence

            // Identify bounding box for the triangle
            var minX = System.Math.Min(triangle.Point1.X, triangle.Point2.X);
            minX = System.Math.Min(minX, triangle.Point3.X);

            var maxX = System.Math.Max(triangle.Point1.X, triangle.Point2.X);
            maxX = System.Math.Max(maxX, triangle.Point3.X);

            var minY = System.Math.Min(triangle.Point1.Y, triangle.Point2.Y);
            minY = System.Math.Min(minY, triangle.Point3.Y);

            var maxY = System.Math.Max(triangle.Point1.Y, triangle.Point2.Y);
            maxY = System.Math.Max(maxY, triangle.Point3.Y);

            var x0 = (int) System.Math.Ceiling(minX);
            var y0 = (int) System.Math.Ceiling(minY);
            var x1 = (int) System.Math.Floor(maxX) + 1;
            var y1 = (int) System.Math.Floor(maxY) + 1;

            // Make sure the bounding box lies within the raster
            x0 = System.Math.Max(x0, 0);
            x1 = System.Math.Min(x1, raster.GetLength(1));
            y0 = System.Math.Max(y0, 0);
            y1 = System.Math.Min(y1, raster.GetLength(0));

            // Evaluate each raster point by means of barycentric coordinates as explained here
            // https://fgiesen.wordpress.com/2013/02/08/triangle-rasterization-in-practice/
            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    if (raster[y, x] < lowestValueToReplace)
                    {
                        continue;
                    }

                    var rasterPoint = new Point2D(x, y);

                    if (Orient2d(triangle.Point1, triangle.Point2, rasterPoint) < 0 ||
                        Orient2d(triangle.Point2, triangle.Point3, rasterPoint) < 0 ||
                        Orient2d(triangle.Point3, triangle.Point1, rasterPoint) < 0)
                    {
                        continue;
                    }

                    raster[y, x] = drawValue;
                }
            }
        }

        public static void PixelwiseMax(
            this int[,] raster1,
            int[,] raster2)
        {
            var rows = raster1.GetLength(0);
            var cols = raster1.GetLength(1);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    raster1[r, c] = System.Math.Max(raster1[r, c], raster2[r, c]);
                }
            }
        }

        public static void PixelWiseAnd(
            this int[,] raster1,
            int[,] raster2)
        {
            var rows = raster1.GetLength(0);
            var cols = raster1.GetLength(1);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (raster1[r, c] == 1 && raster2[r, c] == 1)
                    {
                        raster1[r, c] = 1;
                    }
                    else
                    {
                        raster1[r, c] = 0;
                    }
                }
            }
        }

        public static void Threshold(
            this int[,] raster,
            int threshold)
        {
            var rows = raster.GetLength(0);
            var cols = raster.GetLength(1);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (raster[r, c] < threshold)
                    {
                        raster[r, c] = 0;
                    }
                    else
                    {
                        raster[r, c] = 1;
                    }
                }
            }
        }

        public static int[,] Threshold(
            this double[,] raster,
            double threshold)
        {
            var result = new int[raster.GetLength(0), raster.GetLength(1)];

            var rows = raster.GetLength(0);
            var cols = raster.GetLength(1);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (raster[r, c] <= threshold)
                    {
                        result[r, c] = 1;
                    }
                    else
                    {
                        result[r, c] = 0;
                    }
                }
            }

            return result;
        }

        public static void Invert(
            this int[,] raster)
        {
            var rows = raster.GetLength(0);
            var cols = raster.GetLength(1);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (raster[r, c] == 0)
                    {
                        raster[r, c] = 1;
                    }
                    else
                    {
                        raster[r, c] = 0;
                    }
                }
            }
        }

        public static int[,] ConvertToRaster(
            this IEnumerable<int> indexes,
            int width,
            int height)
        {
            var result = new int[height, width];

            indexes
                .ToList()
                .ForEach(i => result[i / width, i % width] = 1);

            return result;
        }

        public static IEnumerable<int> ConvertToIndexes(
            this int[,] raster)
        {
            var rows = raster.GetLength(0);
            var cols = raster.GetLength(1);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (raster[r, c] > 0)
                    {
                        yield return r * cols + c;
                    }
                }
            }
        }

        public static double[,] ConvertTo2DArray(
            this IEnumerable<double> values,
            int width,
            int height)
        {
            var result = new double[height, width];

            var index = 0;
            foreach (var v in values)
            {
                result[index / width, index % width] = v;
                index++;
            }

            return result;
        }

        public static void Dilate(
            this int[,] raster,
            double radius)
        {
            DistanceTransform.EuclideanDistanceTransform(
                raster, out var distances, out var xValues, out var yValues);

            var rows = raster.GetLength(0);
            var cols = raster.GetLength(1);

            for (var x = 0; x < cols; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    if (distances[y, x] > radius) continue;

                    raster[y, x] = 1;
                }
            }
        }

        public static void Erode(
            this int[,] raster,
            double radius)
        {
            throw new NotImplementedException();
        }

        public static void Open(
            this int[,] raster,
            double radius)
        {
            throw new NotImplementedException();
        }

        public static void Close(
            this int[,] raster,
            double radius)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<int> IdentifyIndexesLowerThan(
            this double[] values,
            double threshold)
        {
            return values
                .Select((v, i) => new {i, v})
                .Where(x => x.v < threshold)
                .Select(x => x.i);
        }

        public static IEnumerable<int> IdentifyIndexesGreaterThan(
            this double[] values,
            double threshold)
        {
            return values
                .Select((v, i) => new { i, v })
                .Where(x => x.v > threshold)
                .Select(x => x.i);
        }

        public static IEnumerable<int> IdentifyIndexesOfMinimumValue(
            this IEnumerable<int> indexes,
            double[] values)
        {
            var temp = indexes
                .Select(i => new {i, v = values[i]})
                .OrderBy(x => x.v)
                .ToList();

            var minVal = temp.First().v;

            var result = temp
                .TakeWhile(x => System.Math.Abs(x.v - minVal) < 0.000001)
                .Select(x => x.i)
                .ToList();

            return result;
        }

        public static IEnumerable<int> IdentifyIndexesOfMaximumValue(
            this IEnumerable<int> indexes,
            double[] values)
        {
            var temp = indexes
                .Select(i => new { i, v = values[i] })
                .OrderByDescending(x => x.v)
                .ToList();

            var maxVal = temp.First().v;

            var result = temp
                .TakeWhile(x => System.Math.Abs(x.v - maxVal) < 0.000001)
                .Select(x => x.i)
                .ToList();

            return result;
        }

        public static void WriteToFile(
            this int[,] raster,
            string fileName)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                for (var r = 0; r < raster.GetLength(0); r++)
                {
                    for (var c = 0; c < raster.GetLength(1); c++)
                    {
                        streamWriter.Write($"{raster[r,c]},");
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        public static void WriteToFile(
            this double[,] raster,
            string fileName)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                for (var r = 0; r < raster.GetLength(0); r++)
                {
                    for (var c = 0; c < raster.GetLength(0); c++)
                    {
                        var val = raster[r, c];

                        if (val < 999999)
                        {
                            streamWriter.Write($"{val:F4},");
                        }
                        else
                        {
                            streamWriter.Write("      ,");
                        }
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        private static double Orient2d(
            Point2D a,
            Point2D b,
            Point2D c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }
    }
}
