# Testing

Folder `src/Testing/`. A custom test infrastructure on top of **xunit v3** and `Microsoft.Extensions.DependencyInjection`. Almost all test projects in the repository use it, so to understand any test, read this file first.

## Idea

Instead of creating objects manually in a test, the test class receives its dependencies **through the constructor** from a DI container. Containers (service providers) are built in advance and kept in a **pool**, and tests run in parallel: each takes a free provider and returns it after the test. For database tests, each provider from the pool gets its own copy of the database (a snapshot), so parallel tests do not interfere with each other.

## Projects

| Project | Contents |
|---|---|
| `Anch.Testing.Abstractions` | `ITestEnvironment`, `ITestEnvironmentHook`, `EnvironmentHookType` (`Before`/`After`), `ServiceProviderBuildContext`, `ServiceProviderIndex` (`Main` = -1, the rest = number in the pool), `IParallelizationSettings`, `AllowParallelizationConstraint`, `IMainServiceProviderSettings`, `AddEnvironmentHook(...)` |
| `Anch.Testing` | Provider pool: `ServiceProviderPool`, `InternalServiceProviderPool`, `IServiceProviderPoolFactory`, `ParallelizationSettings`, `RunEnvironmentHooks` |
| `Anch.Testing.Xunit` | xunit v3 integration: `[AnchFact]`, `[AnchInlineData]`, `[AnchMemberData]`, `[assembly: AnchTestFramework<TEnv>]`, `Engine/*` (own runners: `AnchTestFramework`, `AnchTestCaseRunner`, …) |
| `Anch.Testing.Database` | Database tests: `DatabaseTestEnvironment`, `IDatabaseManager`, connection strings, snapshot initialization, database preparation/cleanup hooks, `AddDatabaseTesting` |
| `Anch.Testing.Database.Sqlite` | SQLite provider (`SqliteDatabaseTestingProvider`, `SqliteDatabaseManager`): database = file, snapshot = file copy |
| `Anch.Testing.Database.Configuration` | Connection strings from `IConfiguration` (`ConfigurationTestEnvironment`) |
| `Anch.Testing.Tests` | Tests of the infrastructure itself (`TestEnvironment`, `TheoryTests`) |

## How to write tests

1. Specify the environment at assembly level:
   ```csharp
   [assembly: AnchTestFramework<TestEnvironment>]      // TestEnvironment : ITestEnvironment, new()
   // or [assembly: AnchTestFramework]                // without an environment
   ```
2. The environment builds the container:
   ```csharp
   public class TestEnvironment : ITestEnvironment
   {
       public IServiceProvider BuildServiceProvider(IServiceCollection services, ServiceProviderBuildContext buildContext) =>
           services
               .AddXxx(...)
               .AddValidator<DuplicateServiceUsageValidator>().Validate()
               .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
   }
   ```
3. The test class takes services in its constructor (`IServiceProvider` or concrete interfaces), and methods are marked `[AnchFact]`/`[Theory]` (a theory with `[MemberData]` is also supported by the framework). A `CancellationToken ct` can be accepted as a test parameter.
4. Environment hooks: `services.AddEnvironmentHook(EnvironmentHookType.Before|After, sp => ...)`. `Before` hooks are invoked in reverse registration order, `After` in forward order. Example: resetting counters/storage after a test (`HierarchicalExpand.Tests/Environment/TestEnvironment.cs`).

> If a test project shows no `Fact`/`Theory` but has `[AnchFact]`, that is normal. Run via `dotnet test --project <project folder>` (Microsoft.Testing.Platform, see [building-and-testing.md](../building-and-testing.md)).

## Database tests (`Anch.Testing.Database`)

Base class `DatabaseTestEnvironment : ITestEnvironment` (inherited, for example, by the EF/NHibernate integration test environments). What to set in a derived class:
- `RawConnectionString`: the main (reference) connection string (`TestConnectionString`);
- `InitDatabase(IDatabaseTestingSetup)`: attach the database provider (`SetProvider<SqliteDatabaseTestingProvider>()`), the empty schema initializers (`SetEmptySchemaInitializer<T>`) and test data initializers (`SetTestDataInitializer<T>`), and the parallelism policy (`SetParallelization(bool)`).

`TestDatabaseSettings`: `RawConnectionString`, `InitMode`, `RemoveDatabaseOnFailure` (true by default).

`DatabaseInitMode`:
- `RebuildSnapshot`: every run recreates the reference database (default);
- `ReuseSnapshot`: use the ready one if it exists;
- `External`: the database is managed externally.

Lifecycle:
1. The **main provider** (`Index.IsMain`) is initialized once by `DatabaseSnapshotInitializer` (a keyed `IInitializer` with the key `ITestEnvironment.MainServiceProviderKey`): it creates the snapshots `EmptySnapshot` (empty schema, `IEmptySchemaInitializer`) and `FilledSnapshot` (+ test data, `ITestDataInitializer`). Connection string roles: `TestConnectionStringRole.EmptySnapshot/FilledSnapshot`, `PoolTestConnectionStringRole(index)`.
2. **Before each test** the hook `PrepareDatabaseEnvironmentHook` copies `FilledSnapshot` into the private database of the pool provider (`DatabaseSnapshotManager.RestoreDatabaseSnapshot` → `IDatabaseManager.Copy`).
3. **After the test** the hook `CleanDatabaseEnvironmentHook` deletes this copy (`RemoveRestoredDatabase`). The next test gets a fresh copy, so tests can freely modify data.
4. Parallelism: `AllowParallelizationConstraint`. If at least one constraint is `false`, tests run strictly one at a time (`ParallelizationSettings.AllowParallelization`).

The file `src/__RootSolutionItems/DatabaseInitModeHelper.cs` is a static helper for the database initialization mode for integration tests (currently returns `RebuildSnapshot` in both Debug and Release). `XunitParallelSettings.cs` is a commented-out xunit parallelism setting.

## Sample environments

| What it shows | File |
|---|---|
| Minimal environment | `Testing/Anch.Testing.Tests/TestEnvironment.cs` |
| In-memory + hooks | `HierarchicalExpand/Anch.HierarchicalExpand.Tests/Environment/TestEnvironment.cs` |
| Database + EF Core | `GenericQueryable/Anch.GenericQueryable.IntegrationTests.EntityFramework/Environment/*` |
| Database + NHibernate | `GenericQueryable/Anch.GenericQueryable.IntegrationTests.NHibernate/Environment/*` |
| Full SecuritySystem stack | `SecuritySystem/_Example/ExampleApp.IntegrationTests*/Environment/*` |

## Module-specific test helpers

`Anch.SecuritySystem.Testing` (see [security-system.md](security-system.md)): `RootAuthManager`, `RootImpersonateService`, `TestingEvaluator` for running code on behalf of a test user/with specific permissions.
