# Reborn Geometry Viewer Documentation

This folder contains documentation, diagrams and specification for the Reborn geometry viewer component.

Files
- use-cases.md, use-cases.puml — use case descriptions and PlantUML source
- supplementary-spec.md — nonfunctional requirements and constraints
- class-diagram.md, class-diagram.puml — static design and PlantUML source
- sequence-diagrams.md, *.puml — sequence diagrams for drawing, panning/zooming, and render pipeline

Rendering PlantUML
- Install PlantUML (or a PlantUML extension in your editor) and Graphviz for diagram rendering.
- To generate PNG from a .puml file using plantuml CLI:

  plantuml class-diagram.puml

or to render all .puml files in this directory:

  plantuml *.puml

Contribution guidance
- Keep PlantUML sources as the canonical editable sources. Commit rendered images only when you want to freeze a version for e.g. a README or release.

Questions or changes
- If you prefer alternative diagram formats (draw.io, Mermaid), tell me and I can produce those as well.
