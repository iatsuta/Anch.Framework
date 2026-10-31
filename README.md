# Anch.Framework

A set of .NET libraries (`net10.0`) for building the application layer of enterprise applications: role-based access control with security contexts, ORM-independent async LINQ, hierarchy expansion, an OData parser, DI utilities and test infrastructure. Published to NuGet under the `Anch.*` prefix. MIT license.

> **The `src/Workflow/` subsystem is out of scope for now: do not read or analyze it until the owner lifts the restriction.** See [doc/modules/workflow.md](doc/modules/workflow.md).

## Module map

| Module (folder in `src/`) | Purpose | Main packages | Document |
|---|---|---|---|
| `Core` | Base utilities, expression tools, `Maybe`, data access interfaces, parser combinators, async primitives | `Anch.Core`, `Anch.GenericRepository`, `Anch.Parsing`, `Anch.RelativePath`, `Anch.Threading` | [core.md](doc/modules/core.md) |
| `DependencyInjection` | The `AddXxx(setup)` pattern, proxy factory, registration validator | `Anch.DependencyInjection` | [dependency-injection.md](doc/modules/dependency-injection.md) |
| `Caching` | Cache abstraction | `Anch.Caching` | [caching.md](doc/modules/caching.md) |
| `IdentitySource`, `VisualIdentitySource` | Id and display name of a domain object | `Anch.IdentitySource`, `Anch.VisualIdentitySource` | [identity-source.md](doc/modules/identity-source.md) |
| `GenericQueryable` | `GenericToListAsync…` and `WithFetch` independent of the ORM | `Anch.GenericQueryable`, `…EntityFramework`, `…NHibernate` | [generic-queryable.md](doc/modules/generic-queryable.md) |
| `HierarchicalExpand` | Expansion of ancestor/descendant trees in SQL queries | `Anch.HierarchicalExpand` | [hierarchical-expand.md](doc/modules/hierarchical-expand.md) |
| `OData` | `$filter/$orderby/$top/$skip` string → `Expression` | `Anch.OData` | [odata.md](doc/modules/odata.md) |
| `SecuritySystem` | Roles, permissions, security contexts, RunAs, notifications, admin UI (Angular) | `Anch.SecuritySystem`, `…GeneralPermission`, `…VirtualPermission`, `…Notification`, `…Configurator`, `…Testing` | [security-system.md](doc/modules/security-system.md) |
| `Testing` | DI container pool, database snapshots, xunit v3 integration | `Anch.Testing`, `Anch.Testing.Xunit`, `Anch.Testing.Database(.Sqlite/.Configuration)` | [testing.md](doc/modules/testing.md) |
| `Workflow` | **Do not study for now** | — | [workflow.md](doc/modules/workflow.md) |

## Repository structure

```
.
├── README.md                  ← you are here
├── doc/                       ← documentation (start with doc/README.md)
├── global.json                # test runner: Microsoft.Testing.Platform
├── .github/workflows/         # build.yml (CI), publish.yml (NuGet)
├── graphify-out/              # knowledge graph: only graph.json is in git, the rest is local (not sources)
└── src/
    ├── Anch.Framework.slnx    # the only solution
    ├── Directory.Build.props  # shared properties: net10.0, Nullable, LangVersion=latest
    ├── Directory.Packages.props   # central NuGet versions
    ├── __RootSolutionItems/   # shared files linked into projects
    └── <Module>/Anch.<Module>[.Abstractions|.Domain|.Runtime|…]
```

Typical module layout (details in [doc/architecture.md](doc/architecture.md)):

| Project suffix | Contents |
|---|---|
| `.Domain` | Plain models/enums |
| `.Abstractions` | Public interfaces |
| `.Runtime` | Implementations |
| *(no suffix)* | **Entry point**: `AddXxx(Action<IXxxSetup>)` for `IServiceCollection` |
| `.EntityFramework`, `.NHibernate`, `.Sqlite`, `.Xunit` | Technology adapters |
| `*.Tests`, `*.IntegrationTests*`, `*.DiTests` | Tests |

## Quick start

Requires .NET SDK 10.0.x.

```bash
dotnet restore src/Anch.Framework.slnx
dotnet build src/Anch.Framework.slnx -c Release
dotnet test --project src/Core/Anch.Core.Tests -c Release
```

The full list of test projects, notes on building the configurator's Angular UI, and CI: [doc/building-and-testing.md](doc/building-and-testing.md).

Registration example (the idiom of all modules):

```csharp
services
    .AddIdentitySource()
    .AddHierarchicalExpand(b => b.AddHierarchicalInfo(v => v.Parent, directedLink, undirectedLink))
    .AddSecuritySystem(b => b
        .SetQueryableSource<MyQueryableSource>()
        .AddSecurityContext<BusinessUnit>(buGuid)
        .AddSecurityRole(MyRoles.Manager, new SecurityRoleInfo(roleGuid))
        .AddDomainSecurity<Order>(DomainSecurityRule.AnyRole /* or specific roles */, SecurityPath<Order>.Create(o => o.BusinessUnit)));
```

A complete working example: `src/SecuritySystem/_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`.

## Notes for an AI agent

1. Read [doc/architecture.md](doc/architecture.md), then [doc/where-to-find.md](doc/where-to-find.md): it maps a task to a file.
2. Look for an interface in `*.Abstractions`, its implementation in `*.Runtime`, and its registration in the project without a suffix (`DependencyInjection/*Setup.cs`).
3. Look for a usage example of any API in the tests of the corresponding module.
4. The code uses C# 14 `extension(...)` blocks; property paths are always passed as `Expression<Func<…>>`.
5. Do not edit `graphify-out/`, `bin/`, `obj/`; do not touch `src/Workflow/**`.
