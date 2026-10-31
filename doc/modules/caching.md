# Caching

Folder `src/Caching/`. A minimal cache abstraction.

| Project | Contents |
|---|---|
| `Anch.Caching.Abstractions` | `ICache` / `ICache<TKey,TValue>` (`GetOrAdd(key, factory)`, `RootKey`, `KeyType`, `ValueType`), `ICacheProvider` (`GetCache<TKey,TValue>(object key)`), `CacheExtensions` |
| `Anch.Caching.Runtime` | `CacheProvider` (default in-memory implementation), `Cache`, `CacheProxy<,>` (`ICache<,>` implementation via the provider) |
| `Anch.Caching` | DI: `AddAnchCaching(setup?)`, `ICachingSetup.SetCacheProvider<T>()` |

## Usage

```csharp
services.AddAnchCaching();                                        // default CacheProvider
services.AddAnchCaching(s => s.SetCacheProvider<MyRedisProvider>()); // custom implementation

// in a service:
public class Foo(ICacheProvider provider)
{
    private readonly ICache<string, Item> cache = provider.GetCache<string, Item>("Foo");
    public Item Get(string key) => cache.GetOrAdd(key, Load);
}
```

Registers `ICache<,>` → `CacheProxy<,>` (singleton) and `ICacheProvider`. If a provider is already registered and no new one is set, the registration is left unchanged (`CachingSetup.Initialize`).

Consumers in the repository: `Anch.OData.Runtime` (cache of parsed queries). The module has no test projects of its own.
