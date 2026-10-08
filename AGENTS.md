# Geometry editor: guidance for Codex

This repository contains a geometry viewer that is being extended with drawing and editing. Work from the repository's current code and documentation. The design notes below come from earlier discussions with the project owner; verify them before treating them as implemented behavior.

## Design intent to preserve

- Dot mode creates a point on release for a click, or a permanent arrow anchored at the press position for a drag, with a live preview. The owner confirmed that dragged arrows remain as geometry.

- Reborn drawing is click-by-click polyline drawing (double-click or Enter to finish), plus individual point creation in Dot mode. The owner confirmed that this replaced the earlier freehand workflow. Keep documentation aligned with this decision.
- Keep world coordinates distinct from viewport coordinates. Use explicit transforms and their inverse for rendering, pointer positions, panning, and zooming.
- Keep WPF presentation and input concerns separate from view-model and geometry/domain logic. Follow the repository's existing MVVM conventions.
- Preserve the intended rendering behavior: clipping to the viewport, grid and axes, stable pan and zoom, and strokes that remain about one screen pixel wide where specified. The project may support non-uniform scaling and an option to preserve aspect ratio.
- Treat time-axis strategies and rendering updates as separate concerns from drawing-tool state. Locate the current implementation before changing either.
- Look for existing project terminology and types, including `GeometryCanvas`, `GeometryViewModel`, `WorldWindow`, and `CursorWorldPosition`; names and structure may have changed.

## Before changing code

1. Identify the relevant solution and projects, trace the actual input-to-model path, and read nearby tests and documentation.
2. State any uncertainty where the code, tests, and these notes disagree. Ask the project owner about product behavior that cannot be resolved from the repository.
3. Make focused changes consistent with the existing architecture. Run the relevant build and tests available in the repository, and report what was and was not verified.

## Use case modeling

When asked to create or update a use case model:

1. Identify external actors and their goals from the UI, documentation, and tests. Name use cases for user-visible outcomes, such as drawing an object or navigating the view, rather than classes, commands, or internal rendering steps.
2. Produce an **as-built** inventory first. For each use case, cite the relevant UI entry point and supporting files, and mark uncertain or unverified behavior.
3. Keep **proposed** behavior separate from implemented behavior. Ask the project owner to settle intended workflows, especially cancellation, editing, persistence, and undo/redo if the code does not establish them.
4. For each agreed use case, describe the trigger, preconditions, main flow, alternate/error flows, and observable result. Include a compact actor/use-case diagram only when it makes relationships clearer.
5. Maintain traceability from agreed use cases to code and tests. Do not infer a user requirement solely from an implementation detail.

This file records durable guidance, not a complete specification. Update it when the project owner corrects a recurring assumption or establishes a new design decision.

- Arrows have a fixed length of 100 WPF viewport units. Dragging chooses a polar angle snapped to 5-degree increments; show the candidate angle while dragging. The tail may snap to the grid, but the arrow tip must not be grid-snapped.

- Backwards compatibility with earlier geometry file formats is not currently required. Persist oriented points as a position and angle, without legacy arrow flags or conversion paths.

- Drawing labels concatenate the Label and optional Number fields. Number accepts only non-negative integers or empty text and increments once after successful point, arrow, or completed polyline creation; all segments of a polyline share one label.

- Existing labels are edited as complete text through the Selected object panel in Select mode for one point, arrow, or individual segment. Apply commits, a blank identifier requires removing all information before Apply can make the object unlabeled, and Cancel or a selection change discards drafts. Editing does not advance or alter the drawing label fields.

- Labeled geometry stores an ordered list of strings. The first is the identifier generated from Label + Number and is the only string drawn on the canvas; subsequent strings are arbitrary information edited after selection with Add/Remove rows. Apply/Cancel affect the entire list. Cancel requires a dirty draft; Apply additionally requires that every information string is nonblank and an identifier exists while information rows remain. Add information requires a nonblank draft identifier. Preserve nonblank multiline and duplicate information entries and their order; reject empty or whitespace-only strings in stored label lists. Identifiers need not be unique. Persist the Labels array without migrating older single-Text files.
