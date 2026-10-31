# Cheat sheet: "what to look for where"

A quick pointer for navigating the code. All paths are relative to `src/`. Project layers (`Abstractions`/`Runtime`/…) are explained in [architecture.md](architecture.md).

> `src/Workflow/**` is not to be studied for now, see [modules/workflow.md](modules/workflow.md).

## By task

| I want to… | Look at |
|---|---|
| Understand how any module is plugged into DI | `DependencyInjection/Anch.DependencyInjection/ServiceCollectionExtensions.cs`, then `<Module>/Anch.<Module>/DependencyInjection/*Setup.cs` |
| Find an interface (contract) | `*/Anch.*.Abstractions/` (for Core: `Core/Anch.Core/*.cs`) |
| Find the implementation of interface `IFoo` | `*/Anch.*.Runtime/` (search `class .* : IFoo`) |
| Understand how to restrict data access by roles | [modules/security-system.md](modules/security-system.md), `SecuritySystem/_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` |
| Add a new role/rule/security context | `ISecuritySystemSetup` (`SecuritySystem/Anch.SecuritySystem/DependencyInjection/`), rules: `SecuritySystem/Anch.SecuritySystem.Rules/` |
| See how the SQL access filter is built | `SecuritySystem/Anch.SecuritySystem.Runtime/Builders/`, `Providers/RoleBaseSecurityPathProvider.cs` |
| Permissions stored in the application database | `SecuritySystem/Anch.SecuritySystem.GeneralPermission*` |
| Permissions computed from domain data | `SecuritySystem/Anch.SecuritySystem.VirtualPermission*` |
| Permission admin UI (REST + Angular) | `SecuritySystem/Anch.SecuritySystem.Configurator/` (`Handlers/`, `configurator-ui/`) |
| Expand a "parent/children" hierarchy in a query | `HierarchicalExpand/Anch.HierarchicalExpand.Runtime/HierarchicalObjectAncestorLinkExpander.cs` |
| ORM-independent async LINQ (`GenericToListAsync`) | `GenericQueryable/Anch.GenericQueryable.Abstractions/GenericQueryableExtensions.cs` |
| Eager loading (`WithFetch`) | `GenericQueryable/Anch.GenericQueryable.Abstractions/Fetching/` |
| Plug GenericQueryable into EF / NHibernate | `GenericQueryable/Anch.GenericQueryable.EntityFramework/DbContextOptionsBuilderExtensions.cs` / `…NHibernate/ServiceCollectionExtensions.cs`, `ConfigurationExtensions.cs` |
| Parse an OData string (`$filter=…`) | `OData/Anch.OData.Runtime/` (grammar: `InternalParser/`) |
| Get the Id/name of an arbitrary domain type | `IdentitySource/`, `VisualIdentitySource/` |
| Work with expression trees (comparison, compilation, visitors) | `Core/Anch.Core/ExpressionComparers/`, `ExpressionEvaluate/`, `Visitor/`, `ExpressionExtensions.cs`, `ExpressionHelper.cs` |
| `PropertyAccessors`, `LambdaExpressionPath`, `Maybe<T>`, `MergeResult` | `Core/Anch.Core/` (files with the same names) |
| Async locks, `AsyncOnce` | `Core/Anch.Threading/` |
| Cache | `Caching/` |
| DI registration validation, `DuplicateServiceUsageValidator` | `DependencyInjection/Anch.DependencyInjection/` |
| Write an integration test with a database | [modules/testing.md](modules/testing.md); samples: `GenericQueryable/Anch.GenericQueryable.IntegrationTests.EntityFramework/Environment/` |
| Test SecuritySystem without a database | `SecuritySystem/Anch.SecuritySystem.DiTests/` |
| How to run/add tests, CI, publishing | [building-and-testing.md](building-and-testing.md) |
| Central package versions / shared build properties | `Directory.Packages.props`, `Directory.Build.props` |

## By type name (frequent)

| Type | File/project |
|---|---|
| `ISecuritySystem` | `SecuritySystem/Anch.SecuritySystem.Abstractions/SecuritySystem/` |
| `ISecurityProvider<T>` | `SecuritySystem/Anch.SecuritySystem.Abstractions/Providers/` |
| `IDomainSecurityService<T>` | `SecuritySystem/Anch.SecuritySystem.Abstractions/DomainServices/` |
| `DomainSecurityRule`, `SecurityRule`, `SecurityRole`, `SecurityPathRestriction` | `SecuritySystem/Anch.SecuritySystem.Rules/` |
| `SecurityPath<T>` | `SecuritySystem/Anch.SecuritySystem.Runtime/SecurityPath/SecurityPath.cs` |
| `SecuritySystemSetup` | `SecuritySystem/Anch.SecuritySystem/DependencyInjection/SecuritySystemSetup.cs` |
| `IPermissionSystem` / `IPermissionSource` | `SecuritySystem/Anch.SecuritySystem.Runtime/ExternalSystem/` |
| `IPrincipalManagementService` | `SecuritySystem/Anch.SecuritySystem.Abstractions/ExternalSystem/Management/` |
| `IHierarchicalObjectExpander<T>` | `HierarchicalExpand/Anch.HierarchicalExpand.Abstractions/` |
| `HierarchicalInfo<T>`, `AncestorLinkInfo<,>` | `HierarchicalExpand/Anch.HierarchicalExpand.Abstractions/` |
| `FetchRule<T>`, `IFetchService` | `GenericQueryable/Anch.GenericQueryable.Abstractions/Fetching/` |
| `IGenericQueryableExecutor` | `GenericQueryable/Anch.GenericQueryable.Abstractions/Services/` |
| `SelectOperation<T>` | `OData/Anch.OData.Domain/SelectOperation.cs` |
| `ISelectOperationParser` | `OData/Anch.OData.Abstractions/` |
| `IIdentityInfoSource` | `IdentitySource/Anch.IdentitySource.Abstractions/` |
| `IQueryableSource`, `IGenericRepository` | `Core/Anch.GenericRepository/` |
| `IRelativeDomainPathInfo<,>` | `Core/Anch.RelativePath/` |
| `IServiceProxyFactory` | interface in `Core/Anch.Core/`, implementation in `DependencyInjection/Anch.DependencyInjection/ServiceProxy/` |
| `ITestEnvironment` | `Testing/Anch.Testing.Abstractions/` |
| `DatabaseTestEnvironment` | `Testing/Anch.Testing.Database/` |
| `ICurrentUser` | `Core/Anch.Core/Auth/` |
| `IInitializer` | `Core/Anch.Core/IInitializer.cs` |

## Search tips

- **Find an extension method** by the method name: the repository uses C# 14 `extension(T x) { … }` blocks, not `this T x`.
- **Where a service is registered**: `Grep` for `AddScoped<IFoo` / `typeof(IFoo<>)` in `*Setup.cs`.
- **Usage example of an API**: tests (`*.Tests`, `ExampleApp.IntegrationTests`) almost always contain a minimal working example.
- **Exclude from searches**: `bin/`, `obj/`, `graphify-out/`, `configurator-ui/node_modules`, `src/Workflow/**`.
- `graphify-out/graph.json`: the code knowledge graph, stored in git; updated with `graphify update .`. The `GRAPH_REPORT.md` report is created locally by the same command and is not in git.
