# Anch.Framework documentation

Index. The documentation is written so that a human or an AI agent can quickly find their way around the repository.

## Start here

1. [Root README](../README.md): what this is, module map, quick start.
2. [architecture.md](architecture.md): repository layout, project layers (`Abstractions`/`Runtime`/…), the `AddXxx(Action<IXxxSetup>)` pattern, dependency graph, code conventions.
3. [where-to-find.md](where-to-find.md): cheat sheet "what to look for where", by task and by type name.
4. [building-and-testing.md](building-and-testing.md): build, running tests, CI, publishing, checklist for a new project.

## Modules

| Document | Module | Summary |
|---|---|---|
| [core.md](modules/core.md) | Core (`Anch.Core`, `GenericRepository`, `Parsing`, `RelativePath`, `Threading`) | Base utilities, `Maybe`, expression tools, data access interfaces |
| [dependency-injection.md](modules/dependency-injection.md) | `Anch.DependencyInjection` | Setup pattern, service proxy factory, registration validation |
| [caching.md](modules/caching.md) | Caching | `ICache`/`ICacheProvider` |
| [identity-source.md](modules/identity-source.md) | IdentitySource, VisualIdentitySource | Id and display name of an arbitrary domain type |
| [generic-queryable.md](modules/generic-queryable.md) | GenericQueryable | ORM-independent async LINQ and `WithFetch` (EF Core, NHibernate) |
| [hierarchical-expand.md](modules/hierarchical-expand.md) | HierarchicalExpand | Expansion of hierarchies (parents/children) in queries |
| [odata.md](modules/odata.md) | OData | OData query parser producing an `Expression` |
| [security-system.md](modules/security-system.md) | SecuritySystem | Roles, permissions, security contexts, admin UI |
| [testing.md](modules/testing.md) | Testing | Test infrastructure: DI container pool, database snapshots, xunit v3 |
| [workflow.md](modules/workflow.md) | Workflow | **Do not study for now** |

## State of the documentation

The documents describe the code as of the time of writing; some conclusions about internals (especially `SecuritySystem.Runtime/Builders`) were made from file structure and names, without line-by-line reading; such places are marked in the text. If the documentation disagrees with the code, the code is right and the document must be fixed.
