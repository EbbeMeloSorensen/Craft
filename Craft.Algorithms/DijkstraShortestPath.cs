using Craft.DataStructures;
using Craft.DataStructures.Graph;

namespace Craft.Algorithms
{
    public static class DijkstraShortestPath
    {
        public static void ComputeDistances(
            this IGraph graph,
            IEnumerable<int> sourceIndexes,
            HashSet<int> forbiddenIndexes,
            double maxCost,
            out double[] distances)
        {
            var distancesResult = Enumerable.Repeat(double.MaxValue, graph.VertexCount).ToArray();
            var shortestPathTreeSet = new bool[graph.VertexCount];
            var heap = new Heap<HeapElement>();

            foreach (var sourceIndex in sourceIndexes)
            {
                heap.Insert(new HeapElement(0, sourceIndex));
            }

            while (!heap.IsEmpty())
            {
                var primary = heap.PopPrimary();

                if (shortestPathTreeSet[primary.Id])
                {
                    continue; // Apparently this is an estimate that was later improved by a better one
                }

                // If the cost of getting to the next vertex exceeds max cost, then we're done
                if (primary.Key > maxCost)
                {
                    break;
                }

                // Now we "claim" the next vertex
                distancesResult[primary.Id] = primary.Key;
                shortestPathTreeSet[primary.Id] = true;

                graph.OutgoingEdges(primary.Id).ToList().ForEach(edge =>
                {
                    if (forbiddenIndexes != null && forbiddenIndexes.Contains(edge.VertexId2))
                    {
                        return;
                    }

                    if (shortestPathTreeSet[edge.VertexId2])
                    {
                        return; // The final distance has already been calculated for this vertex
                    }

                    // Calculate the (added) cost of going along the edge
                    var incrementalDistance = ((EdgeWithCost)edge).Cost;
                    var totalDistanceViaCurrentNode = distancesResult[primary.Id] + incrementalDistance;

                    if (!(distancesResult[edge.VertexId2] > 9999999) &&
                        !(distancesResult[edge.VertexId2] > totalDistanceViaCurrentNode))
                    {
                        return;
                    }

                    // We haven't visited this vertex yet, so insert it into the priority queue and proceed
                    heap.Insert(new HeapElement(totalDistanceViaCurrentNode, edge.VertexId2));

                    // Update the distance map
                    distancesResult[edge.VertexId2] = totalDistanceViaCurrentNode;
                });
            }

            // Make sure no cost exceeds max cost
            distances = distancesResult
                .Select(d => d > maxCost ? double.MaxValue : d)
                .ToArray();

            distances = distancesResult;
        }

        // Same as above, but here you also get a map of previous indexes, so you can
        // determine the path to a given destination
        public static void ComputeDistances(
            this IGraph graph,
            IEnumerable<int> sourceIndexes,
            HashSet<int> forbiddenIndexes,
            double maxCost,
            out double[] distances,
            out int[] previous)
        {
            var distancesResult = Enumerable.Repeat(double.MaxValue, graph.VertexCount).ToArray();
            var previousResult = Enumerable.Repeat(-1, graph.VertexCount).ToArray();
            var shortestPathTreeSet = new bool[graph.VertexCount];
            var heap = new Heap<HeapElement>();

            foreach (var sourceIndex in sourceIndexes)
            {
                heap.Insert(new HeapElement(0, sourceIndex));
            }

            while (!heap.IsEmpty())
            {
                var primary = heap.PopPrimary();

                if (shortestPathTreeSet[primary.Id])
                {
                    // Apparently this is an estimate that was later improved by a better one
                    continue; 
                }

                // If the cost of getting to the next node exceeds max cost, then we're done
                if (primary.Key > maxCost)
                {
                    break;
                }

                // Now we "claim" the next node
                distancesResult[primary.Id] = primary.Key;
                shortestPathTreeSet[primary.Id] = true;

                graph.OutgoingEdges(primary.Id).ToList().ForEach(edge =>
                {
                    if (forbiddenIndexes != null && forbiddenIndexes.Contains(edge.VertexId2))
                    {
                        return;
                    }

                    if (shortestPathTreeSet[edge.VertexId2])
                    {
                        return; // The final distance has already been calculated for this vertex
                    }

                    // Calculate the (added) cost of going along the edge
                    var incrementalDistance = ((EdgeWithCost)edge).Cost;
                    var totalDistanceViaCurrentNode = distancesResult[primary.Id] + incrementalDistance;

                    if (!(distancesResult[edge.VertexId2] > 9999999) &&
                        !(distancesResult[edge.VertexId2] > totalDistanceViaCurrentNode))
                    {
                        return;
                    }

                    // We haven't visited this vertex yet, so insert it into the priority queue and proceed
                    heap.Insert(new HeapElement(totalDistanceViaCurrentNode, edge.VertexId2));

                    // Update the distance map and prevoious map
                    distancesResult[edge.VertexId2] = totalDistanceViaCurrentNode;
                    previousResult[edge.VertexId2] = primary.Id;
                });
            }

            distances = distancesResult
                .Select(d => d > maxCost ? double.MaxValue : d)
                .ToArray();

            previous = previousResult;
        }

        public static int[] DeterminePath(
            this int[] previous,
            int destinationIndex)
        {
            var indexes = new List<int> { destinationIndex };
            var currentIndex = destinationIndex;

            while (previous[currentIndex] != -1)
            {
                currentIndex = previous[currentIndex];
                indexes.Add(currentIndex);
            }

            indexes.Reverse();

            return indexes.ToArray();
        }
    }
}