# Reborn Geometry Viewer - Supplementary Specification (Nonfunctional Requirements)

This document captures nonfunctional requirements, constraints, and quality attributes for the Reborn geometry viewer component.

1. Performance and Responsiveness
- Interactive operations (panning, zooming, drawing) must remain responsive at 60 fps for typical desktop hardware. Heavy scenes may drop below this; the viewer should degrade gracefully.
- Live drawing feedback must render in screen-space with minimal latency to follow mouse movement.
- Zoom and pan operations should avoid expensive allocations on the UI thread.

2. Memory and Resource Usage
- The viewer should avoid retaining large intermediate copies of geometry unnecessarily.
- Long-lived data sources (e.g., quadtree-based) must expose mechanisms to limit memory usage (clipping, LRU caching).

3. Scalability
- The viewer must display arbitrarily large coordinate ranges using floating-point world coordinates and view transforms. Practical rendering limits will be hardware-dependent.

4. Accuracy
- Coordinate transforms between world and viewport must be invertible within the limits of double-precision arithmetic used by .NET.

5. Threading and Concurrency
- All UI updates and drawing occur on the UI thread. Background data loading (data sources) must marshal final updates to the UI thread.

6. Compatibility
- Target platform: .NET 8 / WPF on Windows desktop.
- The viewer must interoperate with ViewModel classes and dependency properties used elsewhere in the application.

7. Testability
- Core coordinate transform logic and world-window computations should be unit-tested.

8. Accessibility
- Visual elements (grid lines, axes, labels) should use high-contrast brushes where appropriate and follow system font settings when feasible.

9. Maintainability
- Diagrams and design docs must be kept in repository under docs/Reborn; PlantUML source kept editable.

10. Logging and Diagnostics
- The viewer should expose optional diagnostic tracing for transforms and rendering metrics (frame time), toggled via DebugMode.

11. Security
- The viewer displays local data only; no network privileges are required. If external data sources are used they must sanitize inputs if any parsing occurs.

12. Operational Constraints
- The viewer relies on WPF composition and CompositionTarget.Rendering for frame timing; environments that block the UI thread will affect rendering.

Appendix: Metrics to collect
- Average frame time for render loop (ms)
- Number of visible geometries and primitives
- Memory used by geometry caches
