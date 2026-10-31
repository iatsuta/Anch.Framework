# GenericQueryable

Папка `src/GenericQueryable/`. Есть собственный человеческий README: [`src/GenericQueryable/readme.md`](../../src/GenericQueryable/readme.md) (ссылка на `GenericQueryableExtensions.cs` в нём устарела; файл лежит в `Anch.GenericQueryable.Abstractions/`).

## Зачем

Async LINQ (`ToListAsync`, `FirstOrDefaultAsync`, `Include`…) в EF/NHibernate это статические методы, привязанные к ORM. Прикладной слой не должен на них ссылаться. GenericQueryable даёт ORM-независимые методы `GenericToListAsync()`, `GenericSingleAsync()` … и `WithFetch(...)` (eager loading), а исполнение делегирует провайдеру, «зашитому» в `IQueryable`.

## Проекты

| Проект | Содержимое |
|---|---|
| `Anch.GenericQueryable.Abstractions` | Публичное API и контракты (см. ниже) |
| `Anch.GenericQueryable.Runtime` | Реализации fetch-расширения: `RootFetchService`, `CompositeFetchRuleExpander`, `FetchRuleHeaderExpander`, `UntypedFetchExpander`, `RootFetchRuleExpander` |
| `Anch.GenericQueryable` | DI: `AddGenericQueryable(setup)`, `IGenericQueryableSetup`, `IGenericQueryableExtension` |
| `Anch.GenericQueryable.EntityFramework` | Адаптер EF Core: `UseGenericQueryable()` для `DbContextOptionsBuilder`, `EfFetchService`, `VisitedEfQueryProvider`, `EfTargetMethodExtractor` |
| `Anch.GenericQueryable.NHibernate` | Адаптер NHibernate: `AddNHibernateGenericQueryable()`, `Configuration.SetGenericQueryProvider()`, `NHibFetchService`, `VisitedNHibQueryProvider` |
| `Anch.GenericQueryable.UnitTests` | `MainTests`, `FetchRuleExpanderTests` |
| `Anch.GenericQueryable.IntegrationTests` (+ `.EntityFramework`, `.NHibernate`) | Общие тесты + два окружения (SQLite) |

## Как это работает

1. **Расширения** (`GenericQueryableExtensions.cs`): `source.GenericToListAsync(ct)`, `GenericSingleAsync`, `GenericAnyAsync`, `GenericContainsAsync`, `GenericAllAsync`, `GenericCountAsync`, `GenericFirst(OrDefault)Async`, `GenericSingle(OrDefault)Async`, `GenericToDictionaryAsync`, `GenericToHashSetAsync`, `GenericSumAsync`, `GenericAsAsyncEnumerable` и т.д. Каждое вызывает `source.Execute(() => <тот же метод>)`.
2. `Execute` берёт `IGenericQueryableExecutor` из `IGenericQueryProvider` (т.е. `IQueryable.Provider` должен реализовать `IGenericQueryProvider`), и передаёт выражение вызова.
3. `GenericQueryableExecutor` перебирает `IMethodRedirector`'ы (`QueryableMethodRedirector`, `AsyncEnumerableMethodRedirector`, `MethodRedirector`) и подменяет вызов `GenericXxxAsync` на реальный ORM-метод (`ITargetMethodExtractor` знает соответствия: `EfTargetMethodExtractor`, `NHibTargetMethodExtractor`). Результат компилируется и кэшируется (`LambdaCompileCache`).
4. Для **обычных** `IQueryable` (LINQ to Objects, в тестах) используется `GenericQueryableExecutor.Sync`: методы сводятся к синхронным/`AsyncEnumerable`-аналогам, а `WithFetch` игнорируется (`IgnoreFetchService`).
5. ORM-адаптеры подменяют `IQueryProvider` у запросов своим «visited»-провайдером (`VisitedEfQueryProvider`, `VisitedNHibQueryProvider`), через который проходит `ExpressionVisitor` (точка расширения: `SetVisitor<T>()`, на ней строится, например, `LinkIdVisitor` в интеграционных тестах).

## Fetch-правила (`Abstractions/Fetching/`)

`WithFetch` описывает, какие связи подгрузить:

```csharp
query.WithFetch(r => r.Fetch(x => x.Orders).ThenFetch(o => o.Items));  // типизированно
query.WithFetch("Orders.Items");                                      // строкой (UntypedFetchRule)
query.WithFetch(AppFetchRule.TestFetchRule);                          // именованное правило (FetchRuleHeader)
```

- `FetchRule<TSource>`: корень иерархии (record). Подтипы: `PropertyFetchRule<TSource[,TProp]>` (цепочки `LambdaExpressionPath`), `UntypedFetchRule`, `FetchRuleHeader<TSource,TValue>` (именованный «заголовок»), `CompositeFetchRule`.
- `IFetchRuleExpander.TryExpand(rule)`: превращает «заголовок»/строку/композит в `PropertyFetchRule`. Свои раскрыватели добавляются: `AddFetchRuleExpander<T>()`; готовое соответствие «заголовок → путь»: `AddFetchRule(header, implementation)`.
- `IFetchService.ApplyFetch(query, rule)`: применяет ORM-специфичный `Include`/`Fetch` (`EfFetchService`, `NHibFetchService`).

## Подключение

EF Core:
```csharp
services.AddDbContext<MyDb>(o => o.UseGenericQueryable());
// с настройкой fetch-правил и т.п. через свой setup-тип (IEfGenericQueryableExtensionSetup.SetSetupType<T>()):
services.AddDbContext<MyDb>(o => o.UseGenericQueryable(s => s.SetSetupType<EfGenericQueryableExtensionSetup>()));
// образец: Anch.GenericQueryable.IntegrationTests.EntityFramework/Environment/TestDbContext.cs
```
NHibernate:
```csharp
cfg.SetGenericQueryProvider();                                           // Configuration (NHibConfigurationSource)
services.AddNHibernateGenericQueryable(b => b.AddFetchRuleExpander<AppFetchRuleExpander>());
```

## Где что менять

- Новый async-метод (`GenericXxxAsync`): `Abstractions/GenericQueryableExtensions.cs` + проверить, что `ITargetMethodExtractor`'ы (`Services/*TargetMethodExtractor.cs`, `EfTargetMethodExtractor`, `NHibTargetMethodExtractor`) умеют найти соответствие.
- Новое fetch-правило: `Abstractions/Fetching/*` (модель) + `Runtime/Fetching/*` (раскрытие).
- Поддержка новой ORM: по образцу `Anch.GenericQueryable.NHibernate` (query provider, fetch service, target method extractor, DI-расширение).
