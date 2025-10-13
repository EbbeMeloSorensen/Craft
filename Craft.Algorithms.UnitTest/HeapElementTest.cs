using Xunit;
using FluentAssertions;

namespace Craft.Algorithms.UnitTest
{
    public class HeapElementTest
    {
        [Fact]
        public void CompareTo()
        {
            // Arrange
            var heapElement1 = new HeapElement(1.2, 0);
            var heapElement2 = new HeapElement(1.2, 1);
            var heapElement3 = new HeapElement(2.2, 2);

            // Act
            var heapElement1vsHeapElement2 = heapElement1.CompareTo(heapElement2);
            var heapElement1vsHeapElement3 = heapElement1.CompareTo(heapElement3);
            var heapElement3vsHeapElement1 = heapElement3.CompareTo(heapElement1);

            // Assert
            heapElement1vsHeapElement2.Should().Be(0);
            heapElement1vsHeapElement3.Should().Be(1);
            heapElement3vsHeapElement1.Should().Be(-1);
        }
    }
}