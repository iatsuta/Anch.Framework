# Caching

Папка `src/Caching/`. Минимальная абстракция кэша.

| Проект | Содержимое |
|---|---|
| `Anch.Caching.Abstractions` | `ICache` / `ICache<TKey,TValue>` (`GetOrAdd(key, factory)`, `RootKey`, `KeyType`, `ValueType`), `ICacheProvider` (`GetCache<TKey,TValue>(object key)`), `CacheExtensions` |
| `Anch.Caching.Runtime` | `CacheProvider` (реализация по умолчанию, in-memory), `Cache`, `CacheProxy<,>` (реализация `ICache<,>` через провайдер) |
| `Anch.Caching` | DI: `AddAnchCaching(setup?)`, `ICachingSetup.SetCacheProvider<T>()` |

## Использование

```csharp
services.AddAnchCaching();                                        // CacheProvider по умолчанию
services.AddAnchCaching(s => s.SetCacheProvider<MyRedisProvider>()); // своя реализация

// в сервисе:
public class Foo(ICacheProvider provider)
{
    private readonly ICache<string, Item> cache = provider.GetCache<string, Item>("Foo");
    public Item Get(string key) => cache.GetOrAdd(key, Load);
}
```

Регистрирует `ICache<,>` → `CacheProxy<,>` (singleton) и `ICacheProvider`. Если провайдер уже зарегистрирован и новый не задан, регистрация не меняется (`CachingSetup.Initialize`).

Потребители в репозитории: `Anch.OData.Runtime` (кэш разобранных запросов). Отдельных тестовых проектов у модуля нет.
