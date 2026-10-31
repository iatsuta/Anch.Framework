# Core

Папка `src/Core/`. Базовые библиотеки без доменной логики. На них опираются все остальные модули.

| Проект | Пакет | Назначение |
|---|---|---|
| `Anch.Core` | Anch.Core | Утилиты: Maybe, Expression-инструменты, расширения LINQ/Task/Type, базовые интерфейсы |
| `Anch.GenericRepository` | Anch.GenericRepository | Два минимальных интерфейса доступа к данным |
| `Anch.Parsing` | Anch.Parsing | Парсер-комбинаторы (используются OData) |
| `Anch.RelativePath` | Anch.RelativePath | «Относительный путь» между доменными типами (`From → To`) в виде выражений |
| `Anch.Threading` | Anch.Threading | Асинхронные примитивы синхронизации |
| `Anch.Core.Tests` | не пакет | Юнит-тесты Core |

## Anch.Core

Корневой namespace `Anch.Core` (подпространства: `Auth`, `ExpressionComparers`, `ExpressionEvaluate`, `DictionaryCache`, `Visitor`). Много файлов-расширений `*Extensions.cs`, смотрите по имени типа, который расширяется (`EnumerableExtensions`, `TaskExtensions`, `TypeExtensions`, `StringExtensions` и т.д.).

### Базовые контракты (в корне проекта)

| Тип | Файл | Что это |
|---|---|---|
| `IFactory<TResult>`, `IFactory<TArg,TResult>` | `IFactory.cs` | Фабрика |
| `IInitializer` | `IInitializer.cs` | `Task Initialize(CancellationToken)`. Используется тестовой инфраструктурой и денормализаторами (регистрируется как keyed-сервис) |
| `IQueryableInjector<T>` | `IQueryableInjector.cs` | `IQueryable<T> Inject(IQueryable<T>)`. «Добавь к запросу фильтр/сортировку». Реализуют `ISecurityProvider<T>`, `SelectOperation<T>` (OData) |
| `IServiceProxyFactory`, `IServiceProxyBinder` | одноимённые | Контракты «прокси-фабрики» (реализация в `Anch.DependencyInjection/ServiceProxy`) |
| `IDefaultCancellationTokenSource` | | Источник токена по умолчанию для синхронных мест |
| `ICultureSource` / `CultureSource` | | Источник текущей культуры |
| `Ignore` | `Ignore.cs` | `record struct`, аналог `Unit`/void для generic-параметров |
| `MergeResult<TSource,TTarget>` | `MergeResult.cs` | Результат слияния коллекций: `AddingItems`, `CombineItems`, `RemovingItems` |
| `ICurrentUser` | `Auth/ICurrentUser.cs` | `string Name`; константы ключей `RawKey/ImpersonatedKey/DefaultKey` (keyed-регистрации). `FixedCurrentUser` фиксированный пользователь |

### Maybe (`Maybe/`)

`Maybe<T>` (struct) с `HasValue`, `Value`, `Just`, `Nothing`. Хелперы: `Maybe.Return`, `ToMaybe`, `OfCondition`, `OfTryMethod`; LINQ-расширения (`from x in maybe select ...`) в `LinqMaybeExtensions`. Широко используется внутри (например, `ExpressionExtensions.GetConstantValue<T>()` возвращает `Maybe`).

### Выражения (Expression)

| Что | Где | Зачем |
|---|---|---|
| `PropertyAccessors<TSource,TProp>` | `PropertyAccessors.cs` | `record`: путь-выражение + `Getter` + `Setter`. Равенство через `ExpressionComparer`. Строится из лямбды: `expr.ToPropertyAccessors()` |
| `LambdaExpressionPath` | `LambdaExpressionPath.cs` | Цепочка лямбд-свойств (`a.B.C`), сравнимая по значению. Создаётся из строки: `LambdaExpressionPath.Create(type, ["B","C"])`. Основа fetch-правил GenericQueryable |
| `ExpressionComparers/*` | | Структурное сравнение/хэширование Expression (`ExpressionComparer.Default`). Отдельный компаратор на каждый тип узла |
| `ExpressionEvaluate/*` | | Компиляция лямбд с кэшем: `IExpressionEvaluator`, `LambdaCompileCache`, `LambdaCompileMode` (флаги `InjectMaybe`, `IgnoreStringCase`, `OptimizeBooleanLogic`, `All`) |
| `Visitor/*` | | `ExpressionVisitor`'ы: `ExpandConstVisitor`, `OptimizeBooleanLogicVisitor` (сворачивает константные условия), `OverrideExpressionVisitor` |
| `ExpressionHelper`, `ExpressionExtensions`, `LinqExpressionExtensions`, `FuncHelper` | корень | Построение/композиция выражений (`Select`, `UpdateBody`, `GetEqualityWithExpr`, …) |

### Прочее

- `DictionaryCache/` : `IDictionaryCache<TKey,TValue>`, простой потокобезопасный кэш-словарь.
- `DeepEqualsCollection<T>`, `ArrayComparer`, `EqualityComparerImpl` — сравнение коллекций по значению (используется в `SecurityPathRestriction`).
- `PipeObjectExtensions` (`.Pipe(...)`), `LazyHelper`, `TaskResultHelper`.

## Anch.GenericRepository

Два интерфейса:
```csharp
IGenericRepository { SaveAsync<T>(T, ct); RemoveAsync<T>(T, ct); }   // запись
IQueryableSource  { IQueryable<T> GetQueryable<T>(); }                // чтение
```
Приложение реализует их поверх своей ORM, а модули фреймворка (HierarchicalExpand, SecuritySystem) только зависят от интерфейсов. В тестах это `TestQueryableSource` и `Substitute.For<IGenericRepository>()`.

## Anch.Parsing

Монада парсер-комбинаторов: `delegate ParsingState<TInput,TValue> Parser<TInput,TValue>(TInput input)`. Файлы: `Parser.cs`, `ParserCombinatorsMonad.cs` (LINQ-синтаксис), `ParserCombinatorExtensions.cs`, `CharParsers.cs`, `DecimalParser.cs`, `Parsers.cs`. Единственный потребитель в репозитории `Anch.OData.Runtime`.

## Anch.RelativePath

`IRelativeDomainPathInfo<TFrom,TTo>`: описание связи между типами с помощью выражения (одиночное свойство или коллекция):
- `CreateCondition(Expression<Func<TTo,bool>>)` превращает фильтр по `TTo` в фильтр по `TFrom`;
- `GetRelativeObjects(TFrom)`, `Select(...)`, `OverrideInput(...)`.

Реализации: `SingleRelativeDomainPathInfo`, `ManyRelativeDomainPathInfo`, `SelfRelativeDomainPathInfo`. Регистрация: `services.AddRelativeDomainPath<TFrom,TTo>(x => x.Prop, key?)` (`DependencyInjection/ServiceCollectionExtensions.cs`). Используется SecuritySystem для зависимых доменных объектов (`SetDependency`, `CurrentUser` с `RelativePathKey`).

## Anch.Threading

- `AsyncLocker` / `IAsyncLocker`: `await locker.CreateScope(ct)` → `IDisposable` (асинхронный mutex).
- `AsyncOnce`: выполнить async-действие один раз, повторные вызовы получают то же исключение (`EnsureExecutedAsync`).
- `AsyncLazy<T>`.
