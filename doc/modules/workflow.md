# Workflow: DO NOT STUDY FOR NOW

> **For AI agents and developers: the `src/Workflow/` subsystem is currently out of scope. Do not read it, analyze it, use it as an example, or include it in overviews/reviews until the repository owner lifts this restriction.** Documentation for it is intentionally not written.

What you can know without looking inside:

- The `src/Workflow/` folder (projects `Anch.Workflow.*`) is fully isolated; the other modules (Core, GenericQueryable, SecuritySystem, …) do **not** depend on it.
- It is part of the `Anch.Framework.slnx` solution, is built and tested in CI (the "Workflow Unit tests" step), and is published as NuGet packages together with the rest.
- If a task needs something specifically in Workflow, ask the repository owner first.

When searching the whole repository, exclude `src/Workflow/**` to avoid irrelevant noise.

Additionally: `graphify-out/graph.json` contains Workflow nodes; ignore them.
