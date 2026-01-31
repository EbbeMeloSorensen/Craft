namespace Craft.Utils.Linq
{
    public static class Helpers
    {
        // Partitions a sequence into arrays of desired size
        // From: https://stackoverflow.com/questions/4461367/linq-to-objects-return-pairs-of-numbers-from-list-of-numbers
        public static IEnumerable<T[]> Partition<T>(
            this IEnumerable<T> sequence, 
            int partitionSize)
        {
            var buffer = new T[partitionSize];
            var n = 0;
            foreach (var item in sequence)
            {
                buffer[n] = item;
                n += 1;
                if (n != partitionSize) continue;

                yield return buffer;
                buffer = new T[partitionSize];
                n = 0;
            }

            // Partial leftovers
            if (n > 0) yield return buffer;
        }

        // Returns a sequence of adjacent pairs, where all elements except the first and last are part of 2 pairs
        // Example: [1, 2, 3, 4] => [[1, 2], [2, 3], [3, 4]]
        // From: https://stackoverflow.com/questions/1624341/getting-pair-set-using-linq
        public static IEnumerable<Tuple<T, T>> AdjacentPairs<T>(
            this IEnumerable<T> sequence)
        {
            return sequence.Zip(sequence.Skip(1), (first, second) => new Tuple<T, T>(first, second));
        }

        public static IEnumerable<T> Shuffle<T>(
            this IEnumerable<T> sequence,
            Random? random = null)
        {
            random ??= new Random((int)DateTime.UtcNow.Ticks);

            var list = new List<T>(sequence);

            var n = list.Count();

            for (var i = 0; i < n; i++ )
            {
                var k = random.Next(i, n);
                yield return list[k];
                list[k] = list[i];
            }
        }
    }
}
