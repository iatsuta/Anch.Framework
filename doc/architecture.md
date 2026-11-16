# Architecture and conventions

This document describes how the repository is organized as a whole. Modules are covered in [modules/](modules/).

## 1. Repository layout

```
Anch.Framework/
├── global.json                     # test runner = Microsoft.Testing.Platform
├── src/
│   ├── Anch.Framework.slnx         # the only solution (slnx format)
│   ├── Directory.Build.props       # net10.0, Nullable, LangVersion=latest, shared NoWarn
│   ├── Directory.Packages.props    # central NuGet package versions (CPM)
│   ├── __RootSolutionItems/        # files linked into all projects (AssemblyInfo, xunit settings)
│   ├── Core/                       # base libraries independent of any domain
│   ├── DependencyInjection/        # extensions over Microsoft.Extensions.DependencyInjection
│   ├── Caching/
│   ├── IdentitySource/             # "how to get the Id of a domain object"
│   ├── VisualIdentitySource/       # "how to get the Name/Display of a domain object"
│   ├── GenericQueryable/           # ORM-independent async LINQ + fetch rules
│   ├── HierarchicalExpand/         # expansion of hierarchies (parents/children)
│   ├── OData/                      # OData query parser producing an Expression
│   ├── SecuritySystem/             # permission system (the largest module)
│   ├── Testing/                    # integration test infrastructure (xunit v3)
│   └── Workflow/                   # ⚠ DO NOT STUDY FOR NOW, see modules/workflow.md
├── doc/                            # this documentation
├── graphify-out/                   # knowledge graph: only graph.json is in git, the rest is generated locally
└── .github/workflows/              # build.yml (CI), publish.yml (NuGet)
```

Each module folder contains several csproj files. Project name = `Anch.<Module>[.<Layer>]`; the NuGet package name is the same (`<PackageId>`).

> The working copy may contain directories without the `Anch.` prefix (for example `src/GenericQueryable/GenericQueryable`, `src/SecuritySystem/SecuritySystem`). These are leftovers from a rename: they contain only `bin/obj` and are not in git. Sources are always in the `Anch.*` directories.

## 2. Layers inside a module (the main navigation rule)

Almost every module is split into the same layers. Knowing the layer tells you what a project contains:

| Project suffix | Contents | Dependencies |
|---|---|---|
| `.Domain` | Plain models/enums without logic (`HierarchicalExpandType`, `ISecurityContext`, `SelectOperation`) | minimal |
| `.Abstractions` | Public interfaces and contracts (`IIdentityInfoSource`, `ISecuritySystem`) | Core, Domain |
| `.Runtime` | Interface implementations (classes `*Source`, `*Service`, `*Expander`) | Abstractions |
| *(no suffix)* `Anch.<Module>` | **Consumer entry point**: `AddXxx(...)` for `IServiceCollection` and `IXxxSetup` (fluent configuration) | Runtime + `Anch.DependencyInjection` |
| `.<ORM/technology>` | Adapters (`.EntityFramework`, `.NHibernate`, `.Sqlite`, `.Xunit`) | main package + external library |
| `.Tests`, `.UnitTests`, `.IntegrationTests*`, `.DiTests` | Tests | see [building-and-testing.md](building-and-testing.md) |

Not every module has all layers: for example, `Caching` has no `.Domain`, and `Core` has no layers at all.

Rule for consumers: referencing the package without a suffix (`Anch.SecuritySystem`) is enough; it transitively pulls Runtime/Abstractions. Libraries that need only the contracts (for example, an application's domain layer) reference `.Abstractions`.

## 3. Registration pattern: `AddXxx(Action<IXxxSetup>)`

A single way to plug any module into DI:

```csharp
services.AddIdentitySource(b => b.SetId((MyEntity e) => e.Key));   // IdentitySource
services.AddHierarchicalExpand(b => b.AddHierarchicalInfo(...));   // HierarchicalExpand
services.AddSecuritySystem(b => b.AddSecurityRole(...));           // SecuritySystem
```

How it works (see `src/DependencyInjection/Anch.DependencyInjection/ServiceCollectionExtensions.cs`):

1. There is an interface `IServiceInitializer<TContainer>` with a method `Initialize(TContainer)`.
2. Each module defines a class `XxxSetup : IXxxSetup, IServiceInitializer`. The `IXxxSetup` methods only **accumulate** settings.
3. `services.Initialize<XxxSetup>(setupAction)` creates `XxxSetup`, invokes `setupAction`, then `Initialize(services)`, where the settings are turned into container registrations.
4. Nested extensions (GeneralPermission, VirtualPermission, Notification) do the same on top of `ISecuritySystemSetup`: `securitySystemSetup.Initialize<ISecuritySystemSetup, XxxSetup>(...)`.

Where to find a module's configuration: the `DependencyInjection/*Setup.cs` and `DependencyInjection/ServiceCollectionExtensions.cs` files in the project without a suffix.

Useful DI helpers from `Anch.DependencyInjection`:
- `AlreadyInitialized<T>()`: checks whether a service is registered;
- `AddScopedFrom<TService, TSource>(selector)`, `AddSingletonFrom<…>`: register a service as a projection of another one;
- `ReplaceSingleton/ReplaceScoped`: replace a registration;
- `AddValidator<T>()` + `Validate()`: manual validation of a service collection (see [dependency-injection.md](modules/dependency-injection.md)).

## 4. Module dependency graph

```
Core (Anch.Core, Threading, Parsing, RelativePath, GenericRepository)
  └── DependencyInjection
        ├── Caching ──────────────┐
        ├── IdentitySource        │
        ├── VisualIdentitySource  │
        ├── GenericQueryable      │
        ├── HierarchicalExpand ← IdentitySource, GenericQueryable.Abstractions, GenericRepository
        ├── OData ← Caching, Parsing
        ├── Testing
        └── SecuritySystem ← HierarchicalExpand, VisualIdentitySource, GenericQueryable.Abstractions,
                             IdentitySource.Abstractions, RelativePath, GenericRepository
```

There are no circular dependencies. `Anch.Core` and `Anch.GenericRepository` depend on nothing.

## 5. Code conventions

- **Target platform**: `net10.0`, `LangVersion=latest`, `Nullable` and `ImplicitUsings` enabled.
- **C# 14 `extension(...)` blocks** instead of classic `this` methods:
  ```csharp
  public static class ServiceCollectionExtensions
  {
      extension(IServiceCollection services)
      {
          public IServiceCollection AddFoo() => ...;
      }
  }
  ```
  This is normal. When searching for an extension method, grep by the method name, not by `this IServiceCollection`.
- **Expressions as data**: property paths are passed as `Expression<Func<T, TProp>>` (not strings). They are stored in `PropertyAccessors<TSource,TProp>` (path + compiled getter/setter) and compared via `ExpressionComparer` (see [core.md](modules/core.md)). That is why many `record`s in the project cache the hash code and have a hand-written `Equals`.
- **`CancellationToken ct`** is the last parameter of all async methods. `IDefaultCancellationTokenSource` is sometimes used for synchronous places.
- **`record`/`record struct`** for models and results. Record inheritance is widely used (see `DomainSecurityRule`).
- **Keyed services** are used heavily: `AddKeyedSingleton/Scoped` with a `string`/enum key (for example, `ElementKey = "Element"`, `ICurrentUser.RawKey/ImpersonatedKey/DefaultKey`, `ITestEnvironment.MainServiceProviderKey`).
- **Tests** use xunit v3 plus the custom layer `Anch.Testing.Xunit` (`[AnchFact]`, `[assembly: AnchTestFramework<TestEnvironment>]`). Details in [testing.md](modules/testing.md).
- **CRLF**: `.gitattributes` forces `eol=crlf` for `*.cs`, `*.csproj`, etc.
- Files start with a BOM (UTF-8 with BOM). Keep this in mind when editing or comparing by hand.

## 6. What is NOT source code

- `graphify-out/`: only `graph.json` is stored in git (the code knowledge graph; query: `graphify query`). `GRAPH_REPORT.md`, `graph.html` and the cache are created locally (`graphify update .`) and are not committed.
- `src/TestResults/`, `bin/`, `obj/`, `*.DotSettings.user` are local artifacts.
- `src/SecuritySystem/Anch.SecuritySystem.Configurator/configurator-ui/` is an Angular application (the configurator UI), built with `npm` and embedded into the assembly as embedded resources.
