using System;

namespace Craft.DataStructures.UnitTest.Helpers
{
    /// <summary>
    /// The sole purpose of this class is to act as a heap element in the testing of the Heap class
    /// </summary>
    public class DummyComparable : IComparable
    {
        public int Age { get; private set; }

        public DummyComparable(int age)
        {
            Age = age;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var other = obj as DummyComparable;

            if (other != null)
            {
                return this.Age.CompareTo(other.Age);
            }

            throw new ArgumentException("Object is not a MyComparable");
        }

        public override string ToString()
        {
            return Age.ToString();
        }
    }
}
