# Build, tests, CI, publishing

## Requirements

- .NET SDK **10.0.x** (`TargetFramework=net10.0`, `LangVersion=latest`, C# 14 `extension` blocks are used).
- Node.js (18 in CI) + npm: only for building the Angular UI of the configurator (`Anch.SecuritySystem.Configurator`). The result `configurator-ui/dist/**` is embedded into the assembly via `EmbeddedResource`. Without `dist` the C# project still builds, but the admin UI will be missing (build the package for publishing only after building the UI, as in `publish.yml`).
- `global.json` enables **Microsoft.Testing.Platform** as the test runner → tests are run with `dotnet test --project <folder>` (not `dotnet test <sln>`).

## Build

All commands are run from the repository root.

```bash
# UI (only needed if the Configurator admin UI matters; CI always builds it)
cd src/SecuritySystem/Anch.SecuritySystem.Configurator/configurator-ui && npm ci && npm run ng build && cd -

dotnet restore src/Anch.Framework.slnx
dotnet build src/Anch.Framework.slnx -c Release
```

A single module only (without UI), for example: `dotnet build src/Core/Anch.Core/Anch.Core.csproj`.

NuGet package versions are centralized in `src/Directory.Packages.props` (`ManagePackageVersionsCentrally`; the `$(DotNet)` version is used for all Microsoft packages). Versions are **not specified** on `PackageReference` in a `csproj`. Add new packages via `PackageVersion` in `Directory.Packages.props`.

## Test projects

Run: `dotnet test --project <project path> -c Release` (this is what CI does).

| Module | Project | What it covers |
|---|---|---|
| Core | `src/Core/Anch.Core.Tests` | Maybe, expressions, Dictionary, async-enumerable, **`CsprojValidationTests`** (checks the `PackageId == project name` convention for all csproj files) |
| DI | `src/DependencyInjection/Anch.DependencyInjection.Tests` | Proxy factory, validators |
| IdentitySource | `src/IdentitySource/Anch.IdentitySource.Tests` | Id lookup |
| VisualIdentitySource | `src/VisualIdentitySource/Anch.VisualIdentitySource.Tests` | Display names |
| GenericQueryable | `Anch.GenericQueryable.UnitTests`, `…IntegrationTests.NHibernate`, `…IntegrationTests.EntityFramework` | (`…IntegrationTests` = shared test library, not run on its own) |
| HierarchicalExpand | `Anch.HierarchicalExpand.Tests`, `…IntegrationTests.NHibernate`, `…IntegrationTests.EntityFramework` | same |
| SecuritySystem | `Anch.SecuritySystem.DiTests`, `_Example/ExampleApp.IntegrationTests.NHibernate`, `_Example/ExampleApp.IntegrationTests.EntityFramework` | `ExampleApp.IntegrationTests` is a shared library |
| OData | `src/OData/Anch.OData.Tests` | Parsing/conversion |
| Testing | `src/Testing/Anch.Testing.Tests` | Test infrastructure |
| Caching, Parsing, RelativePath, Threading, GenericRepository, Configurator | *(no dedicated tests)* | Parsing/Threading/RelativePath are partly covered through other modules |

Example: `dotnet test --project src/HierarchicalExpand/Anch.HierarchicalExpand.Tests -c Release`.

Integration tests use SQLite files (created automatically, see [testing.md](modules/testing.md)); no external database is needed.

### How the tests are organized

The environment (`TestEnvironment`) and the DI container pool, `[AnchFact]`, `[assembly: AnchTestFramework<…>]`: see [modules/testing.md](modules/testing.md). Tests usually receive services through the constructor.

## CI (GitHub Actions)

`.github/workflows/build.yml` (on push/PR to `main`): Node 18 → `npm ci && npm run ng build` → .NET 10 → `dotnet restore` → `dotnet build -c Release` → one `dotnet test --project … --no-build -c Release` step for each test project in the table above. **A new test project must be added to `build.yml` manually.**

`.github/workflows/publish.yml` (on GitHub Release publication): UI build → `dotnet pack src/Anch.Framework.slnx /p:PackageVersion=<release tag> /p:InformationalVersion=<sha>` → `dotnet nuget push` to nuget.org (secret `NUGET_Anch`). Package version = **release tag**; the `AssemblyVersion` value in `src/__RootSolutionItems/RootCommonAssemblyInfo.cs` (currently `3.9.3.0`) is set manually and is not tied to the tag.

### NuGet package metadata

The common metadata (`Authors`, `Company`, `Copyright`, MIT license, `PackageProjectUrl`, `PackageReadmeFile`) is set in `src/Directory.Build.props`. `Description` and `PackageTags` are computed from `PackageId` in `src/Directory.Build.targets` (module description + layer suffix); the root `README.md` is packed into every package. A `Description`/`PackageTags` set in a csproj takes priority. For a new module, add a description line to `Directory.Build.targets`, otherwise the default description is used.

### XML documentation

`GenerateDocumentationFile` is enabled for all projects in `src/Directory.Build.props`, so the XML comments of the public API reach IntelliSense and the NuGet packages. Missing comments (`CS1591`) are suppressed for now because the API is not documented in full yet; malformed comments are still reported as warnings. Write comments in English; document new public types and members together with the code.

## Adding a new project (checklist)

1. Directory `src/<Module>/Anch.<Module>[.<Layer>]/`, a csproj with `<PackageId>` equal to the project name (checked by the `CsprojValidationTests` test) and, if needed, `<RootNamespace>`.
2. Add the project to `src/Anch.Framework.slnx` (into the right `<Folder>`).
3. References to other projects via `ProjectReference`, packages via `PackageReference` without a version (+ an entry in `Directory.Packages.props`).
4. For a test project: `<IsPackable>false</IsPackable>`, `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>`, a reference to `Anch.Testing.Xunit`, a step in `build.yml`.
5. Shared settings (nullable, LangVersion, NoWarn, `CS4014/CS1998/CA2016` as errors) come from `src/Directory.Build.props` automatically. `__RootSolutionItems/RootCommonAssemblyInfo.cs` is linked from there too.

## Knowledge graph (graphify)

Only `graphify-out/graph.json` is stored in git; the report, HTML and cache are created locally and ignored (`.gitignore`).

- Update the graph: `graphify update .` (code only, no LLM), then `git add graphify-out/graph.json`.
- Update the graph in a **separate commit** after major code changes. Whoever regenerated the graph commits it.
- The graph is not merged. A conflict in `graph.json` is resolved not by editing but by rebuilding: `graphify update .` and `git add graphify-out/graph.json`.
- Merging is configured as usual (the graphify merge driver is not used); the file diff is hidden (`-diff`, `linguist-generated`).

## Common problems

| Symptom | Cause / solution |
|---|---|
| The `/admin/configurator` page returns 404 / no static files | The Angular UI was not built before building the C# project: run `npm ci && npm run ng build` in `configurator-ui`, then rebuild the project |
| `dotnet test` on `.slnx` runs nothing | Use `--project <csproj or folder>` (Microsoft.Testing.Platform) |
| A test fails with "has been registered many times" | `DuplicateServiceUsageValidator` fired, see [dependency-injection.md](modules/dependency-injection.md) |
| Warnings as errors `CS4014/CS1998/CA2016` | This is a repository setting: a forgotten `await`, `async` without `await`, a `CancellationToken` that was not passed |
| Directories `src/**/Anch.X` and next to them `src/**/X` (no prefix) | The second is leftover garbage after a rename (`bin/obj`), do not touch |
