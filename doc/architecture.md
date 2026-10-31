# Архитектура и соглашения

Документ описывает, как устроен репозиторий в целом. Модули разобраны в [modules/](modules/).

## 1. Раскладка репозитория

```
Anch.Framework/
├── global.json                     # test runner = Microsoft.Testing.Platform
├── src/
│   ├── Anch.Framework.slnx         # единственное решение (формат slnx)
│   ├── Directory.Build.props       # net10.0, Nullable, LangVersion=latest, общие NoWarn
│   ├── Directory.Packages.props    # центральные версии NuGet-пакетов (CPM)
│   ├── __RootSolutionItems/        # файлы, линкуемые во все проекты (AssemblyInfo, xunit-настройки)
│   ├── Core/                       # базовые библиотеки без привязки к домену
│   ├── DependencyInjection/        # расширения над Microsoft.Extensions.DependencyInjection
│   ├── Caching/
│   ├── IdentitySource/             # «как достать Id из доменного объекта»
│   ├── VisualIdentitySource/       # «как достать Name/Display из доменного объекта»
│   ├── GenericQueryable/           # ORM-независимые async LINQ + fetch-правила
│   ├── HierarchicalExpand/         # раскрытие иерархий (родители/дети)
│   ├── OData/                      # парсер OData-запросов в Expression
│   ├── SecuritySystem/             # система прав (самый большой модуль)
│   ├── Testing/                    # инфраструктура интеграционных тестов (xunit v3)
│   └── Workflow/                   # ⚠ ПОКА НЕ ИЗУЧАТЬ, см. modules/workflow.md
├── doc/                            # эта документация
├── graphify-out/                   # артефакты graphify (граф знаний кода), не исходники
└── .github/workflows/              # build.yml (CI), publish.yml (NuGet)
```

Каждая папка модуля содержит несколько csproj. Имя проекта = `Anch.<Модуль>[.<Слой>]`, имя пакета NuGet совпадает (`<PackageId>`).

> В рабочей копии могут лежать каталоги без префикса `Anch.` (например `src/GenericQueryable/GenericQueryable`, `src/SecuritySystem/SecuritySystem`). Это остатки после переименования: внутри только `bin/obj`, в git их нет. Исходники всегда в каталогах `Anch.*`.

## 2. Слои внутри модуля (главное правило навигации)

Почти каждый модуль разбит на одни и те же слои. Зная слой, понятно, что в проекте лежит:

| Суффикс проекта | Что внутри | Зависимости |
|---|---|---|
| `.Domain` | Чистые модели/enum'ы без логики (`HierarchicalExpandType`, `ISecurityContext`, `SelectOperation`) | минимум |
| `.Abstractions` | Публичные интерфейсы и контракты (`IIdentityInfoSource`, `ISecuritySystem`) | Core, Domain |
| `.Runtime` | Реализации интерфейсов (классы `*Source`, `*Service`, `*Expander`) | Abstractions |
| *(без суффикса)* `Anch.<Модуль>` | **Точка входа для потребителя**: `AddXxx(...)` для `IServiceCollection` и `IXxxSetup` (fluent-настройка) | Runtime + `Anch.DependencyInjection` |
| `.<ORM/технология>` | Адаптеры (`.EntityFramework`, `.NHibernate`, `.Sqlite`, `.Xunit`) | основной пакет + внешняя библиотека |
| `.Tests`, `.UnitTests`, `.IntegrationTests*`, `.DiTests` | Тесты | см. [building-and-testing.md](building-and-testing.md) |

Не везде есть все слои: например, у `Caching` нет `.Domain`, у `Core` слоёв нет вовсе.

Правило для потребителя: ссылаться достаточно на пакет без суффикса (`Anch.SecuritySystem`), он транзитивно тянет Runtime/Abstractions. Библиотеки, которым нужны только контракты (например, доменный слой приложения), ссылаются на `.Abstractions`.

## 3. Паттерн регистрации: `AddXxx(Action<IXxxSetup>)`

Единый способ подключения любого модуля к DI:

```csharp
services.AddIdentitySource(b => b.SetId((MyEntity e) => e.Key));   // IdentitySource
services.AddHierarchicalExpand(b => b.AddHierarchicalInfo(...));   // HierarchicalExpand
services.AddSecuritySystem(b => b.AddSecurityRole(...));           // SecuritySystem
```

Как это работает (см. `src/DependencyInjection/Anch.DependencyInjection/ServiceCollectionExtensions.cs`):

1. Есть интерфейс `IServiceInitializer<TContainer>` с методом `Initialize(TContainer)`.
2. Каждый модуль определяет класс `XxxSetup : IXxxSetup, IServiceInitializer`. Методы `IXxxSetup` только **накапливают** настройки.
3. `services.Initialize<XxxSetup>(setupAction)` создаёт `XxxSetup`, вызывает `setupAction`, затем `Initialize(services)`, где настройки превращаются в регистрации в контейнере.
4. Вложенные расширения (GeneralPermission, VirtualPermission, Notification) делают то же самое поверх `ISecuritySystemSetup`: `securitySystemSetup.Initialize<ISecuritySystemSetup, XxxSetup>(...)`.

Где искать настройку модуля: файлы `DependencyInjection/*Setup.cs` и `DependencyInjection/ServiceCollectionExtensions.cs` в проекте без суффикса.

Полезные хелперы DI из `Anch.DependencyInjection`:
- `AlreadyInitialized<T>()` проверка, зарегистрирован ли сервис;
- `AddScopedFrom<TService, TSource>(selector)`, `AddSingletonFrom<…>` регистрация сервиса как проекции другого;
- `ReplaceSingleton/ReplaceScoped` замена регистрации;
- `AddValidator<T>()` + `Validate()` ручная валидация коллекции сервисов (см. [dependency-injection.md](modules/dependency-injection.md)).

## 4. Граф зависимостей модулей

```
Core (Anch.Core, Threading, Parsing, RelativePath, GenericRepository)
  └── DependencyInjection
        ├── Caching ──────────────┐
        ├── IdentitySource        │
        ├── VisualIdentitySource  │
        ├── GenericQueryable      │
        ├── HierarchicalExpand ← IdentitySource, GenericQueryable.Abstractions, GenericRepository
        ├── OData ← Caching, Parsing
        ├── Testing
        └── SecuritySystem ← HierarchicalExpand, VisualIdentitySource, GenericQueryable.Abstractions,
                             IdentitySource.Abstractions, RelativePath, GenericRepository
```

Циклических зависимостей нет. `Anch.Core` и `Anch.GenericRepository` ни от чего не зависят.

## 5. Конвенции кода

- **Целевая платформа**: `net10.0`, `LangVersion=latest`, включены `Nullable` и `ImplicitUsings`.
- **C# 14 `extension(...)`-блоки** вместо классических `this`-методов:
  ```csharp
  public static class ServiceCollectionExtensions
  {
      extension(IServiceCollection services)
      {
          public IServiceCollection AddFoo() => ...;
      }
  }
  ```
  Это нормально. При поиске метода расширения грепайте по имени метода, а не по `this IServiceCollection`.
- **Выражения как данные**: пути к свойствам передаются как `Expression<Func<T, TProp>>` (а не строки). Они хранятся в `PropertyAccessors<TSource,TProp>` (путь + скомпилированные getter/setter) и сравниваются через `ExpressionComparer` (см. [core.md](modules/core.md)). Поэтому многие `record` в проекте имеют кэш `hashCode` и ручной `Equals`.
- **`CancellationToken ct`** последний параметр во всех async-методах. Иногда используется `IDefaultCancellationTokenSource` для синхронных мест.
- **`record`/`record struct`** для моделей и результатов. Наследование records широко используется (см. `DomainSecurityRule`).
- **Ключевые (keyed) сервисы** активно применяются: `AddKeyedSingleton/Scoped` с `string`/enum-ключом (например, `ElementKey = "Element"`, `ICurrentUser.RawKey/ImpersonatedKey/DefaultKey`, `ITestEnvironment.MainServiceProviderKey`).
- **Тесты** используют xunit v3 + свою надстройку `Anch.Testing.Xunit` (`[AnchFact]`, `[assembly: AnchTestFramework<TestEnvironment>]`). Подробнее в [testing.md](modules/testing.md).
- **CRLF**: `.gitattributes` принудительно задаёт `eol=crlf` для `*.cs`, `*.csproj` и др.
- Файлы начинаются с BOM (UTF-8 with BOM). При ручном редактировании/сравнении учитывать это.

## 6. Что НЕ является исходным кодом

- `graphify-out/` кэш и отчёт инструмента graphify (граф знаний). Файл `graphify-out/GRAPH_REPORT.md` можно использовать как быструю карту «сообществ» классов, но он генерируется автоматически и может отставать.
- `src/TestResults/`, `bin/`, `obj/`, `*.DotSettings.user` локальные артефакты.
- `src/SecuritySystem/Anch.SecuritySystem.Configurator/configurator-ui/` Angular-приложение (UI конфигуратора), собирается `npm` и встраивается в сборку как embedded-ресурсы.
