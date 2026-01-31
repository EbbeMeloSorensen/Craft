using Xunit;
using FluentAssertions;
using Craft.DataStructures.UnitTest.Helpers;

namespace Craft.DataStructures.UnitTest
{
    public class HeapTest
    {
        [Fact]
        public void MakeHeap()
        {
            var heap = new Heap<DummyComparable>();

            heap.Insert(new DummyComparable(7));
            heap.Insert(new DummyComparable(9));
            heap.Insert(new DummyComparable(13));
            heap.Insert(new DummyComparable(5));
            heap.Insert(new DummyComparable(11));
            heap.Insert(new DummyComparable(16));
            heap.Insert(new DummyComparable(14));

            heap.Elements
                .Select(x => x.Age)
                .SequenceEqual(new[] {16, 11, 14, 5, 7, 9, 13})
                .Should().BeTrue();

            heap.PopPrimary();

            heap.Elements
                .Select(x => x.Age)
                .SequenceEqual(new[] {14, 11, 13, 5, 7, 9})
                .Should().BeTrue();

            heap.PopPrimary();

            heap.Elements
                .Select(x => x.Age)
                .SequenceEqual(new[] {13, 11, 9, 5, 7})
                .Should().BeTrue();

            var a = 1;
            var c = a.CompareTo(2);
        }

        [Fact]
        public void FunWithGenericHeapUsedForInts()
        {
            var heap = new Heap<int>();
            heap.Insert(7);
            heap.Insert(9);
            heap.Insert(13);
            heap.Insert(5);
            heap.Insert(11);
            heap.Insert(16);
            heap.Insert(14);

            heap.Elements
                .SequenceEqual(new[] { 16, 11, 14, 5, 7, 9, 13 })
                .Should().BeTrue();
        }
    }
}
