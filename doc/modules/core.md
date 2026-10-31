# Core

Folder `src/Core/`. Base libraries without domain logic. All other modules build on them.

| Project | Package | Purpose |
|---|---|---|
| `Anch.Core` | Anch.Core | Utilities: Maybe, expression tools, LINQ/Task/Type extensions, base interfaces |
| `Anch.GenericRepository` | Anch.GenericRepository | Two minimal data access interfaces |
| `Anch.Parsing` | Anch.Parsing | Parser combinators (used by OData) |
| `Anch.RelativePath` | Anch.RelativePath | A "relative path" between domain types (`From → To`) expressed as expressions |
| `Anch.Threading` | Anch.Threading | Asynchronous synchronization primitives |
| `Anch.Core.Tests` | not a package | Core unit tests |

## Anch.Core

Root namespace `Anch.Core` (sub-namespaces: `Auth`, `ExpressionComparers`, `ExpressionEvaluate`, `DictionaryCache`, `Visitor`). Many `*Extensions.cs` extension files; look them up by the name of the type being extended (`EnumerableExtensions`, `TaskExtensions`, `TypeExtensions`, `StringExtensions`, etc.).

### Base contracts (project root)

| Type | File | What it is |
|---|---|---|
| `IFactory<TResult>`, `IFactory<TArg,TResult>` | `IFactory.cs` | Factory |
| `IInitializer` | `IInitializer.cs` | `Task Initialize(CancellationToken)`. Used by the test infrastructure and denormalizers (registered as a keyed service) |
| `IQueryableInjector<T>` | `IQueryableInjector.cs` | `IQueryable<T> Inject(IQueryable<T>)`. "Add a filter/ordering to the query". Implemented by `ISecurityProvider<T>`, `SelectOperation<T>` (OData) |
| `IServiceProxyFactory`, `IServiceProxyBinder` | same names | Contracts of the "proxy factory" (implementation in `Anch.DependencyInjection/ServiceProxy`) |
| `IDefaultCancellationTokenSource` | | Default token source for synchronous places |
| `ICultureSource` / `CultureSource` | | Source of the current culture |
| `Ignore` | `Ignore.cs` | `record struct`, analogue of `Unit`/void for generic parameters |
| `MergeResult<TSource,TTarget>` | `MergeResult.cs` | Result of merging collections: `AddingItems`, `CombineItems`, `RemovingItems` |
| `ICurrentUser` | `Auth/ICurrentUser.cs` | `string Name`; key constants `RawKey/ImpersonatedKey/DefaultKey` (keyed registrations). `FixedCurrentUser` is a fixed user |

### Maybe (`Maybe/`)

`Maybe<T>` (struct) with `HasValue`, `Value`, `Just`, `Nothing`. Helpers: `Maybe.Return`, `ToMaybe`, `OfCondition`, `OfTryMethod`; LINQ extensions (`from x in maybe select ...`) in `LinqMaybeExtensions`. Used widely inside (for example, `ExpressionExtensions.GetConstantValue<T>()` returns a `Maybe`).

### Expressions

| What | Where | Why |
|---|---|---|
| `PropertyAccessors<TSource,TProp>` | `PropertyAccessors.cs` | `record`: path expression + `Getter` + `Setter`. Equality via `ExpressionComparer`. Built from a lambda: `expr.ToPropertyAccessors()` |
| `LambdaExpressionPath` | `LambdaExpressionPath.cs` | A chain of property lambdas (`a.B.C`), comparable by value. Created from strings: `LambdaExpressionPath.Create(type, ["B","C"])`. The basis of GenericQueryable fetch rules |
| `ExpressionComparers/*` | | Structural comparison/hashing of expressions (`ExpressionComparer.Default`). A separate comparer for each node type |
| `ExpressionEvaluate/*` | | Lambda compilation with a cache: `IExpressionEvaluator`, `LambdaCompileCache`, `LambdaCompileMode` (flags `InjectMaybe`, `IgnoreStringCase`, `OptimizeBooleanLogic`, `All`) |
| `Visitor/*` | | `ExpressionVisitor`s: `ExpandConstVisitor`, `OptimizeBooleanLogicVisitor` (folds constant conditions), `OverrideExpressionVisitor` |
| `ExpressionHelper`, `ExpressionExtensions`, `LinqExpressionExtensions`, `FuncHelper` | root | Building/composing expressions (`Select`, `UpdateBody`, `GetEqualityWithExpr`, …) |

### Other

- `DictionaryCache/`: `IDictionaryCache<TKey,TValue>`, a simple thread-safe dictionary cache.
- `DeepEqualsCollection<T>`, `ArrayComparer`, `EqualityComparerImpl`: value-based collection comparison (used in `SecurityPathRestriction`).
- `PipeObjectExtensions` (`.Pipe(...)`), `LazyHelper`, `TaskResultHelper`.

## Anch.GenericRepository

Two interfaces:
```csharp
IGenericRepository { SaveAsync<T>(T, ct); RemoveAsync<T>(T, ct); }   // write
IQueryableSource  { IQueryable<T> GetQueryable<T>(); }                // read
```
The application implements them on top of its ORM, while the framework modules (HierarchicalExpand, SecuritySystem) depend only on the interfaces. In tests these are `TestQueryableSource` and `Substitute.For<IGenericRepository>()`.

## Anch.Parsing

A parser-combinator monad: `delegate ParsingState<TInput,TValue> Parser<TInput,TValue>(TInput input)`. Files: `Parser.cs`, `ParserCombinatorsMonad.cs` (LINQ syntax), `ParserCombinatorExtensions.cs`, `CharParsers.cs`, `DecimalParser.cs`, `Parsers.cs`. The only consumer in the repository is `Anch.OData.Runtime`.

## Anch.RelativePath

`IRelativeDomainPathInfo<TFrom,TTo>`: describes a relation between types using an expression (a single property or a collection):
- `CreateCondition(Expression<Func<TTo,bool>>)` turns a filter on `TTo` into a filter on `TFrom`;
- `GetRelativeObjects(TFrom)`, `Select(...)`, `OverrideInput(...)`.

Implementations: `SingleRelativeDomainPathInfo`, `ManyRelativeDomainPathInfo`, `SelfRelativeDomainPathInfo`. Registration: `services.AddRelativeDomainPath<TFrom,TTo>(x => x.Prop, key?)` (`DependencyInjection/ServiceCollectionExtensions.cs`). Used by SecuritySystem for dependent domain objects (`SetDependency`, `CurrentUser` with `RelativePathKey`).

## Anch.Threading

- `AsyncLocker` / `IAsyncLocker`: `await locker.CreateScope(ct)` → `IDisposable` (an asynchronous mutex).
- `AsyncOnce`: runs an async action once; repeated calls receive the same exception (`EnsureExecutedAsync`).
- `AsyncLazy<T>`.
