# Testing

Папка `src/Testing/`. Собственная тестовая инфраструктура поверх **xunit v3** и `Microsoft.Extensions.DependencyInjection`. Её используют почти все тестовые проекты репозитория, поэтому, чтобы понять любой тест, сначала прочитайте этот файл.

## Идея

Вместо ручного создания объектов в тесте, тестовый класс получает зависимости **через конструктор** из DI-контейнера. Контейнеры (service providers) собираются заранее и лежат в **пуле**, а тесты выполняются параллельно, каждый берёт свободный провайдер и после теста возвращает его. Для тестов с БД на каждый провайдер из пула создаётся своя копия БД (snapshot), так что параллельные тесты не мешают друг другу.

## Проекты

| Проект | Содержимое |
|---|---|
| `Anch.Testing.Abstractions` | `ITestEnvironment`, `ITestEnvironmentHook`, `EnvironmentHookType` (`Before`/`After`), `ServiceProviderBuildContext`, `ServiceProviderIndex` (`Main` = -1, остальные = номер в пуле), `IParallelizationSettings`, `AllowParallelizationConstraint`, `IMainServiceProviderSettings`, `AddEnvironmentHook(...)` |
| `Anch.Testing` | Пул провайдеров: `ServiceProviderPool`, `InternalServiceProviderPool`, `IServiceProviderPoolFactory`, `ParallelizationSettings`, `RunEnvironmentHooks` |
| `Anch.Testing.Xunit` | Интеграция с xunit v3: `[AnchFact]`, `[AnchInlineData]`, `[AnchMemberData]`, `[assembly: AnchTestFramework<TEnv>]`, `Engine/*` (собственные runner'ы: `AnchTestFramework`, `AnchTestCaseRunner`, …) |
| `Anch.Testing.Database` | Тесты с БД: `DatabaseTestEnvironment`, `IDatabaseManager`, строки подключения, snapshot-инициализация, hooks подготовки/очистки БД, `AddDatabaseTesting` |
| `Anch.Testing.Database.Sqlite` | Провайдер для SQLite (`SqliteDatabaseTestingProvider`, `SqliteDatabaseManager`): БД = файл, snapshot = копия файла |
| `Anch.Testing.Database.Configuration` | Строки подключения из `IConfiguration` (`ConfigurationTestEnvironment`) |
| `Anch.Testing.Tests` | Тесты самой инфраструктуры (`TestEnvironment`, `TheoryTests`) |

## Как писать тесты

1. На уровне сборки указать окружение:
   ```csharp
   [assembly: AnchTestFramework<TestEnvironment>]      // TestEnvironment : ITestEnvironment, new()
   // или [assembly: AnchTestFramework]                // без окружения
   ```
2. Окружение собирает контейнер:
   ```csharp
   public class TestEnvironment : ITestEnvironment
   {
       public IServiceProvider BuildServiceProvider(IServiceCollection services, ServiceProviderBuildContext buildContext) =>
           services
               .AddXxx(...)
               .AddValidator<DuplicateServiceUsageValidator>().Validate()
               .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
   }
   ```
3. Тестовый класс принимает сервисы в конструкторе (`IServiceProvider` или конкретные интерфейсы), методы помечаются `[AnchFact]`/`[Theory]` (теория с `[MemberData]` тоже поддерживается каркасом). `CancellationToken ct` можно принимать параметром теста.
4. Хуки окружения: `services.AddEnvironmentHook(EnvironmentHookType.Before|After, sp => ...)`. `Before` вызываются в обратном порядке регистрации, `After` в прямом. Пример: сброс счётчиков/хранилища после теста (`HierarchicalExpand.Tests/Environment/TestEnvironment.cs`).

> Если в тестовом проекте не видно `Fact`/`Theory`, а есть `[AnchFact]`, это нормально. Запуск через `dotnet test --project <папка проекта>` (Microsoft.Testing.Platform, см. [building-and-testing.md](../building-and-testing.md)).

## Тесты с БД (`Anch.Testing.Database`)

Базовый класс `DatabaseTestEnvironment : ITestEnvironment` (наследуют, например, окружения интеграционных тестов EF/NHibernate). Что нужно задать в наследнике:
- `RawConnectionString`: основная (эталонная) строка подключения (`TestConnectionString`);
- `InitDatabase(IDatabaseTestingSetup)`: подключить провайдер БД (`SetProvider<SqliteDatabaseTestingProvider>()`), инициализаторы пустой схемы (`SetEmptySchemaInitializer<T>`) и тестовых данных (`SetTestDataInitializer<T>`), политику параллелизма (`SetParallelization(bool)`).

`TestDatabaseSettings`: `RawConnectionString`, `InitMode`, `RemoveDatabaseOnFailure` (по умолчанию true).

`DatabaseInitMode`:
- `RebuildSnapshot`: каждый прогон пересоздаёт эталонную БД (по умолчанию);
- `ReuseSnapshot`: использовать готовую, если существует;
- `External`: БД управляется снаружи.

Жизненный цикл:
1. **Main provider** (`Index.IsMain`) инициализируется один раз `DatabaseSnapshotInitializer` (keyed-`IInitializer` с ключом `ITestEnvironment.MainServiceProviderKey`): создаёт снимки `EmptySnapshot` (пустая схема, `IEmptySchemaInitializer`) и `FilledSnapshot` (+ тестовые данные, `ITestDataInitializer`). Роли строк подключения: `TestConnectionStringRole.EmptySnapshot/FilledSnapshot`, `PoolTestConnectionStringRole(index)`.
2. **Перед каждым тестом** hook `PrepareDatabaseEnvironmentHook` копирует `FilledSnapshot` в личную БД провайдера из пула (`DatabaseSnapshotManager.RestoreDatabaseSnapshot` → `IDatabaseManager.Copy`).
3. **После теста** hook `CleanDatabaseEnvironmentHook` удаляет эту копию (`RemoveRestoredDatabase`). Следующий тест получит свежую копию, поэтому тесты могут свободно менять данные.
4. Параллельность: `AllowParallelizationConstraint`. Если хоть одно ограничение `false`, тесты идут строго по одному (`ParallelizationSettings.AllowParallelization`).

Файл `src/__RootSolutionItems/DatabaseInitModeHelper.cs` статический хелпер режима инициализации БД для интеграционных тестов (сейчас в Debug и Release возвращает `RebuildSnapshot`). `XunitParallelSettings.cs` закомментированная настройка параллелизма xunit.

## Образцы окружений

| Что показывает | Файл |
|---|---|
| Минимальное окружение | `Testing/Anch.Testing.Tests/TestEnvironment.cs` |
| In-memory + хуки | `HierarchicalExpand/Anch.HierarchicalExpand.Tests/Environment/TestEnvironment.cs` |
| БД + EF Core | `GenericQueryable/Anch.GenericQueryable.IntegrationTests.EntityFramework/Environment/*` |
| БД + NHibernate | `GenericQueryable/Anch.GenericQueryable.IntegrationTests.NHibernate/Environment/*` |
| Полный стек SecuritySystem | `SecuritySystem/_Example/ExampleApp.IntegrationTests*/Environment/*` |

## Модуль-специфичные тест-хелперы

`Anch.SecuritySystem.Testing` (см. [security-system.md](security-system.md)): `RootAuthManager`, `RootImpersonateService`, `TestingEvaluator` для выполнения кода от имени тестового пользователя/с определёнными правами.
