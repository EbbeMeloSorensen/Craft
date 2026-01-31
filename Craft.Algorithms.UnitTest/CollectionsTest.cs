using Xunit;
using FluentAssertions;

namespace Craft.Algorithms.UnitTest
{
    public class CollectionsTest
    {
        [Fact]
        public void CompareWithOtherStringList_ReturnsCorrectResult_1()
        {
            // Arrange
            var list1 = new List<string>
            {
                "Bamse",
                "Kylling",
                "Aske"
            };

            var list2 = new List<string>
            {
                "Bamse",
                "Kylling",
                "Luna"
            };

            // Act
            List<string> stringsOnlyPresentInList1;
            List<string> stringsOnlyPresentInList2;

            list1.CompareWithOtherStringList(
                list2, 
                out stringsOnlyPresentInList1,
                out stringsOnlyPresentInList2);

            // Assert
            stringsOnlyPresentInList1.Single().Should().Be("Aske");
            stringsOnlyPresentInList2.Single().Should().Be("Luna");
        }
    }
}
