# Reborn geometry editor

This is an as-built reference based on source inspection on 2026-09-26, replacing an earlier generated description of freehand drawing. The owner has confirmed click-by-click polyline drawing and individual point drawing as the intended current behavior.

The descriptions are traced from code, not verified by running the GUI. They are not a complete agreed product specification.

- [Use cases](use-cases.md): UI entry points, flows, limitations, and open decisions.
- [Architecture](class-diagram.md): presentation, state, host, and storage responsibilities.
- [Interaction sequences](sequence-diagrams.md): drawing, navigation, and frames.
- [Constraints and verification](supplementary-spec.md): coordinates, rendering, and test coverage.

## Scope

[Craft.sln](../../Craft.sln) contains several generations of geometry UI. These documents concern:

- [GeometryCanvas](../../Craft.UIElements/Geometry2D/Reborn/GeometryCanvas.cs) and its reusable WPF view.
- [GeometryViewModel](../../Craft.ViewModels/Geometry2D/Reborn/GeometryViewModel.cs) and supporting Reborn types.
- [The editor GUI harness](../../Craft.UIElements.Reborn.GuiTest/MainWindow.xaml), including selection, deletion, and persistence.

[The Reborn simulation host](../../Craft.Simulation.Reborn.GuiTest/SimulationLaboratoryViewModel.cs) is a separate consumer. The editor harness's storage and persistence policies should not be assumed for every host.

## Maintenance

Follow [AGENTS.md](../../AGENTS.md). Verify descriptions against code and keep proposed behavior separate until the owner settles it. Update prose and the accompanying editable PlantUML sources together.

The PlantUML files were previously empty. They now describe the inspected relationships and interactions; they have not been rendered during this update.
