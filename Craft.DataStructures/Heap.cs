namespace Craft.DataStructures
{
    public class Heap<T> where T : IComparable
    {
        private List<T> _elements;

        public List<T> Elements
        {
            get { return _elements.Skip(1).ToList(); }
        }

        public void Insert(T element)
        {
            _elements.Add(element);

            IncreaseKey(_elements.Count - 1);
        }

        public T PopPrimary()
        {
            var primary = _elements[1];

            _elements[1] = _elements.Last();
            _elements = _elements.Take(_elements.Count - 1).ToList(); // ugly

            if (!IsEmpty())
            {
                Heapify(1);
            }

            return primary;
        }

        public bool IsEmpty()
        {
            return _elements.Count == 1;
        }

        public override string ToString()
        {
            return IsEmpty() ? "[]" : string.Concat("[", _elements.Skip(1).Select(e => e.ToString()).Aggregate((c, n) => $"{c}, {n}"), "]");
        }

        public Heap()
        {
            // First element is a dummy, since we don't want elements with zero as index
            _elements = new List<T> { default(T) };
        }

        private void IncreaseKey(int heapIndex)
        {
            var currentHeapIndex = heapIndex;
            var element = _elements[currentHeapIndex];

            var parentIndex = currentHeapIndex >> 1;
            var parentElement = _elements[parentIndex];

            while (parentIndex > 0 && parentElement.CompareTo(element) == -1)
            {
                _elements[currentHeapIndex] = parentElement;
                currentHeapIndex = parentIndex;
                parentIndex = currentHeapIndex >> 1;
                parentElement = _elements[parentIndex];
            }

            _elements[currentHeapIndex] = element;
        }

        private void Heapify(int heapIndex)
        {
            var currentHeapIndex = heapIndex;
            var currentElement = _elements[currentHeapIndex];
            bool done;

            do
            {
                // Identify the largest element among the current one,
                // its left child (if any) and its right child (if any)
                var largestElement = currentElement;
                var indexLargest = currentHeapIndex;

                // Left child
                var indexChild = currentHeapIndex << 1;
                if (indexChild < _elements.Count)
                {
                    var childElement = _elements[indexChild];

                    if (childElement.CompareTo(largestElement) == 1)
                    {
                        largestElement = childElement;
                        indexLargest = indexChild;
                    }

                    // Right child
                    indexChild = indexChild | 1;

                    if (indexChild < _elements.Count)
                    {
                        childElement = _elements[indexChild];

                        if (childElement.CompareTo(largestElement) == 1)
                        {
                            largestElement = childElement;
                            indexLargest = indexChild;
                        }
                    }
                }

                // If the current element doesn't have larger children, then we're done
                done = largestElement.Equals(currentElement);

                if (!done)
                {
                    // Otherwise, switch the current element with its largest child and down the hierarchy
                    _elements[currentHeapIndex] = largestElement;
                    _elements[indexLargest] = currentElement;
                    currentHeapIndex = indexLargest;
                }

            } while (!done);
        }
    }
}
