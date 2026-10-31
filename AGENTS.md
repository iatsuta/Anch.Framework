# AGENTS.md

Guidance for AI coding agents working in this repository.

Anch.Framework is a .NET 10 monorepo of libraries: Core, DependencyInjection, Caching, IdentitySource, VisualIdentitySource, GenericQueryable, HierarchicalExpand, OData, SecuritySystem and Testing. The solution is `src/Anch.Framework.slnx`.

## Where to start

Do not scan the whole tree. Read in this order:

1. [doc/README.md](doc/README.md): index of the documentation.
2. [doc/architecture.md](doc/architecture.md): layers and module dependencies.
3. [doc/where-to-find.md](doc/where-to-find.md): "I need X, where is it" lookup.
4. `doc/modules/<module>.md`: the contracts of one module.

`graphify-out/graph.json` is a prebuilt knowledge graph of the code; use it for relationship questions if the `graphify` tool is available.

## Out of scope

- The **Workflow** subsystem: do not study or change it unless asked.
- Directories without the `Anch.` prefix next to an `Anch.X` project (for example `src/**/X`) are leftovers of a rename; do not touch them.

## Build and test

Requirements: .NET SDK 10.0.x (see `global.json`). Run commands from the repository root.

```bash
dotnet build src/Anch.Framework.slnx -c Release
dotnet test --project src/Core/Anch.Core.Tests -c Release
```

- Tests run on Microsoft.Testing.Platform: use `dotnet test --project <folder>`, **not** `dotnet test <sln>` (it runs nothing). The list of test projects is in [doc/building-and-testing.md](doc/building-and-testing.md).
- Integration tests use SQLite files; no external database is needed.
- The Angular UI of the configurator is built separately (`npm ci && npm run ng build` in `configurator-ui`); it is not needed for ordinary C# changes.
- Warnings `CS4014`, `CS1998` and `CA2016` are errors: await every task, no `async` without `await`, pass `CancellationToken`.

## Conventions

- **Layers:** `.Domain` (contracts and models), `.Abstractions` (interfaces), `.Runtime` (implementations), no suffix (DI entry point `AddXxx(Action<IXxxSetup>)`), and adapters (`.EntityFramework`, `.NHibernate`, ...). Respect the dependency direction in `doc/architecture.md`.
- **Packages:** versions are central in `src/Directory.Packages.props`; never put a `Version` on `PackageReference`.
- **New project:** follow the checklist in `doc/building-and-testing.md` (`PackageId` equal to the project name, add to the `.slnx`, add a test step to `.github/workflows/build.yml`).
- **Language:** C# 14 with nullable enabled; the code uses `extension(...)` blocks.
- **Tests:** use `[AnchFact]` and constructor-injected services; see `doc/modules/testing.md`.

## Documentation

- Everything in the repository is **English**: code, comments, docs, commit messages. Do not add Cyrillic anywhere, including file names.
- Document every new public type and member with XML comments (`<summary>`, `<param>`, `<typeparam>`, `<returns>`, `<exception>` where relevant). `GenerateDocumentationFile` is on, so they reach IntelliSense and NuGet. Use `<inheritdoc/>` for implementations and `<see cref="..."/>` for links.
- Describe the contract (what, when, what is guaranteed), not the implementation.
- When behavior or structure changes, update the matching file in `doc/` in the same change.

## Files and formatting

- Files are UTF-8 with BOM. `.gitattributes` enforces CRLF for `*.cs` and `*.csproj`; keep line endings when editing.
- Follow `.editorconfig`.
- `graphify-out/graph.json` is generated: never edit or merge it by hand. On a conflict run `graphify update .` and add the file again.

## Git

- Do not commit, push or create tags unless explicitly asked.
- Do not change the package version or `RootCommonAssemblyInfo.cs` as a side effect of other work.
