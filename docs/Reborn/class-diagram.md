# Reborn architecture

Source-inspected on 2026-09-26. [class-diagram.puml](class-diagram.puml) shows the principal relationships.

## Presentation and state

[GeometryCanvas](../../Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs) is a WPF FrameworkElement owning input, transient drawing/selection state, viewport transforms, constraints, overlays, and rendering.

[GeometryView](../../Craft.UIElements/Geometry2D/Reborn/GeometryView.xaml) binds the canvas to [GeometryViewModel](../../Craft.ViewModels/Geometry2D/Reborn/GeometryViewModel.cs). WorldWindow and requests use two-way bindings. ViewState, cursor/click positions, and DrawingStrokePoints flow from canvas to view-model. [View code-behind](../../Craft.UIElements/Geometry2D/Reborn/GeometryView.xaml.cs) forwards frame notifications to an IFrameAware host.

GeometryViewModel exposes state, selected objects, layers, and layer-management methods. It does not itself create or persist drawn geometry.

## Host policy

[MainWindowViewModel](../../Craft.UIElements.Reborn.GuiTest/MainWindowViewModel.cs) observes state notifications, turns drawing points into domain points/segments, queries selection, deletes objects, handles file dialogs, and refreshes geometry.

This is the current responsibility split, not strict presentation independence: the view-model project uses WPF types, and the GUI host view-model reads keyboard state and opens Windows dialogs.

## Geometry and storage

[ViewState](../../Craft.ViewModels/Geometry2D/Reborn/ViewState.cs) stores WorldOrigin and independent X/Y Scaling. WorldWindow is a BoundingBox property, not a class named WorldWindow. [WorldWindowLimiter](../../Craft.ViewModels/Geometry2D/Reborn/WorldWindowLimiter.cs) fits and clamps proposed windows.

[GeometryLayer](../../Craft.ViewModels/Geometry2D/Reborn/GeometryLayer.cs) wraps an IEnumerable and IsFrameDependent flag; it has no visibility property or required common GeometricObject base class.

[MxCifQuadTreeGeometryDataStore](../../Craft.ViewModels/Geometry2D/Reborn/GeometryDataSources/MxCifQuadTreeGeometryDataStore.cs) stores Point2D and LineSegment2D and maps objects to spatial items for removal.

The renderer also supports Circle2D, PolyLineModel, HorizontalLineModel, and VerticalLineModel. Rendering support does not imply drawing, storage, selection, or persistence support for every type.

## Time and frames

[TimeTickEngine](../../Craft.UIElements/Geometry2D/Reborn/TimeAxis/TimeTickEngine.cs) selects fixed-duration, month, or year strategies. [TimeCoordinates](../../Craft.UIElements/Geometry2D/Reborn/TimeAxis/TimeCoordinates.cs) defines the epoch.

CompositionTarget.Rendering drives camera damping and host frame notifications. WPF OnRender draws the visual when scheduled. Drawing-tool state and time-tick generation are separate concerns.

Arrow update (2026-09-27): OrientedPoint2D is a Point2D subtype storing AngleDegrees directly, supporting shaft selection, rectangle selection, and deletion. Arrows are included independently of the spatial index window because their visible extent varies with viewport scale. The canvas publishes DrawnOrientedPoint through GeometryView to the host. Rendering draws a 100-unit shaft and fixed-size arrowhead in viewport coordinates. Selection computes the corresponding world shaft from the current X/Y scales. Oriented-point persistence stores Point and AngleDegrees. Earlier file formats are not a compatibility requirement.

LabeledPoint2D specializes Point2D with an immutable Text property. The host's PointLabel property supplies text when a click is completed. The canvas draws the marker and label; GeometryFile persists the position and text.

LabeledOrientedPoint2D and LabeledLineSegment2D specialize their corresponding geometry types with immutable Text properties. The shared DrawingLabel field supplies labels for newly created geometry. Each polyline segment receives the entered label; labels render beside oriented-point anchors or segment midpoints. Selection and deletion continue to act on the geometry.
