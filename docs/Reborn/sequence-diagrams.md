# Reborn Geometry Viewer - Sequence Diagrams

This file explains the dynamic interactions depicted in the accompanying PlantUML sequence diagrams.

1. drawing-sequence.puml
- Shows the lifecycle of a user drawing a stroke: MouseDown → live screen-space feedback (OnRender) → MouseUp → conversion to world-space and addition to GeometryLayer.

2. pan-zoom-sequence.puml
- Shows panning via right-mouse drag and zoom via mouse wheel. Both update the ViewState by computing a proposed world window and calling UpdateViewState.

3. render-pipeline.puml
- Shows CompositionTarget.Rendering triggering the viewer's rendering pipeline, which ultimately calls OnRender and DrawGeometries.

The PlantUML sources can be rendered to images using PlantUML.
