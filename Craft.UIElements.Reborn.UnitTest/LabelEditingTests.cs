using Craft.DataStructures.Geometry;
using Craft.Math;
using Craft.Math.IO;
using Craft.UIElements.Reborn.GuiTest;
using Craft.ViewModels.Geometry2D.Reborn;
using System.Windows;
using Xunit;

namespace Craft.UIElements.Reborn.UnitTest;

public class LabelEditingTests
{
    private static MainWindowViewModel CreateEditor()
    {
        var editor = new MainWindowViewModel { DrawingLabel = "item", DrawingLabelNumber = "1" };
        editor.GeometryViewModel.WorldWindowExpanded = new BoundingBox(-100, 100, -100, 100);
        return editor;
    }

    private static object[] Objects(MainWindowViewModel editor) => editor.GeometryViewModel.GeometryLayers
        .SelectMany(layer => layer.GeometricObjects.Cast<object>()).ToArray();

    private static void Select(MainWindowViewModel editor, object geometry)
    {
        editor.GeometryViewModel.SelectedGeometricObjects.Clear();
        editor.GeometryViewModel.SelectedGeometricObjects.Add(geometry);
    }

    [Fact]
    public void RenamingPointPreservesPositionSelectionAndDrawingFieldsAndPersists()
    {
        var editor = CreateEditor();
        editor.GeometryViewModel.DrawingStrokePoints = new List<Point> { new(12, 34) };
        var original = Assert.IsType<LabeledPoint2D>(Assert.Single(Objects(editor)));
        Select(editor, original);
        Assert.Equal("item1", editor.SelectedObjectLabel);
        editor.SelectedObjectLabel = "renamed point";
        Assert.Equal("item1", original.Identifier);
        editor.ApplySelectedLabelCommand.Execute(null);

        var renamed = Assert.IsType<LabeledPoint2D>(Assert.Single(Objects(editor)));
        Assert.Equal("renamed point", renamed.Identifier);
        Assert.Equal(12, renamed.X);
        Assert.Equal(34, renamed.Y);
        Assert.Same(renamed, Assert.Single(editor.GeometryViewModel.SelectedGeometricObjects));
        Assert.Equal("item", editor.DrawingLabel);
        Assert.Equal("2", editor.DrawingLabelNumber);
        var loaded = Assert.IsType<LabeledPoint2D>(Assert.Single(GeometryFile.Deserialize(GeometryFile.Serialize(Objects(editor)))));
        Assert.Equal(renamed.Identifier, loaded.Identifier);
        Assert.Equal(renamed.X, loaded.X);
        Assert.Equal(renamed.Y, loaded.Y);

        editor.HandleKeyEvent(System.Windows.Input.Key.Delete);
        Assert.Empty(Objects(editor));
        Assert.False(editor.CanEditSelectedLabel);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RemovingAndAddingArrowLabelPreservesOrientation(string emptyLabel)
    {
        var editor = CreateEditor();
        editor.GeometryViewModel.DrawnOrientedPoint = new OrientedPoint2D(10, 20, 45);
        Select(editor, Assert.Single(Objects(editor)));
        editor.SelectedObjectLabel = emptyLabel;
        editor.ApplySelectedLabelCommand.Execute(null);
        var arrow = Assert.IsType<OrientedPoint2D>(Assert.Single(Objects(editor)));
        Assert.Equal(10, arrow.X);
        Assert.Equal(20, arrow.Y);
        Assert.Equal(45, arrow.AngleDegrees);
        Assert.Equal("", editor.SelectedObjectLabel);
        Assert.Same(arrow, Assert.Single(editor.GeometryViewModel.SelectedGeometricObjects));
        Assert.IsType<OrientedPoint2D>(Assert.Single(GeometryFile.Deserialize(GeometryFile.Serialize(Objects(editor)))));

        editor.SelectedObjectLabel = "new arrow";
        editor.ApplySelectedLabelCommand.Execute(null);
        var labeled = Assert.IsType<LabeledOrientedPoint2D>(Assert.Single(Objects(editor)));
        Assert.Equal("new arrow", labeled.Identifier);
        Assert.Equal(45, labeled.AngleDegrees);
        var loaded = Assert.IsType<LabeledOrientedPoint2D>(Assert.Single(GeometryFile.Deserialize(GeometryFile.Serialize(Objects(editor)))));
        Assert.Equal(labeled.Identifier, loaded.Identifier);
        Assert.Equal(labeled.AngleDegrees, loaded.AngleDegrees);
    }

    [Fact]
    public void RenamingOnePolylineSegmentLeavesOtherSegmentsUnchanged()
    {
        var editor = CreateEditor();
        editor.GeometryViewModel.DrawingStrokePoints = new List<Point> { new(0, 0), new(10, 10), new(20, 0) };
        var segments = Objects(editor).Cast<LabeledLineSegment2D>().ToArray();
        Select(editor, segments[0]);
        editor.SelectedObjectLabel = "changed";
        editor.ApplySelectedLabelCommand.Execute(null);
        var edited = Objects(editor).OfType<LabeledLineSegment2D>().Single(s => s.Identifier == "changed");
        Assert.Same(segments[0].Point1, edited.Point1);
        Assert.Same(segments[0].Point2, edited.Point2);
        Assert.Contains(segments[1], Objects(editor));
        Assert.Equal("item1", segments[1].Identifier);
        Assert.Equal("2", editor.DrawingLabelNumber);
        Assert.Same(edited, Assert.Single(editor.GeometryViewModel.SelectedGeometricObjects));
        var loaded = GeometryFile.Deserialize(GeometryFile.Serialize(Objects(editor))).Cast<LabeledLineSegment2D>();
        Assert.Contains(loaded, s => s.Identifier == "changed" && s.Point1.X == edited.Point1.X && s.Point2.Y == edited.Point2.Y);

        editor.SelectedObjectLabel = "";
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.IsType<LineSegment2D>(Assert.Single(editor.GeometryViewModel.SelectedGeometricObjects));
        editor.SelectedObjectLabel = "restored";
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal("restored", Assert.IsType<LabeledLineSegment2D>(Assert.Single(editor.GeometryViewModel.SelectedGeometricObjects)).Identifier);
    }

    [Fact]
    public void CancelAndSelectionChangesDiscardDraftsAndMultipleSelectionDisablesEditing()
    {
        var editor = CreateEditor();
        editor.GeometryViewModel.DrawingStrokePoints = new List<Point> { new(0, 0) };
        editor.GeometryViewModel.DrawingStrokePoints = new List<Point> { new(10, 10) };
        var points = Objects(editor).Cast<LabeledPoint2D>().ToArray();
        Assert.False(editor.CanEditSelectedLabel);
        Assert.False(editor.ApplySelectedLabelCommand.CanExecute(null));
        Select(editor, points[0]);
        editor.SelectedObjectLabel = "draft";
        editor.CancelSelectedLabelCommand.Execute(null);
        Assert.Equal(points[0].Identifier, editor.SelectedObjectLabel);
        editor.SelectedObjectLabel = "other draft";
        Select(editor, points[1]);
        Assert.Equal(points[1].Identifier, editor.SelectedObjectLabel);
        Assert.Equal("item1", points.Single(p => p.Identifier == "item1").Identifier);
        editor.GeometryViewModel.SelectedGeometricObjects.Add(points[0]);
        Assert.False(editor.CanEditSelectedLabel);
        Assert.Equal("", editor.SelectedObjectLabel);
        Assert.False(editor.CancelSelectedLabelCommand.CanExecute(null));
        editor.SelectedObjectLabel = "must not apply";
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal(2, Objects(editor).Length);
        Assert.All(Objects(editor), p => Assert.StartsWith("item", ((LabeledPoint2D)p).Identifier));
        Select(editor, points[0]);
        editor.SelectedObjectLabel = "draft before mode change";
        editor.GeometryViewModel.CanvasMode = CanvasMode.Dot;
        Assert.False(editor.CanEditSelectedLabel);
        Assert.Equal("", editor.SelectedObjectLabel);
    }

    [Fact]
    public void UnlabeledPointCanReceiveAndRemoveLabel()
    {
        var editor = CreateEditor();
        editor.DrawingLabel = "";
        editor.DrawingLabelNumber = "";
        editor.GeometryViewModel.DrawingStrokePoints = new List<Point> { new(7, 9) };
        Select(editor, Assert.IsType<Point2D>(Assert.Single(Objects(editor))));
        Assert.Equal("", editor.SelectedObjectLabel);
        editor.SelectedObjectLabel = "named";
        editor.ApplySelectedLabelCommand.Execute(null);
        Assert.Equal("named", Assert.IsType<LabeledPoint2D>(Assert.Single(Objects(editor))).Identifier);
        editor.SelectedObjectLabel = "";
        editor.ApplySelectedLabelCommand.Execute(null);
        var point = Assert.IsType<Point2D>(Assert.Single(Objects(editor)));
        Assert.Equal(7, point.X);
        Assert.Equal(9, point.Y);
    }
}
