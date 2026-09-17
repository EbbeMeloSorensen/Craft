# Reborn Geometry Viewer - Use Case Model

This document describes primary actors and use cases for the Reborn geometry viewer component.

Actors
- User: interacts with the viewer through mouse/keyboard to inspect and create geometry.
- External Data Source: provides geometry layers and time-series data to the viewer.

Primary Use Cases
- Draw Geometry: user draws freehand polylines (mouse left button). The viewer captures stroke points and converts to world-space geometry on completion.
- Pan View: user pans the world (mouse right button). The viewer updates ViewState.WorldOrigin and re-renders.
- Zoom View: user zooms with mouse wheel. Viewer scales ViewState.Scaling and keeps mouse point fixed.
- Toggle Grid / Coordinate System: user toggles visual overlays (grid, axes, labels).
- Time Axis Mode: viewer switches into time-axis rendering (special ticks/labels). Useful for time-series display.
- Edit Geometry: user modifies or removes existing geometry (provided by UI/commands).
- Import/Bind Data Source: the viewer consumes geometry layers from data sources (e.g., function curve, quadtree).

Notes
- Live drawing uses screen-space feedback for responsiveness; final geometry is stored in world-space.
- ViewState and WorldWindow are central to coordinate transforms between world and viewport.

Diagram
The PlantUML source file for the use-case diagram is: `use-cases.puml` (in this folder). You can render it with PlantUML or a supporting VSCode extension.
