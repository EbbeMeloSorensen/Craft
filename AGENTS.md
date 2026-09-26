# Geometry editor: guidance for Codex

This repository contains a geometry viewer that is being extended with drawing and editing. Work from the repository's current code and documentation. The design notes below come from earlier discussions with the project owner; verify them before treating them as implemented behavior.

## Design intent to preserve

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
