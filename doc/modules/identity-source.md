# IdentitySource and VisualIdentitySource

Two small related modules. They answer the question "how to get the identifier / display name of an **arbitrary** domain type without knowing it in advance". Both are built on `PropertyAccessors` (expressions).

## IdentitySource (`src/IdentitySource/`)

| Project | Contents |
|---|---|
| `Anch.IdentitySource.Abstractions` | `IIdentityInfoSource`, `IIdentityInfo<TDomain,TIdent>`, `IdentityInfo` (record), `IIdentityPropertyExtractor` |
| `Anch.IdentitySource.Runtime` | `IdentityInfoSource`, `IdentityInfoProxy`, `IdentityPropertyExtractor`, `IdentityPropertySourceSettings` |
| `Anch.IdentitySource` | DI: `AddIdentitySource(setup?)`, `IIdentitySourceSetup` |
| `Anch.IdentitySource.Tests` | Tests |

Key types:
- `IdentityInfo<TDomainObject,TIdent>(PropertyAccessors Id)`: the path to the Id, `GetId(obj)`, `CreateFilter(ident)` / `CreateFilter(idents)` (ready-made `Expression<Func<T,bool>>` for Where: equality and `Contains`).
- `IIdentityInfoSource.GetIdentityInfo<T>()` / `TryGetIdentityInfo<T>()` / by `Type`.

How the Id property is found:
1. If set explicitly via `AddIdentitySource(b => b.SetId((T x) => x.Key))`, it is used.
2. Otherwise `IdentityPropertyExtractor` looks for a property by the name from `IdentityPropertySourceSettings` (`"Id"` by default; change it with `b.SetSettings(new IdentityPropertySourceSettings("MyId"))`).

```csharp
services.AddIdentitySource(b => b.SetId((Location l) => l.MyId));   // as in _Example
var info = sp.GetRequiredService<IIdentityInfoSource>().GetIdentityInfo<Location, Guid>();
```

Consumers: `HierarchicalExpand`, `SecuritySystem`, the NHibernate integration tests.

## VisualIdentitySource (`src/VisualIdentitySource/`)

The structure mirrors IdentitySource:

| Project | Contents |
|---|---|
| `Anch.VisualIdentitySource.Abstractions` | `IVisualIdentityInfoSource`, `IVisualIdentityInfo<T>` (`Name` as `PropertyAccessors<T,string>`), `IDisplayObjectInfoSource`, `IDomainObjectDisplayService` (`Format(obj)`), `IVisualIdentityPropertyExtractor` |
| `Anch.VisualIdentitySource.Runtime` | `VisualIdentityInfoSource`, `DisplayObjectInfoSource`, `DomainObjectDisplayService`, `*Proxy`, `VisualIdentityPropertyExtractor`, `VisualIdentityPropertySourceSettings` |
| `Anch.VisualIdentitySource` | DI: `AddVisualIdentitySource(setup?)` |
| `Anch.VisualIdentitySource.Tests` | Tests (`DomainObjectDisplayServiceTests`, …) |

Default "name" lookup: the first existing property from the list `Login`, `Name`, `Code` (`VisualIdentityPropertySourceSettings.Default`). Override: `AddVisualIdentitySource(b => b.SetName(x => x.MyName))`, or the whole display function: `b.SetDisplay((T v) => v.MyName)`.

Only a public `string` property fits. The first one found in list order is taken.

`IDomainObjectDisplayService.Format(obj)` yields a string for messages and UI. Consumers in SecuritySystem: `PlainTypedSecurityContextStorage`, `HierarchicalTypedSecurityContextStorage` (names of security contexts), `PermissionDelegationValidator` (GeneralPermission).
