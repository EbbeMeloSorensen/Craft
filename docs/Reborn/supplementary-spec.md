# Reborn constraints and verification

This replaces the earlier generated supplementary specification. These are source observations and development guidance, not measured performance guarantees.

## Coordinates and rendering

[GeometryCanvas](../../Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs) keeps world and viewport coordinates distinct. For each axis:

- viewport = (world - worldOrigin) * scaling
- world = worldOrigin + viewport / scaling

Y currently increases downward, matching the viewport. These transforms do not invert Y. Invertibility assumes valid nonzero scales and viewport dimensions; exhaustive numerical behavior was not tested here.

[GeometryView](../../Craft.UIElements/Geometry2D/Reborn/GeometryView.xaml) sets ClipToBounds. The normal renderer explicitly transforms positions, keeping stroke widths independent of zoom. Current segment/polyline pens are 2 WPF device-independent units wide; point markers have radius 3. These are not physical-pixel guarantees. Grid/axis pens and debug rendering have their own handling.

Independent X/Y scales are supported. Axis locks and bounds constrain navigation. Aspect locking and time mode are mutually exclusive in [GeometryViewModel](../../Craft.ViewModels/Geometry2D/Reborn/GeometryViewModel.cs).

## Platform and separation of responsibilities

[Craft.UIElements](../../Craft.UIElements/Craft.UIElements.csproj) and [Craft.ViewModels](../../Craft.ViewModels/Craft.ViewModels.csproj) target net8.0-windows7.0 with WPF. [The editor harness](../../Craft.UIElements.Reborn.GuiTest/Craft.UIElements.Reborn.GuiTest.csproj) targets net8.0-windows. These declarations are not a tested OS compatibility matrix.

Preserve the separation of presentation/input, exposed state, and geometry/storage described in [architecture](class-diagram.md), acknowledging existing WPF dependencies.

## Unestablished claims

This inspection does not establish a 60 fps guarantee, arbitrary-coordinate-range guarantee, memory-cache policy, accessibility conformance, or frame-time logging contract. DebugMode selects a visual debug branch; it is not documented here as a metrics logger.

The inspected input paths use WPF event handling. A general background-loading/thread-marshalling contract has not been established.

## Verification

This is a documentation-only update. No build, test suite, GUI session, benchmark, or PlantUML rendering was run.

[MxCifQuadTreeTest.cs](../../Craft.DataStructures.UnitTest/MxCifQuadTreeTest.cs) exercises the underlying spatial index, including insertion and removal. It does not validate canvas interaction or transforms. No dedicated Reborn unit-test project was found.

Suggested future checks include transform round trips, constrained cursor-centred zoom, non-uniform scaling, repeated drawing, modifier selection, mode changes mid-drawing, and persistence. These are not claims of existing tests.

See [use cases](use-cases.md) for unresolved product decisions.
