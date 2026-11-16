# GenericQueryable

Folder `src/GenericQueryable/`. It has its own README: [`src/GenericQueryable/readme.md`](../../src/GenericQueryable/readme.md) (the link to `GenericQueryableExtensions.cs` in it is outdated; the file is in `Anch.GenericQueryable.Abstractions/`).

## Why

Async LINQ (`ToListAsync`, `FirstOrDefaultAsync`, `Include`…) in EF/NHibernate consists of static methods tied to the ORM. The application layer should not reference them. GenericQueryable provides ORM-independent methods `GenericToListAsync()`, `GenericSingleAsync()` … and `WithFetch(...)` (eager loading), and delegates execution to the provider "baked into" the `IQueryable`.

## Projects

| Project | Contents |
|---|---|
| `Anch.GenericQueryable.Abstractions` | Public API and contracts (see below) |
| `Anch.GenericQueryable.Runtime` | Fetch extension implementations: `RootFetchService`, `CompositeFetchRuleExpander`, `FetchRuleHeaderExpander`, `UntypedFetchExpander`, `RootFetchRuleExpander` |
| `Anch.GenericQueryable` | DI: `AddGenericQueryable(setup)`, `IGenericQueryableSetup`, `IGenericQueryableExtension` |
| `Anch.GenericQueryable.EntityFramework` | EF Core adapter: `UseGenericQueryable()` for `DbContextOptionsBuilder`, `EfFetchService`, `VisitedEfQueryProvider`, `EfTargetMethodExtractor` |
| `Anch.GenericQueryable.NHibernate` | NHibernate adapter: `AddNHibernateGenericQueryable()`, `Configuration.SetGenericQueryProvider()`, `NHibFetchService`, `VisitedNHibQueryProvider` |
| `Anch.GenericQueryable.UnitTests` | `MainTests`, `FetchRuleExpanderTests` |
| `Anch.GenericQueryable.IntegrationTests` (+ `.EntityFramework`, `.NHibernate`) | Shared tests + two environments (SQLite) |

## How it works

1. **Extensions** (`GenericQueryableExtensions.cs`): `source.GenericToListAsync(ct)`, `GenericSingleAsync`, `GenericAnyAsync`, `GenericContainsAsync`, `GenericAllAsync`, `GenericCountAsync`, `GenericFirst(OrDefault)Async`, `GenericSingle(OrDefault)Async`, `GenericToDictionaryAsync`, `GenericToHashSetAsync`, `GenericSumAsync`, `GenericAsAsyncEnumerable`, etc. Each calls `source.Execute(() => <the same method>)`.
2. `Execute` takes an `IGenericQueryableExecutor` from `IGenericQueryProvider` (i.e. `IQueryable.Provider` must implement `IGenericQueryProvider`) and passes it the call expression.
3. `GenericQueryableExecutor` goes through the `IMethodRedirector`s (`QueryableMethodRedirector`, `AsyncEnumerableMethodRedirector`, `MethodRedirector`) and replaces the `GenericXxxAsync` call with the real ORM method (`ITargetMethodExtractor` knows the mappings: `EfTargetMethodExtractor`, `NHibTargetMethodExtractor`). The result is compiled and cached (`LambdaCompileCache`).
4. For **plain** `IQueryable` (LINQ to Objects, in tests) `GenericQueryableExecutor.Sync` is used: the methods are reduced to synchronous/`AsyncEnumerable` analogues, and `WithFetch` is ignored (`IgnoreFetchService`).
5. ORM adapters replace the `IQueryProvider` of queries with their own "visited" provider (`VisitedEfQueryProvider`, `VisitedNHibQueryProvider`), through which an `ExpressionVisitor` passes (extension point: `SetVisitor<T>()`, on which, for example, `LinkIdVisitor` in the integration tests is built).

## Fetch rules (`Abstractions/Fetching/`)

`WithFetch` describes which relations to load:

```csharp
query.WithFetch(r => r.Fetch(x => x.Orders).ThenFetch(o => o.Items));  // typed
query.WithFetch("Orders.Items");                                      // by string (UntypedFetchRule)
query.WithFetch(AppFetchRule.TestFetchRule);                          // named rule (FetchRuleHeader)
```

- `FetchRule<TSource>`: the root of the hierarchy (record). Subtypes: `PropertyFetchRule<TSource[,TProp]>` (`LambdaExpressionPath` chains), `UntypedFetchRule`, `FetchRuleHeader<TSource,TValue>` (a named "header"), `CompositeFetchRule`.
- `IFetchRuleExpander.TryExpand(rule)`: turns a "header"/string/composite into a `PropertyFetchRule`. Custom expanders are added with `AddFetchRuleExpander<T>()`; a ready "header → path" mapping: `AddFetchRule(header, implementation)`.
- `IFetchService.ApplyFetch(query, rule)`: applies the ORM-specific `Include`/`Fetch` (`EfFetchService`, `NHibFetchService`).

## Setup

EF Core:
```csharp
services.AddDbContext<MyDb>(o => o.UseGenericQueryable());
// with fetch rule configuration etc. through your own setup type (IEfGenericQueryableExtensionSetup.SetSetupType<T>()):
services.AddDbContext<MyDb>(o => o.UseGenericQueryable(s => s.SetSetupType<EfGenericQueryableExtensionSetup>()));
// sample: Anch.GenericQueryable.IntegrationTests.EntityFramework/Environment/TestDbContext.cs
```
NHibernate:
```csharp
cfg.SetGenericQueryProvider();                                           // Configuration (NHibConfigurationSource)
services.AddNHibernateGenericQueryable(b => b.AddFetchRuleExpander<AppFetchRuleExpander>());
```

## What to change where

- A new async method (`GenericXxxAsync`): `Abstractions/GenericQueryableExtensions.cs` + make sure the `ITargetMethodExtractor`s (`Services/*TargetMethodExtractor.cs`, `EfTargetMethodExtractor`, `NHibTargetMethodExtractor`) can find the mapping.
- A new fetch rule: `Abstractions/Fetching/*` (model) + `Runtime/Fetching/*` (expansion).
- Support for a new ORM: follow `Anch.GenericQueryable.NHibernate` (query provider, fetch service, target method extractor, DI extension).
