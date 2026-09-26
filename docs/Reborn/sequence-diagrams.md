# Reborn interaction sequences

These are source-derived sequences, not runtime traces.

## Drawing

[drawing-sequence.puml](drawing-sequence.puml) covers both modes.

1. Each Draw-mode left click is immediately converted to world coordinates and optionally snapped.
2. Mouse movement updates a candidate world point. OnRender transforms stored and candidate points back to viewport coordinates for the preview.
3. Double-click publishes DrawingStrokePoints if at least two vertices exist.
4. The host inserts independent LineSegment2D objects and refreshes its static layer.
5. The canvas clears its transient stroke state.

Mouse release does not commit a polyline. In Dot mode, a left click publishes a one-point list and the host creates Point2D. Neither path samples a freehand stroke.

Sources: [canvas](../../Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs), [bindings](../../Craft.UIElements/Geometry2D/Reborn/GeometryView.xaml), [host](../../Craft.UIElements.Reborn.GuiTest/MainWindowViewModel.cs).

## Navigation

[pan-zoom-sequence.puml](pan-zoom-sequence.puml) shows right drag and wheel input proposing a world window. UpdateViewState applies axis locks, then WorldWindowLimiter, then publishes the window and recalculated ViewState.

WorldWindow changes can update WorldWindowExpanded. In the editor host, that triggers a spatial query and layer refresh. This is conditional, not a reload for every pointer movement.

Sources: [canvas](../../Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs), [limiter](../../Craft.ViewModels/Geometry2D/Reborn/WorldWindowLimiter.cs), [host](../../Craft.UIElements.Reborn.GuiTest/MainWindowViewModel.cs).

## Frames and rendering

[render-pipeline.puml](render-pipeline.puml) separates frame notifications from OnRender.

OnRendering checks load/time state, computes elapsed time, updates a pending damped camera move, and raises FrameRendering. GeometryView forwards it to its IFrameAware handler. The editor host can request another focus from OnFrame.

OnRender paints background, overlays, geometry, and drawing/selection feedback. Debug mode has a separate branch. Frame callbacks do not unconditionally call OnRender directly.

Sources: [canvas](../../Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs), [view code-behind](../../Craft.UIElements/Geometry2D/Reborn/GeometryView.xaml.cs), [IFrameAware](../../Craft.ViewModels/Geometry2D/Reborn/IFrameAware.cs).
