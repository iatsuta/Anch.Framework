# HierarchicalExpand

Folder `src/HierarchicalExpand/`. Expands hierarchies ("parent → children" trees) directly in SQL queries using precomputed tables/views of "ancestor → descendant" links.

Example: a user has a permission on the `BusinessUnit` "Russia". You need the Id of "Russia" plus all its children. The module builds such a query over the link table without recursion in the database.

## Projects

| Project | Contents |
|---|---|
| `Anch.HierarchicalExpand.Domain` | `HierarchicalExpandType` ([Flags]: `None`, `Parents`, `Children`, `All`) |
| `Anch.HierarchicalExpand.Abstractions` | Hierarchy description models + contracts of expanders and denormalizers |
| `Anch.HierarchicalExpand.Runtime` | Implementations (expanders, link denormalization) |
| `Anch.HierarchicalExpand` | DI: `AddHierarchicalExpand(setup)`, `IHierarchicalExpandSetup` |
| `Anch.HierarchicalExpand.Tests` | Unit/DI tests on in-memory data (`DomainObjectExpanderTests`, `AncestorLinkExtractorTests`, `DiTests`) |
| `…IntegrationTests` (+ `.EntityFramework`, `.NHibernate`) | Tests on a real database (SQLite) |

## Hierarchy description model

For every hierarchical domain type the following are registered (`IHierarchicalExpandSetup.AddHierarchicalInfo`):

| Type | What it describes |
|---|---|
| `HierarchicalInfo<T>(ParentPath)` | The expression `x => x.Parent` |
| `AncestorLinkInfo<TDomain, TLink>(From, To)` | The link table/entity: which link property is "from" and which is "to" |
| `FullAncestorLinkInfo<T, TDirected, TUndirected>` | A pair of links: **Directed** (ancestor → descendant) and **Undirected** (both ways, usually a view) |
| `DeepLevelInfo<T>(DeepLevel)` | Optional: a "node depth" property (`x => x.DeepLevel`) |

Registration (sample: `Anch.HierarchicalExpand.Tests/Environment/TestEnvironment.cs`):

```csharp
services.AddHierarchicalExpand(b => b.AddHierarchicalInfo(
    v => v.Parent,
    new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(l => l.Ancestor, l => l.Child),
    new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(v => v.Source, v => v.Target),
    v => v.DeepLevel));   // deepLevelPath is optional
```

`AddHierarchicalExpand` automatically calls `AddIdentitySource()` and `AddServiceProxyFactory()`. It requires `IQueryableSource` (data reading; see [core.md](core.md)) and `IGenericRepository` to be in the container.

## Expansion by Id

`IHierarchicalObjectExpander<TIdent>` (obtained via `IHierarchicalObjectExpanderFactory.Create(domainType)`):

| Method | Result |
|---|---|
| `Expand(idents, expandType)` | Ids taking the expansion mode into account |
| `GetExpandExpression(expandType)` | `Expression<Func<IEnumerable<TIdent>, IEnumerable<TIdent>>>` to embed into queries |
| `TryGetSingleExpandExpression(expandType)` | the same for a single Id, `null` if not supported |
| `ExpandWithParents(idents, expandType)` | A dictionary `Id → Id of the parent/source` |

Semantics of `HierarchicalExpandType` (see `HierarchicalObjectAncestorLinkExpander`):
- `None`: unchanged;
- `Children`: descendants via **Directed** links;
- `Parents`: ancestors via **Directed** links in the reverse direction (`Directed.Reverse()`);
- `All`: via **Undirected** (both ancestors and descendants).

If the type is not hierarchical, `HierarchicalObjectExpanderTypeResolver` returns `PlainHierarchicalObjectExpander`, which returns its input unchanged. So calling code does not need to check whether a type is hierarchical.

`IDomainObjectExpander<TDomain>` (factory `IDomainObjectExpanderFactory<T>.Create()`) is the same at the object level: `GetAllParents(objects, ct)`, `GetAllChildren(objects, ct)`.

`IHierarchicalInfoSource`: `GetHierarchicalInfo<T>()`, `IsHierarchical(Type)`.

## Denormalization (keeping link tables up to date)

Directed link tables and the `DeepLevel` field are stored in the database and must be synchronized when the tree changes:

- `IAncestorDenormalizer<T>.SyncAsync(updated, removed, ct)`: recomputes links for changed/removed nodes (`Runtime/Denormalization/AncestorDenormalizer.cs`, `AncestorLinkExtractor`, `SyncResult`).
- `IDeepLevelDenormalizer<T>.UpdateDeepLevels(objects, ct)`.
- Both inherit `IInitializer`: `Initialize(ct)` initializes (recomputes) the data for all registered types; used in test environments.
- The undirected representation (Undirected) in the examples is a **SQL view** (`UndirectedAncestorViewScriptGenerator` in `_Example`), not a table maintained by code.

## What to change where

- A new expansion mode / query optimization: `Runtime/HierarchicalObjectAncestorLinkExpander.cs`.
- How the implementation for a type is chosen: `Runtime/HierarchicalObjectExpanderTypeResolver.cs`, `HierarchicalObjectExpanderFactory.cs` (via `IServiceProxyFactory`, see [dependency-injection.md](dependency-injection.md)).
- Registrations: `Anch.HierarchicalExpand/DependencyInjection/HierarchicalExpandSetup.cs`.

## Consumers

`Anch.SecuritySystem` (`ISecurityContextInfoSetup.SetHierarchicalInfo`, expansion of security contexts by rules with `HierarchicalExpandType`). See [security-system.md](security-system.md).
