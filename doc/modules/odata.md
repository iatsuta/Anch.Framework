# OData

Папка `src/OData/`. Парсер строки OData-запроса (V2/V3-стиль: `$filter`, `$orderby`, `$top`, `$skip`) в типизированные LINQ `Expression`'ы, готовые к применению к `IQueryable`.

Пример входа (файл `Anch.OData.Tests/request.odata`):
```
$top=30&$filter=Active eq true and (ManagementUnit/Id eq guid'01f5…' or …)
```

## Конвейер

```
string ──RawSelectOperationParser──▶ SelectOperation (RawDomain: свой AST, без типов)
                                         │ SelectOperationConverter / LambdaExpressionConverter
                                         ▼
                                  SelectOperation<TDomainObject> (Domain: System.Linq.Expressions)
                                         │ .Inject(IQueryable<T>)
                                         ▼
                                  IQueryable<T> с Where/OrderBy/Skip/Take
```

Оба парсера кэшируют результат по строке запроса через `ICacheProvider` (см. [caching.md](caching.md)), поэтому повторный разбор одной и той же строки бесплатный.

## Проекты

| Проект | Содержимое |
|---|---|
| `Anch.OData.RawDomain` | Нетипизированная модель: `SelectOperation` (Filter-лямбда, Orders, SkipCount, TakeCount), `QueryLanguage/*` (узлы AST: `BinaryExpression`, `UnaryExpression`, `MethodExpression`, `PropertyExpression`, константы по типам: Boolean/Int32/Int64/Decimal/DateTime/Guid/String/Enum/Null), `MethodExpressionType` (`StringContains`, `StringStartsWith`, `StringEndsWith`, `CollectionAny`, `CollectionAll`) |
| `Anch.OData.Domain` | Типизированная модель: `SelectOperation<TDomain>` (реализует `IQueryableInjector<T>`: `Filter`, `Orders`, `SkipCount`, `TakeCount`, `HasPaging`, `WithoutPaging()`), `SelectOrder<TDomain,TKey>` |
| `Anch.OData.Abstractions` | Контракты: `ISelectOperationParser`, `IRawSelectOperationParser`, `ISelectOperationConverter`, `ILambdaExpressionConverter`, `IParsingExceptionFactory`, `ODataParsingException` |
| `Anch.OData.Runtime` | `RawSelectOperationParser` + `InternalParser/*` (грамматика на `Anch.Parsing`), `SelectOperationConverter`, `LambdaExpressionConverter(Base)`, `SelectOperationParser`, `ParsingExceptionFactory` |
| `Anch.OData` | DI: `AddOData(setup?)`, `IODataSetup.SetParsingExceptionFactory<T>()` |
| `Anch.OData.Tests` | `MainTest`, `ODataParsingAutoTests`, `ODataLiftTests`, `DateParsingTests` + тестовые данные `LiftTestData/*` |

## Использование

```csharp
services.AddOData();                                 // + AddAnchCaching() подтягивается через Anch.OData

var parser = sp.GetRequiredService<ISelectOperationParser>();
SelectOperation<Employee> op = parser.Parse<Employee>("$filter=Active eq true&$top=30");
IQueryable<Employee> q = op.Inject(db.Employees);    // IQueryableInjector
```

Ошибки разбора превращаются в исключение через `IParsingExceptionFactory` (по умолчанию `ParsingExceptionFactory` → `ODataParsingException`). Для своего типа исключения (например, HTTP 400) задайте `AddOData(s => s.SetParsingExceptionFactory<MyFactory>())`.

Культура парсинга берётся из `ICultureSource` (необязательная зависимость), иначе `InvariantCulture`.

## Где что менять

- Новая функция/оператор в языке: грамматика `Runtime/InternalParser/*` + узел в `RawDomain/QueryLanguage/*` + конвертация в `Runtime/LambdaExpressionConverter*.cs`, `StandartExpressionExtensions.cs`.
- Новый тип константы: `RawDomain/QueryLanguage/Constant/*` и разбор в `RawSelectOperationParserParser`.
- Добавлять тест в `ODataParsingAutoTests`/`ODataLiftTests` (`ODataLiftTests` проверяют выполнение фильтров на nullable-полях и вложенных объектах, где вложенный объект может быть `null`; данные в `LiftTestData/*`).
