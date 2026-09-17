# Reborn Geometry Viewer - Class Diagram and Static Design

This diagram captures the primary classes and relationships used by the Reborn geometry viewer.

- GeometryCanvas: the main WPF FrameworkElement implementing rendering and input handling. It holds a ViewState, a collection of GeometryLayer and a WorldWindow. It delegates limits to WorldWindowLimiter.
- ViewState: lightweight DTO containing WorldOrigin and Scaling.
- BoundingBox: represents a rectangular region in world coordinates (WorldWindow).
- GeometryLayer: container for GeometricObject instances; multiple layers allow composition and visibility control.
- GeometricObject and subclasses: PolyLineModel, VerticalLineModel, HorizontalLineModel, LineSegment2D, Point2D, Circle2D.

Files of interest:
- Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs
- Craft.ViewModels/Geometry2D/Reborn/ViewState.cs
- Craft.ViewModels/Geometry2D/Reborn/GeometricModels/*

The PlantUML source is in `class-diagram.puml` in this folder.
