using System;

namespace Craft.Algorithms
{
    public class HeapElement : IComparable
    {
        public double Key { get; private set; }
        public int Id { get; private set; }

        public HeapElement(double key, int id)
        {
            Key = key;
            Id = id;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var other = obj as HeapElement;

            if (other != null)
            {
                return -Key.CompareTo(other.Key);
            }

            throw new ArgumentException("Object is not a HeapElement");
        }

        public override string ToString()
        {
            return $"({Id}:{Key})";
        }
    }
}
