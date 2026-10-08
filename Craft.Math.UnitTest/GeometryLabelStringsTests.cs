using Craft.Math;
using Xunit;

namespace Craft.Math.UnitTest;

public class GeometryLabelStringsTests
{
    [Fact]
    public void AllLabeledTypesCopyStringsAndExposeAnImmutableOrderedList()
    {
        var source = new List<string> { "identifier", "info", "more info" };
        ILabeledGeometry[] objects = {
            new LabeledPoint2D(1, 2, source),
            new LabeledOrientedPoint2D(1, 2, 45, source),
            new LabeledLineSegment2D(new Point2D(1, 2), new Point2D(3, 4), source)
        };
        source[0] = "changed";
        source.Add("new");
        Assert.All(objects, item => {
            Assert.Equal("identifier", item.Identifier);
            Assert.Equal(new[] { "identifier", "info", "more info" }, item.Labels);
            Assert.Throws<NotSupportedException>(() => ((IList<string>)item.Labels)[0] = "mutation");
        });
    }

    [Fact]
    public void NullAndEmptyListsOrNullStringsAreRejected()
    {
        Func<IEnumerable<string>, ILabeledGeometry>[] constructors = {
            labels => new LabeledPoint2D(1, 2, labels),
            labels => new LabeledOrientedPoint2D(1, 2, 45, labels),
            labels => new LabeledLineSegment2D(new Point2D(1, 2), new Point2D(3, 4), labels)
        };
        Assert.All(constructors, create => {
            Assert.Throws<ArgumentNullException>(() => create(null!));
            Assert.Throws<ArgumentException>(() => create(Array.Empty<string>()));
            Assert.Throws<ArgumentException>(() => create(new[] { "id", null! }));
            Assert.Throws<ArgumentException>(() => create(new[] { "", "information" }));
            Assert.Throws<ArgumentException>(() => create(new[] { " ", "information" }));
            Assert.Throws<ArgumentException>(() => create(new[] { "id", "" }));
            Assert.Throws<ArgumentException>(() => create(new[] { "id", " \t\n" }));
        });
    }
}
