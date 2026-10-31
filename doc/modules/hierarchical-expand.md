# HierarchicalExpand

Папка `src/HierarchicalExpand/`. Раскрытие иерархий (деревья «родитель → дети») прямо в SQL-запросах за счёт предвычисленных таблиц/view связей «предок → потомок».

Пример: у пользователя есть право на `BusinessUnit` «Россия». Нужно получить Id «Россия» + всех дочерних. Модуль строит такой запрос по таблице связей без рекурсии в БД.

## Проекты

| Проект | Содержимое |
|---|---|
| `Anch.HierarchicalExpand.Domain` | `HierarchicalExpandType` ([Flags]: `None`, `Parents`, `Children`, `All`) |
| `Anch.HierarchicalExpand.Abstractions` | Модели описания иерархии + контракты расширителей и денормализаторов |
| `Anch.HierarchicalExpand.Runtime` | Реализации (расширители, денормализация связей) |
| `Anch.HierarchicalExpand` | DI: `AddHierarchicalExpand(setup)`, `IHierarchicalExpandSetup` |
| `Anch.HierarchicalExpand.Tests` | Юнит/DI-тесты на in-memory данных (`DomainObjectExpanderTests`, `AncestorLinkExtractorTests`, `DiTests`) |
| `…IntegrationTests` (+ `.EntityFramework`, `.NHibernate`) | Тесты на реальной БД (SQLite) |

## Модель описания иерархии

Для каждого иерархического доменного типа регистрируются (`IHierarchicalExpandSetup.AddHierarchicalInfo`):

| Тип | Что описывает |
|---|---|
| `HierarchicalInfo<T>(ParentPath)` | Выражение `x => x.Parent` |
| `AncestorLinkInfo<TDomain, TLink>(From, To)` | Таблица/сущность связей: из какого свойства link берётся «от» и «к» |
| `FullAncestorLinkInfo<T, TDirected, TUndirected>` | Пара связей: **Directed** (предок → потомок) и **Undirected** (в обе стороны, обычно view) |
| `DeepLevelInfo<T>(DeepLevel)` | Опционально: свойство «глубина узла» (`x => x.DeepLevel`) |

Регистрация (образец: `Anch.HierarchicalExpand.Tests/Environment/TestEnvironment.cs`):

```csharp
services.AddHierarchicalExpand(b => b.AddHierarchicalInfo(
    v => v.Parent,
    new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(l => l.Ancestor, l => l.Child),
    new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(v => v.Source, v => v.Target),
    v => v.DeepLevel));   // deepLevelPath необязателен
```

`AddHierarchicalExpand` автоматически вызывает `AddIdentitySource()` и `AddServiceProxyFactory()`. Требует, чтобы в контейнере были `IQueryableSource` (чтение данных; см. [core.md](core.md)) и `IGenericRepository`.

## Расширение по Id

`IHierarchicalObjectExpander<TIdent>` (получение: `IHierarchicalObjectExpanderFactory.Create(domainType)`):

| Метод | Результат |
|---|---|
| `Expand(idents, expandType)` | Id с учётом режима раскрытия |
| `GetExpandExpression(expandType)` | `Expression<Func<IEnumerable<TIdent>, IEnumerable<TIdent>>>` для встраивания в запросы |
| `TryGetSingleExpandExpression(expandType)` | то же для одного Id, `null` если не поддерживается |
| `ExpandWithParents(idents, expandType)` | Словарь `Id → Id родителя/источника` |

Семантика `HierarchicalExpandType` (см. `HierarchicalObjectAncestorLinkExpander`):
- `None`: без изменений;
- `Children`: потомки по **Directed**-связям;
- `Parents`: предки по **Directed**-связям в обратную сторону (`Directed.Reverse()`);
- `All`: через **Undirected** (и предки, и потомки).

Если тип не иерархический, `HierarchicalObjectExpanderTypeResolver` выдаёт `PlainHierarchicalObjectExpander`, который возвращает вход без изменений. Поэтому вызывающий код может не проверять, иерархичен ли тип.

`IDomainObjectExpander<TDomain>` (фабрика `IDomainObjectExpanderFactory<T>.Create()`) то же на уровне объектов: `GetAllParents(objects, ct)`, `GetAllChildren(objects, ct)`.

`IHierarchicalInfoSource`: `GetHierarchicalInfo<T>()`, `IsHierarchical(Type)`.

## Денормализация (поддержание таблиц связей)

Таблицы Directed-связей и поле `DeepLevel` хранятся в БД и должны синхронизироваться при изменении дерева:

- `IAncestorDenormalizer<T>.SyncAsync(updated, removed, ct)`: пересчитать связи для изменённых/удалённых узлов (`Runtime/Denormalization/AncestorDenormalizer.cs`, `AncestorLinkExtractor`, `SyncResult`).
- `IDeepLevelDenormalizer<T>.UpdateDeepLevels(objects, ct)`.
- Оба наследуют `IInitializer`: `Initialize(ct)` инициализирует (пересчитывает) данные для всех зарегистрированных типов; используется в тестовых окружениях.
- Недирективное представление (Undirected) в примерах это **SQL view** (`UndirectedAncestorViewScriptGenerator` в `_Example`), а не таблица, поддерживаемая кодом.

## Где что менять

- Новый режим раскрытия / оптимизация запроса: `Runtime/HierarchicalObjectAncestorLinkExpander.cs`.
- Как выбирается реализация для типа: `Runtime/HierarchicalObjectExpanderTypeResolver.cs`, `HierarchicalObjectExpanderFactory.cs` (через `IServiceProxyFactory`, см. [dependency-injection.md](dependency-injection.md)).
- Регистрации: `Anch.HierarchicalExpand/DependencyInjection/HierarchicalExpandSetup.cs`.

## Потребители

`Anch.SecuritySystem` (`ISecurityContextInfoSetup.SetHierarchicalInfo`, раскрытие контекстов безопасности по правилам с `HierarchicalExpandType`). См. [security-system.md](security-system.md).
