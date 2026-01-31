using Craft.Utils.Linq;

namespace Craft.Math
{
    public enum LineSegmentPart
    {
        Point1,
        MiddleSection,
        Point2
    }

    public static class Operations
    {
        private static double _toleranceForComparingWithZero = double.Epsilon;

        public static bool IsPositive(this double x)
        {
            return x > 0;
        }

        // Calculates the squared distance between 2 points
        public static double SquaredDistanceTo(
            this Point2D p1,
            Point2D p2)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;

            return dx * dx + dy * dy;
        }

        // Calculates the squared distance between 2 points represented by vectors
        public static double SquaredDistanceTo(
            this Vector2D v1,
            Vector2D v2)
        {
            var dx = v2.X - v1.X;
            var dy = v2.Y - v1.Y;

            return dx * dx + dy * dy;
        }

        public static double DistanceToClosestPoint(
            this IEnumerable<Vector2D> points,
            Vector2D point)
        {
            if (!points.Any())
            {
                return double.MaxValue;
            }

            var minSqrDist = points.Min(_ => _.SquaredDistanceTo(point));

            return minSqrDist < double.Epsilon ? 0.0 : System.Math.Sqrt(minSqrDist);
        }

        // Determines whether 2 line segments intersect. 
        public static bool Intersects(
            this LineSegment2D l1, 
            LineSegment2D l2)
        {
            var p1 = l1.Point1;
            var q1 = l1.Point2;
            var p2 = l2.Point1;
            var q2 = l2.Point2;

            // Find the four orientations needed for general and 
            // special cases 
            var o1 = Orientation(p1, q1, p2);
            var o2 = Orientation(p1, q1, q2);
            var o3 = Orientation(p2, q2, p1);
            var o4 = Orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases 
        }

        // Identifies the intersection point between the two lines defined by the line segments l1 and l2.
        // Note that the lines will have an intersection point unless they are parallel.
        // If the lines are parallel, an exception will be thrown
        public static bool IntersectionPointWith(
            this LineSegment2D l1,
            LineSegment2D l2,
            out Point2D intersection)
        {
            var x1 = l1.Point1.X;
            var y1 = l1.Point1.Y;
            var x2 = l1.Point2.X;
            var y2 = l1.Point2.Y;
            var x3 = l2.Point1.X;
            var y3 = l2.Point1.Y;
            var x4 = l2.Point2.X;
            var y4 = l2.Point2.Y;

            var denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (System.Math.Abs(denominator) < double.Epsilon)
            {
                intersection = null;
                return false;
            }

            var a = x1 * y2 - y1 * x2;
            var b = x3 * y4 - y3 * x4;

            intersection = new Point2D(
                (a * (x3 - x4) - b * (x1 - x2)) / denominator,
                (a * (y3 - y4) - b * (y1 - y2)) / denominator);

            return true;
        }

        // Helper for the method IdentifyVisibleRegion in Craft.Algorithms
        // On line: 0
        // Left: 1
        // Right: 2
        public static int SideOfLine(
            Point2D p,
            Point2D l1,
            Point2D l2)
        {
            // "Hatted" version of vector going from l1 to l2
            var px = l1.Y - l2.Y;
            var py = l2.X - l1.X;

            // Vector going from l1 to p
            var qx = p.X - l1.X;
            var qy = p.Y - l1.Y;

            var dotProduct = px * qx + py * qy;

            if (System.Math.Abs(dotProduct) < _toleranceForComparingWithZero)
            {
                return 0;
            }

            return dotProduct > 0 ? 1 : 2;
        }

        // From http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/

        // Calculates the distance between a line segment and a point
        public static double DistanceTo(
            this LineSegment2D l,
            Point2D p)
        {
            var dx = l.Point2.X - l.Point1.X;
            var dy = l.Point2.Y - l.Point1.Y;

            if (System.Math.Abs(dx) < 0.000001 &&
                System.Math.Abs(dy) < 0.000001)
            {
                // It's a point not a line segment
                dx = p.X - l.Point1.X;
                dy = p.Y - l.Point1.Y;
                return System.Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance
            var t = ((p.X - l.Point1.X) * dx + (p.Y - l.Point1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's end points or a point in the middle
            if (t < 0)
            {
                dx = l.Point1.X - p.X;
                dy = l.Point1.Y - p.Y;
            }
            else if (t > 1)
            {
                dx = l.Point2.X - p.X;
                dy = l.Point2.Y - p.Y;
            }
            else
            {
                dx = l.Point1.X + t * dx - p.X;
                dy = l.Point1.Y + t * dy - p.Y;
            }

            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        public static LineSegmentPart ClosestPartOfLineSegment(
            this LineSegment2D l,
            Point2D p)
        {
            var dx = l.Point2.X - l.Point1.X;
            var dy = l.Point2.Y - l.Point1.Y;

            if (System.Math.Abs(dx) < 0.000001 &&
                System.Math.Abs(dy) < 0.000001)
            {
                return LineSegmentPart.Point1;
            }

            // Calculate the t that minimizes the distance
            var t = ((p.X - l.Point1.X) * dx + (p.Y - l.Point1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's end points or a point in the middle
            if (t < 0)
            {
                return LineSegmentPart.Point1;
            }

            return t > 1 ? LineSegmentPart.Point2 : LineSegmentPart.MiddleSection;
        }

        public static bool Overlaps(
            this Tuple<double, double> interval,
            Tuple<double, double> other)
        {
            return interval.Item1 < other.Item2 && other.Item1 < interval.Item2;
        }

        public static bool Overlaps(
            this Tuple<DateTime, DateTime> timeInterval,
            Tuple<DateTime, DateTime> other)
        {
            return timeInterval.Item1 < other.Item2 && other.Item1 < timeInterval.Item2;
        }

        public static bool Covers(
            this Tuple<DateTime, DateTime> timeInterval,
            Tuple<DateTime, DateTime> other)
        {
            return timeInterval.Item1 <= other.Item1 && other.Item2 <= timeInterval.Item2;
        }

        public static bool AnyOverlaps(
            this IEnumerable<Tuple<DateTime, DateTime>> sortedDateRanges)
        {
            return sortedDateRanges.AdjacentPairs().Any(_ => _.Item1.Overlaps(_.Item2));
        }

        public static bool AnyGaps(
            this IEnumerable<Tuple<DateTime, DateTime>> sortedDateRanges)
        {
            return sortedDateRanges.AdjacentPairs().Any(_ => _.Item1.Item2 < _.Item2.Item1);
        }

        public static double SquaredDistanceTo(
            this LineSegment2D l,
            Point2D p)
        {
            var dx = l.Point2.X - l.Point1.X;
            var dy = l.Point2.Y - l.Point1.Y;

            if (System.Math.Abs(dx) < 0.000001 &&
                System.Math.Abs(dy) < 0.000001)
            {
                // It's a point not a line segment
                dx = p.X - l.Point1.X;
                dy = p.Y - l.Point1.Y;
                return dx * dx + dy * dy;
            }

            // Calculate the t that minimizes the distance
            var t = ((p.X - l.Point1.X) * dx + (p.Y - l.Point1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's end points or a point in the interior part of the line segment
            if (t < 0)
            {
                dx = l.Point1.X - p.X;
                dy = l.Point1.Y - p.Y;
            }
            else if (t > 1)
            {
                dx = l.Point2.X - p.X;
                dy = l.Point2.Y - p.Y;
            }
            else
            {
                dx = l.Point1.X + t * dx - p.X;
                dy = l.Point1.Y + t * dy - p.Y;
            }

            return dx * dx + dy * dy;
        }

        // Helper for determining whether 2 line segments intersect. 
        private static int Orientation(
            Point2D p,
            Point2D q,
            Point2D r)
        {
            var val = (q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y);

            if (System.Math.Abs(val) < _toleranceForComparingWithZero)
            {
                return 0; // colinear 
            }

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // Helper for determining whether 2 line segments intersect. 
        private static bool OnSegment(
            Point2D p,
            Point2D q,
            Point2D r)
        {
            return q.X <= System.Math.Max(p.X, r.X) && q.X >= System.Math.Min(p.X, r.X) &&
                   q.Y <= System.Math.Max(p.Y, r.Y) && q.Y >= System.Math.Min(p.Y, r.Y);
        }
    }
}