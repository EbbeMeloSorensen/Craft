using Craft.DataStructures.Geometry;
using Craft.Math;
using Craft.Math.IO;
using Craft.UIElements.Reborn.GuiTest;
using Craft.ViewModels.Geometry2D.Reborn;
using Newtonsoft.Json;
using System.Windows;
using Xunit;

namespace Craft.UIElements.Reborn.UnitTest;

public class LabelListTests
{
    private static MainWindowViewModel CreateEditor()
    {
        var editor = new MainWindowViewModel { DrawingLabel = "point", DrawingLabelNumber = "1" };
        editor.GeometryViewModel.WorldWindowExpanded = new BoundingBox(-100, 100, -100, 100);
        editor.GeometryViewModel.DrawingStrokePoints = new List<Point> { new(1, 2) };
        editor.GeometryViewModel.SelectedGeometricObjects.Add(Assert.Single(Objects(editor)));
        return editor;
    }

    private static object[] Objects(MainWindowViewModel editor) => editor.GeometryViewModel.GeometryLayers
        .SelectMany(layer => layer.GeometricObjects.Cast<object>()).ToArray();

    private static ILabeledGeometry Selected(MainWindowViewModel editor) =>
        Assert.IsAssignableFrom<ILabeledGeometry>(Assert.Single(editor.GeometryViewModel.SelectedGeometricObjects));

    private static void AddInformation(MainWindowViewModel editor, string text)
    {
        editor.AddSelectedInformationCommand.Execute(null);
        editor.SelectedObjectInformation.Last().Text = text;
    }

    private static void AssertDirty(MainWindowViewModel editor, bool expected)
    {
        Assert.Equal(expected, editor.IsSelectedLabelDirty);
        Assert.Equal(expected && editor.SelectedLabelValidationMessage.Length == 0, editor.ApplySelectedLabelCommand.CanExecute(null));
        Assert.Equal(expected, editor.CancelSelectedLabelCommand.CanExecute(null));
    }

    [Fact]
    public void IdentifierEditsEnableCommandsAndRevertingApplyingOrCancelingDisablesThem()
    {
        var editor = CreateEditor();
        AssertDirty(editor, false);
        editor.SelectedObjectLabel = "changed";
        AssertDirty(editor, true);
        editor.SelectedObjectLabel = "point1";
        AssertDirty(editor, false);
        editor.SelectedObjectLabel = "changed";
        editor.ApplySelectedLabelCommand.Execute(null);
        AssertDirty(editor, false);
        Assert.Equal("changed", Selected(editor).Identifier);
        editor.SelectedObjectLabel = "discard";
        AssertDirty(editor, true);
        editor.CancelSelectedLabelCommand.Execute(null);
        AssertDirty(editor, false);
        Assert.Equal("changed", editor.SelectedObjectLabel);
    }

    [Fact]
    public void InformationEditsAndListChangesUpdateCommandAvailabilityImmediately()
    {
        var editor = CreateEditor();
        var notifications = 0;
        editor.ApplySelectedLabelCommand.CanExecuteChanged += (_, _) => notifications++;
        AddInformation(editor, "original");
        AssertDirty(editor, true);
        editor.ApplySelectedLabelCommand.Execute(null);
        AssertDirty(editor, false);
        var row = editor.SelectedObjectInformation.Single();
        var beforeEdit = notifications;
        row.Text = "edited";
        AssertDirty(editor, true);
        Assert.True(notifications > beforeEdit);
        row.Text = "original";
        AssertDirty(editor, false);
        AddInformation(editor, "");
        AssertDirty(editor, true);
        editor.RemoveSelectedInformationCommand.Execute(editor.SelectedObjectInformation.Last());
        AssertDirty(editor, false);
        editor.RemoveSelectedInformationCommand.Execute(row);
        AssertDirty(editor, true);
        editor.CancelSelectedLabelCommand.Execute(null);
        AssertDirty(editor, false);
        var beforeStaleEdit = notifications;
        row.Text = "detached row";
        Assert.Equal(beforeStaleEdit, notifications);
        AssertDirty(editor, false);
    }

    [Fact]
    public void ChangingSelectionDiscardsDirtyStateAndUnsubscribesOldRows()
    {
        var editor = CreateEditor();
        AddInformation(editor, "stored");
        editor.ApplySelectedLabelCommand.Execute(null);
        var selected = Assert.Single(Objects(editor));
        var oldRow = editor.SelectedObjectInformation.Single();
        oldRow.Text = "draft";
        AssertDirty(editor, true);
        editor.GeometryViewModel.SelectedGeometricObjects.Clear();
        AssertDirty(editor, false);
        editor.GeometryViewModel.SelectedGeometricObjects.Add(selected);
        AssertDirty(editor, false);
        Assert.Equal("stored", editor.SelectedObjectInformation.Single().Text);
        oldRow.Text = "stale draft";
        AssertDirty(editor, false);
    }

    [Fact]
    public void InformationRequiresAnIdentifierAndBlankRowsCannotBeApplied()
    {
        var editor = CreateEditor();
        editor.SelectedObjectLabel = "";
        editor.ApplySelectedLabelCommand.Execute(null);
        AssertDirty(editor, false);
        editor.SelectedObjectLabel = " ";
        AssertDirty(editor, true);
        editor.CancelSelectedLabelCommand.Execute(null);
        AssertDirty(editor, false);
        Assert.Equal("", editor.SelectedObjectLabel);
        Assert.False(editor.AddSelectedInformationCommand.CanExecute(null));
        editor.AddSelectedInformationCommand.Execute(null);
        Assert.Empty(editor.SelectedObjectInformation);
        editor.SelectedObjectLabel = "new identifier";
        Assert.True(editor.AddSelectedInformationCommand.CanExecute(null));
        AddInformation(editor, "");
        AssertDirty(editor, true);
        Assert.False(editor.ApplySelectedLabelCommand.CanExecute(null));
        editor.CancelSelectedLabelCommand.Execute(null);
        AssertDirty(editor, false);
    }

    [Fact]
    public void InformationIsDraftedAndAppliedInOrderWithoutChangingDrawingFields()
    {
        var editor = CreateEditor();
        AddInformation(editor, "material: steel");
        AddInformation(editor, "multi\nline");
        Assert.Equal(new[] { "point1" }, Selected(editor).Labels);
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal(new[] { "point1", "material: steel", "multi\nline" }, Selected(editor).Labels);
        Assert.Equal("point", editor.DrawingLabel);
        Assert.Equal("2", editor.DrawingLabelNumber);
        editor.SelectedObjectLabel = "renamed";
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal(new[] { "renamed", "material: steel", "multi\nline" }, Selected(editor).Labels);
    }

    [Fact]
    public void IdentifierCannotBeClearedWhileInformationExists()
    {
        var editor = CreateEditor();
        AddInformation(editor, "keep me");
        editor.ApplySelectedLabelCommand.Execute(null);
        editor.SelectedObjectLabel = "";
        Assert.False(editor.ApplySelectedLabelCommand.CanExecute(null));
        Assert.True(editor.CancelSelectedLabelCommand.CanExecute(null));
        Assert.False(editor.AddSelectedInformationCommand.CanExecute(null));
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal(new[] { "point1", "keep me" }, Selected(editor).Labels);
        editor.RemoveSelectedInformationCommand.Execute(editor.SelectedObjectInformation.Single());
        Assert.True(editor.ApplySelectedLabelCommand.CanExecute(null));
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.IsType<Point2D>(Assert.Single(Objects(editor)));
        Assert.False(editor.AddSelectedInformationCommand.CanExecute(null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void BlankInformationDisablesApplyButAllowsCancelAndCorrection(string invalid)
    {
        var editor = CreateEditor();
        AddInformation(editor, invalid);
        Assert.False(editor.ApplySelectedLabelCommand.CanExecute(null));
        Assert.True(editor.CancelSelectedLabelCommand.CanExecute(null));
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal(new[] { "point1" }, Selected(editor).Labels);
        Assert.NotEmpty(editor.SelectedLabelValidationMessage);
        editor.SelectedObjectInformation.Single().Text = " valid information ";
        Assert.True(editor.ApplySelectedLabelCommand.CanExecute(null));
        Assert.Empty(editor.SelectedLabelValidationMessage);
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal(new[] { "point1", " valid information " }, Selected(editor).Labels);
        editor.SelectedObjectInformation.Single().Text = invalid;
        Assert.False(editor.ApplySelectedLabelCommand.CanExecute(null));
        editor.CancelSelectedLabelCommand.Execute(null);
        Assert.Equal(" valid information ", editor.SelectedObjectInformation.Single().Text);
    }

    [Fact]
    public void CancelAndSelectionChangesDiscardInformationDrafts()
    {
        var editor = CreateEditor();
        AddInformation(editor, "original");
        AddInformation(editor, "second");
        editor.ApplySelectedLabelCommand.Execute(null);
        var stored = Assert.Single(Objects(editor));
        var staleDraft = editor.SelectedObjectInformation[0];
        staleDraft.Text = "draft";
        editor.RemoveSelectedInformationCommand.Execute(editor.SelectedObjectInformation[1]);
        AddInformation(editor, "new draft");
        editor.CancelSelectedLabelCommand.Execute(null);
        Assert.Equal(new[] { "original", "second" }, editor.SelectedObjectInformation.Select(i => i.Text));
        Assert.Equal(new[] { "point1", "original", "second" }, Selected(editor).Labels);
        staleDraft.Text = "must not leak";
        Assert.Equal("original", editor.SelectedObjectInformation[0].Text);
        editor.SelectedObjectInformation[0].Text = "discard on selection";
        editor.GeometryViewModel.SelectedGeometricObjects.Clear();
        Assert.Empty(editor.SelectedObjectInformation);
        Assert.False(editor.AddSelectedInformationCommand.CanExecute(null));
        editor.GeometryViewModel.SelectedGeometricObjects.Add(stored);
        Assert.Equal("original", editor.SelectedObjectInformation[0].Text);
        editor.GeometryViewModel.CanvasMode = CanvasMode.Draw;
        Assert.Empty(editor.SelectedObjectInformation);
    }

    [Fact]
    public void InformationEditsAffectOnlySelectedPolylineSegment()
    {
        var editor = CreateEditor();
        editor.GeometryViewModel.DrawingStrokePoints = new List<Point> { new(0, 0), new(10, 10), new(20, 0) };
        var segments = Objects(editor).OfType<LabeledLineSegment2D>().ToArray();
        Assert.All(segments, segment => Assert.Equal(new[] { "point2" }, segment.Labels));
        editor.GeometryViewModel.SelectedGeometricObjects.Clear();
        editor.GeometryViewModel.SelectedGeometricObjects.Add(segments[0]);
        AddInformation(editor, "only this segment");
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal(new[] { "point2", "only this segment" }, Selected(editor).Labels);
        Assert.Contains(segments[1], Objects(editor));
        Assert.Equal(new[] { "point2" }, segments[1].Labels);
        Assert.Equal("3", editor.DrawingLabelNumber);
        editor.GeometryViewModel.SelectedGeometricObjects.Add(segments[1]);
        Assert.False(editor.AddSelectedInformationCommand.CanExecute(null));
        Assert.Empty(editor.SelectedObjectInformation);
    }

    [Fact]
    public void SaveLoadPreservesAllStringsForPointsArrowsAndSegments()
    {
        var labels = new[] { "id1", "duplicate", "duplicate", "quotes \" and slash \\ and newline\n", "\u00e6\u03b1" };
        object[] geometry = {
            new LabeledPoint2D(1, 2, labels),
            new LabeledOrientedPoint2D(3, 4, 45, labels),
            new LabeledLineSegment2D(new Point2D(5, 6), new Point2D(7, 8), labels),
            new Point2D(9, 10)
        };
        var json = GeometryFile.Serialize(geometry);
        Assert.Contains("\"Labels\"", json);
        Assert.DoesNotContain("\"Text\"", json);
        var loaded = GeometryFile.Deserialize(json);
        Assert.IsType<LabeledPoint2D>(loaded[0]);
        Assert.IsType<LabeledOrientedPoint2D>(loaded[1]);
        Assert.IsType<LabeledLineSegment2D>(loaded[2]);
        Assert.IsType<Point2D>(loaded[3]);
        Assert.All(loaded.Take(3), item => Assert.Equal(labels, ((ILabeledGeometry)item).Labels));
        Assert.Equal(45, ((OrientedPoint2D)loaded[1]).AngleDegrees);
        var segment = (LineSegment2D)loaded[2];
        Assert.Equal(5, segment.Point1.X);
        Assert.Equal(8, segment.Point2.Y);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[null]")]
    [InlineData("[\"\",\"info\"]")]
    [InlineData("[\" \",\"info\"]")]
    [InlineData("[\"id\",\"\"]")]
    [InlineData("[\"id\",\"   \"]")]
    public void InvalidStoredLabelListsAreRejected(string labels)
    {
        Assert.Throws<System.IO.InvalidDataException>(() => GeometryFile.Deserialize(
            "[{\"Point\":{\"X\":1,\"Y\":2},\"Labels\":" + labels + "}]"));
    }

    [Fact]
    public void UnsupportedSingleStringFileIsRejectedRatherThanLosingItsLabel()
    {
        Assert.Throws<JsonSerializationException>(() => GeometryFile.Deserialize(
            "[{\"Point\":{\"X\":1,\"Y\":2},\"Text\":\"old label\"}]"));
    }
}
