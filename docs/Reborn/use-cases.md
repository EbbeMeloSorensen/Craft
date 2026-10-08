# Reborn: as-built use cases

Source-inspected on 2026-09-26; GUI execution not verified. Polyline and point drawing are owner-confirmed. Other entries describe current code, not newly agreed requirements.

## Actors and evidence

The **user** draws and inspects geometry in the editor harness. The **host application** supplies geometry and requests view changes through the reusable viewer's bindings. Data-source classes are implementation collaborators, not separately established user actors.

Evidence keys:

- **UI**: [MainWindow.xaml](../../Craft.UIElements.Reborn.GuiTest/MainWindow.xaml).
- **Canvas**: [GeometryCanvas.cs](../../Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs).
- **Bindings**: [GeometryView.xaml](../../Craft.UIElements/Geometry2D/Reborn/GeometryView.xaml).
- **State**: [GeometryViewModel.cs](../../Craft.ViewModels/Geometry2D/Reborn/GeometryViewModel.cs).
- **Host**: [MainWindowViewModel.cs](../../Craft.UIElements.Reborn.GuiTest/MainWindowViewModel.cs).
- **Keys**: [MainWindow.xaml.cs](../../Craft.UIElements.Reborn.GuiTest/MainWindow.xaml.cs).
- **Store**: [MxCifQuadTreeGeometryDataStore.cs](../../Craft.ViewModels/Geometry2D/Reborn/GeometryDataSources/MxCifQuadTreeGeometryDataStore.cs).

Canvas interactions assume a loaded view with usable viewport dimensions and initialized view state.

## Inventory

| ID | Outcome | UI entry point | Supporting code |
| --- | --- | --- | --- |
| UC1 | Draw a polyline | Draw; left clicks; double-click or Enter | Canvas.OnMouseDown/OnMouseMove/OnKeyDown; Bindings.DrawingStrokePoints; Host.GeometryViewModel_PropertyChanged; Store |
| UC2 | Draw a point or arrow | Dot; left click/release or drag | Canvas.OnMouseDown; Host.GeometryViewModel_PropertyChanged; Store |
| UC3 | Select geometry | Select; click or left drag | Canvas.OnMouseUp; Host clicked-position/selection-window handlers |
| UC4 | Delete selection | Delete key | Keys.MainWindow_KeyDown; Host.HandleKeyEvent; Store |
| UC5 | Pan and zoom | Right drag; wheel; axis controls | Canvas mouse handlers and UpdateViewState |
| UC6 | Configure snapping and overlays | Snap, spacing, grid, coordinate-system controls | UI; Bindings; Canvas.SnapPointToGrid/OnRender |
| UC7 | Set region or focus | World-window, bounds, focus Apply buttons | Host commands; State requests; Canvas callbacks |
| UC8 | Inspect a time interval | Time axis mode (x); time-interval Apply | Host.SetTimeInterval; State.TimeAxisMode; Canvas.OnRender |
| UC9 | Save/load geometry | Save and Load menus | Host.SaveGeometry/LoadGeometry |
| UC10 | Display host geometry | Host bindings, not an import dialog | State layer methods; Bindings; Canvas.DrawGeometries |

## UC1: Draw a polyline

**Trigger/preconditions:** User selects Draw and clicks the canvas.

**Main flow:** Left clicks add vertices, immediately transformed to world coordinates and optionally snapped. Mouse movement previews the next segment. Double-click or Enter finishes and publishes DrawingStrokePoints through the binding. Enter commits only clicked vertices, excluding the mouse-preview endpoint, while the canvas has keyboard focus. The host creates a LineSegment2D for each adjacent pair, inserts them into the store, and refreshes its static layer.

**Alternates/limits:** Consecutive coincident vertices are suppressed using a small world-coordinate tolerance. Completion with fewer than two vertices adds no polyline. Mouse release does not finish drawing. Right drag supports panning in Draw mode. Cancellation and mode switching during unfinished drawing are open product questions.

**Result:** Independent line segments are stored and displayed. The drawn polyline is not stored as a single editable aggregate or PolyLineModel.

## UC2: Draw a point or arrow

**Trigger/preconditions:** User selects Dot and presses the left mouse button.

**Main flow:** The canvas captures the mouse and previews the position. Releasing without exceeding the system drag threshold publishes a fresh one-point list. Dragging previews an arrow from the press position to the current position; release publishes DrawnOrientedPoint and the host stores an OrientedPoint2D. The tail is stored in world coordinates and optionally grid-snapped. Drag direction snaps to 5-degree polar steps (0 degrees right, 90 degrees up); a live angle label accompanies the preview. Arrows always span 100 WPF viewport units, independent of zoom and axis scaling. The static layer refreshes after creation.

**Alternates/limits:** Right drag pans when no left-button gesture is active. Losing mouse capture or switching modes cancels the pending gesture. Save persists points, segments, and arrows.

**Result:** An individual point or directed arrow is stored and displayed. Arrows can be selected by their shaft or by a rectangle enclosing both endpoints, and deleted like segments.

## UC3: Select geometry

**Trigger/preconditions:** Select mode; geometry exists.

**Main flow:** A click publishes ClickedWorldPosition. The host queries nearby objects and selects the nearest supported point or segment within its tolerance. Left drag publishes a world-coordinate SelectionWindow; the host selects supported objects with enclosed bounding boxes.

**Alternates/limits:** For clicks, Left Shift adds and Left Ctrl toggles; without these modifiers the previous selection is cleared. Rectangle selection preserves existing selection with Left Shift. Clicking empty space without modifiers clears selection. Right-side modifier keys are not checked by these host handlers. Unsupported object types reach an exception path in the host.

**Result:** Selected points/segments render blue. Segments are selected individually, not as whole drawn polylines.

## UC4: Delete selection

**Trigger/preconditions:** Delete reaches the window handler; objects are selected.

**Main flow:** The host removes selected objects from the store, clears selection, and refreshes the layer.

**Alternates/limits:** Empty selection does nothing. The window ignores repeated key events. No undo operation is implemented in this host.

**Result:** Removed objects disappear.

## UC5: Pan and zoom

**Trigger/preconditions:** Right drag or wheel input in any canvas mode.

**Main flow:** Dragging changes world origin using viewport displacement divided by scale. Wheel input proposes scale and origin changes that keep the world point under the cursor fixed. UpdateViewState applies axis locks and bounds.

**Alternates/limits:** With aspect locking off, Ctrl-wheel changes X scale and Alt-wheel changes Y scale; Ctrl takes precedence if both are held. Otherwise both scales change. Bounds and axis locks can constrain cursor anchoring. Mouse input interrupts a pending damped focus shift.

**Result:** The visible world region changes; stored geometry does not.

## UC6: Configure snapping and overlays

**Trigger/preconditions:** User changes snap, spacing, grid, or coordinate-system controls.

**Main flow:** Bindings update the canvas settings. Snapping rounds drawing positions to multiples of GridSpacing, independently of zoom. In drawing modes the X/Y readout shows PlacementWorldPosition; raw CursorWorldPosition remains available unchanged. In Select mode the readout remains unsnapped. Rendering uses the overlay flags.

**Alternates/limits:** Showing the grid and enabling snapping are independent. The editable spacing list offers 0.01, 0.1, 1, 5, 10 and 50 world units, as well as custom positive finite values. Invalid text is marked with a validation error and leaves the last valid spacing active. The control accepts a decimal point.

**Result:** Subsequent drawing positions and visual overlays follow the settings.

## UC7: Set region or focus

**Trigger/preconditions:** User enters values and presses the relevant Apply button.

**Main flow:** The host parses numbers and publishes bounds, a requested world window, or a world focus request. The canvas applies constraints and optional damping. Focus requests specify a world point, desired viewport ratio, and scale.

**Alternates/limits:** Failed numeric parsing results in no action and no dedicated error feedback. Bounds, aspect handling, and axis locks constrain requests. Continually Move Focus is a host demonstration sending focus requests per frame.

**Result:** The view moves directly or gradually toward the constrained requested region.

## UC8: Inspect a time interval

**Trigger/preconditions:** Time axis mode enabled; valid start/end dates supplied.

**Main flow:** The host converts dates to ticks relative to 2000-01-01 UTC and requests an X interval while retaining the current Y interval. The canvas generates time ticks and labels.

**Alternates/limits:** Time mode disables aspect locking; enabling aspect locking disables time mode. The date command dereferences nullable dates without a dedicated error flow. This changes axis presentation, not drawing-tool state or data import.

**Result:** The horizontal coordinate range is presented as time.

## UC9: Save/load geometry

**Trigger/preconditions:** User chooses Save or Load.

**Main flow:** Save opens a file dialog and serializes stored points, segments, and arrows to JSON through GeometryFile. A Point record stores an ordinary point; Point plus AngleDegrees describes oriented points; Point1/Point2 describes line segments. Load opens a dialog, deserializes points, segments, and arrows, adds them to the existing store, and refreshes the layer.

**Alternates/limits:** Cancelling the dialog leaves data unchanged. Load appends rather than replaces. These methods have no dedicated recovery flow for malformed JSON or file I/O failures.

**Result:** Points, segments, and arrows are written to a file or added to the scene. This is not full scene persistence.

## UC10: Display host geometry

**Trigger/preconditions:** A host provides GeometryLayers through bindings.

**Main flow:** The canvas draws recognized objects. The editor harness queries the store using WorldWindowExpanded and refreshes its static layer when that region changes.

**Alternates/limits:** Rendering supports more types than the editor's store and selection handlers. This contract does not establish a generic import UI.

**Result:** Supplied geometry appears in the current view.

## Open product decisions

These are not implemented requirements:

- Cancellation of an unfinished polyline and behavior when changing modes mid-drawing.
- Editing whole polylines, independent segments, or vertices.
- Undo/redo history and grouping for drawing, deletion, and loading.
- Persistence of polyline identity, other geometry, and view state; append versus replace on load.

## Verification traceability

Reborn label editing has view-model regression tests in `Craft.UIElements.Reborn.UnitTest`; pointer interactions and transforms remain unverified by dedicated tests. GuiTest is a manual harness. [MxCifQuadTreeTest.cs](../../Craft.DataStructures.UnitTest/MxCifQuadTreeTest.cs) covers supporting spatial-index insertion, queries, and removal, not these workflows end to end.

Manual checks still to run: repeated polyline and point creation; click/rectangle selection with modifiers; deletion; constrained pan/zoom; snapping; time intervals; save/load of mixed geometry. None was executed for this documentation update.

Persistence verification (2026-09-27): a standalone check of GeometryFile passed mixed point/segment/arrow file round trips, point-only and empty scenes, and rejection of invalid entries. The GUI project builds; file dialogs were not manually exercised.

## Labeled points (2026-09-27)

In Dot mode, enter text in the toolbar's Label field, then click and release to place a LabeledPoint2D. Its Identifier (the first entry in Labels) is displayed beside the marker at a fixed screen size. Blank or whitespace-only input creates an ordinary point. The Label field retains its text for repeated placement; changing it affects future objects only. An adjacent Number field accepts an empty string or a non-negative integer (digits only, including zero). The final label concatenates Label and Number without a separator; an empty Label permits numeric-only labels. Number starts empty. After successfully creating a point, oriented point, or completed polyline, a populated Number increments by one. Every segment of a completed polyline receives the same label, and Number increments once for the entire polyline. Cancellation and navigation do not increment it. Typing and pasting invalid numbers are rejected. Leading zeros are used as entered for the current label; incrementing displays the next integer without leading zeros. Dragging creates a labeled oriented point when text is present. The same Label field applies in Draw mode to each completed polyline segment.

Labeled points use the ordinary point marker for click/rectangle selection and deletion; the text itself is not a separate hit target. Save/load stores position/endpoints, optional orientation, and the ordered Labels array, and restores the subtype and every string exactly. Labels[0] is a nonblank identifier; subsequent entries carry nonblank information. Invalid lists are rejected when constructed or loaded. Identifiers are not required to be unique. Earlier single-Text files are unsupported.

### Edit an object label

To edit a placed label, use Select mode and select exactly one point, oriented point, or line segment. The Selected object panel shows its identifier (empty for unlabeled objects), plus one editable row per additional information string. Edit the identifier, add rows with Add information, edit their text, or remove individual rows. Information rows accept arbitrary nonblank text, including line breaks, and preserve their order. Empty or whitespace-only rows disable Apply; Cancel remains available for dirty drafts. Add information is enabled only when the draft identifier is nonblank. Apply requires a dirty, valid draft; Cancel requires a dirty draft. Reverting edits disables both buttons. Choose Apply to commit the whole list, or Cancel to restore the stored strings. An identifier cannot be cleared while information rows remain: Apply is disabled until the identifier is restored or those rows are removed. Clearing the identifier and removing all information rows makes the object unlabeled. Only the identifier is drawn on the canvas. New objects start with the identifier generated from Label and Number; information is entered after selecting an object. Changing selection or drawing mode discards unapplied edits; zero or multiple selections disable editing. Apply preserves coordinates, orientation, and selection, and refreshes the canvas. A polyline segment is edited individually; other segments retain their labels. Editing does not change the drawing Label or Number fields. Save/load retains the edited label. Delete while a text input has focus edits text instead of deleting selected geometry.

Label editing is implemented by the Selected object panel in `Craft.UIElements.Reborn.GuiTest/MainWindow.xaml` and the selection/draft/Apply commands in `MainWindowViewModel.cs`. Regression coverage is in `Craft.UIElements.Reborn.UnitTest/LabelEditingTests.cs` and `LabelListTests.cs`; `Craft.Math.UnitTest/GeometryLabelStringsTests.cs` covers immutable list storage. On Windows, build the WPF test project before running tests:

```powershell
dotnet build Craft.UIElements.Reborn.UnitTest/Craft.UIElements.Reborn.UnitTest.csproj -m:1
dotnet test Craft.UIElements.Reborn.UnitTest/Craft.UIElements.Reborn.UnitTest.csproj --no-build
```
