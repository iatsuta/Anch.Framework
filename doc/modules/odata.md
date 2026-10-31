# OData

Folder `src/OData/`. A parser of an OData query string (V2/V3 style: `$filter`, `$orderby`, `$top`, `$skip`) into typed LINQ `Expression`s ready to be applied to an `IQueryable`.

Example input (file `Anch.OData.Tests/request.odata`):
```
$top=30&$filter=Active eq true and (ManagementUnit/Id eq guid'01f5…' or …)
```

## Pipeline

```
string ──RawSelectOperationParser──▶ SelectOperation (RawDomain: own AST, untyped)
                                         │ SelectOperationConverter / LambdaExpressionConverter
                                         ▼
                                  SelectOperation<TDomainObject> (Domain: System.Linq.Expressions)
                                         │ .Inject(IQueryable<T>)
                                         ▼
                                  IQueryable<T> with Where/OrderBy/Skip/Take
```

Both parsers cache the result by query string via `ICacheProvider` (see [caching.md](caching.md)), so re-parsing the same string is free.

## Projects

| Project | Contents |
|---|---|
| `Anch.OData.RawDomain` | Untyped model: `SelectOperation` (Filter lambda, Orders, SkipCount, TakeCount), `QueryLanguage/*` (AST nodes: `BinaryExpression`, `UnaryExpression`, `MethodExpression`, `PropertyExpression`, constants by type: Boolean/Int32/Int64/Decimal/DateTime/Guid/String/Enum/Null), `MethodExpressionType` (`StringContains`, `StringStartsWith`, `StringEndsWith`, `CollectionAny`, `CollectionAll`) |
| `Anch.OData.Domain` | Typed model: `SelectOperation<TDomain>` (implements `IQueryableInjector<T>`: `Filter`, `Orders`, `SkipCount`, `TakeCount`, `HasPaging`, `WithoutPaging()`), `SelectOrder<TDomain,TKey>` |
| `Anch.OData.Abstractions` | Contracts: `ISelectOperationParser`, `IRawSelectOperationParser`, `ISelectOperationConverter`, `ILambdaExpressionConverter`, `IParsingExceptionFactory`, `ODataParsingException` |
| `Anch.OData.Runtime` | `RawSelectOperationParser` + `InternalParser/*` (grammar built on `Anch.Parsing`), `SelectOperationConverter`, `LambdaExpressionConverter(Base)`, `SelectOperationParser`, `ParsingExceptionFactory` |
| `Anch.OData` | DI: `AddOData(setup?)`, `IODataSetup.SetParsingExceptionFactory<T>()` |
| `Anch.OData.Tests` | `MainTest`, `ODataParsingAutoTests`, `ODataLiftTests`, `DateParsingTests` + test data `LiftTestData/*` |

## Usage

```csharp
services.AddOData();                                 // + AddAnchCaching() is pulled in via Anch.OData

var parser = sp.GetRequiredService<ISelectOperationParser>();
SelectOperation<Employee> op = parser.Parse<Employee>("$filter=Active eq true&$top=30");
IQueryable<Employee> q = op.Inject(db.Employees);    // IQueryableInjector
```

Parsing errors are turned into an exception via `IParsingExceptionFactory` (by default `ParsingExceptionFactory` → `ODataParsingException`). For your own exception type (for example, HTTP 400) set `AddOData(s => s.SetParsingExceptionFactory<MyFactory>())`.

The parsing culture is taken from `ICultureSource` (an optional dependency), otherwise `InvariantCulture`.

## What to change where

- A new function/operator in the language: grammar `Runtime/InternalParser/*` + a node in `RawDomain/QueryLanguage/*` + conversion in `Runtime/LambdaExpressionConverter*.cs`, `StandartExpressionExtensions.cs`.
- A new constant type: `RawDomain/QueryLanguage/Constant/*` and parsing in `RawSelectOperationParserParser`.
- Add a test to `ODataParsingAutoTests`/`ODataLiftTests` (`ODataLiftTests` check the execution of filters on nullable fields and nested objects where a nested object may be `null`; data in `LiftTestData/*`).
