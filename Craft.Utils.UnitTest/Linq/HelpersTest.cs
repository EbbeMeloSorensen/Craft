using Craft.Utils.Linq;
using FluentAssertions;

namespace Craft.Utils.UnitTest.Linq
{
    public class HelpersTest
    {
        [Fact]
        public void Shuffle_Randomly_Works()
        {
            // Arrange
            var numberCount = 100;
            var numbers = Enumerable.Range(1, numberCount).ToList();
            var testCount = 10;

            for (var i = 0; i < testCount; i++)
            {
                // Act
                var shuffled = numbers.Shuffle().ToList();

                // Assert
                shuffled.SequenceEqual(numbers).Should().BeFalse();
                shuffled.Count.Should().Be(numberCount);
                shuffled.Sum().Should().Be(numbers.Sum());
            }
        }

        [Fact]
        public void Shuffle_PseudoRandomly_Works()
        {
            // Arrange
            var random = new Random(0);
            var list = Enumerable.Range(1, 10);

            // Act
            list = list.Shuffle(random);

            // Assert
            list.SequenceEqual(new[] { 8, 9, 2, 7, 6, 1, 10, 3, 4, 5 }).Should().BeTrue();
        }
    }
}
