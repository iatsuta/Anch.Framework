# Шпаргалка: «что где искать»

Быстрый указатель для навигации по коду. Все пути относительно `src/`. Слои проектов (`Abstractions`/`Runtime`/…) объяснены в [architecture.md](architecture.md).

> `src/Workflow/**` пока не изучать, см. [modules/workflow.md](modules/workflow.md).

## По задаче

| Хочу… | Смотреть |
|---|---|
| Понять, как подключается любой модуль к DI | `DependencyInjection/Anch.DependencyInjection/ServiceCollectionExtensions.cs`, затем `<Модуль>/Anch.<Модуль>/DependencyInjection/*Setup.cs` |
| Найти интерфейс (контракт) | `*/Anch.*.Abstractions/` (у Core: `Core/Anch.Core/*.cs`) |
| Найти реализацию интерфейса `IFoo` | `*/Anch.*.Runtime/` (поиск `class .* : IFoo`) |
| Понять, как ограничить доступ к данным по ролям | [modules/security-system.md](modules/security-system.md), `SecuritySystem/_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` |
| Добавить новую роль/правило/контекст безопасности | `ISecuritySystemSetup` (`SecuritySystem/Anch.SecuritySystem/DependencyInjection/`), правила: `SecuritySystem/Anch.SecuritySystem.Rules/` |
| Как строится SQL-фильтр доступа | `SecuritySystem/Anch.SecuritySystem.Runtime/Builders/`, `Providers/RoleBaseSecurityPathProvider.cs` |
| Права, хранящиеся в БД приложения | `SecuritySystem/Anch.SecuritySystem.GeneralPermission*` |
| Права, вычисляемые из доменных данных | `SecuritySystem/Anch.SecuritySystem.VirtualPermission*` |
| Админка прав (REST + Angular) | `SecuritySystem/Anch.SecuritySystem.Configurator/` (`Handlers/`, `configurator-ui/`) |
| Раскрыть иерархию «родитель/дети» в запросе | `HierarchicalExpand/Anch.HierarchicalExpand.Runtime/HierarchicalObjectAncestorLinkExpander.cs` |
| Async LINQ без привязки к ORM (`GenericToListAsync`) | `GenericQueryable/Anch.GenericQueryable.Abstractions/GenericQueryableExtensions.cs` |
| Eager loading (`WithFetch`) | `GenericQueryable/Anch.GenericQueryable.Abstractions/Fetching/` |
| Подключить GenericQueryable к EF / NHibernate | `GenericQueryable/Anch.GenericQueryable.EntityFramework/DbContextOptionsBuilderExtensions.cs` / `…NHibernate/ServiceCollectionExtensions.cs`, `ConfigurationExtensions.cs` |
| Разобрать OData-строку (`$filter=…`) | `OData/Anch.OData.Runtime/` (грамматика: `InternalParser/`) |
| Достать Id/имя у произвольного доменного типа | `IdentitySource/`, `VisualIdentitySource/` |
| Работа с Expression-деревьями (сравнение, компиляция, визиторы) | `Core/Anch.Core/ExpressionComparers/`, `ExpressionEvaluate/`, `Visitor/`, `ExpressionExtensions.cs`, `ExpressionHelper.cs` |
| `PropertyAccessors`, `LambdaExpressionPath`, `Maybe<T>`, `MergeResult` | `Core/Anch.Core/` (одноимённые файлы) |
| Асинхронные блокировки, `AsyncOnce` | `Core/Anch.Threading/` |
| Кэш | `Caching/` |
| Валидация регистраций DI, `DuplicateServiceUsageValidator` | `DependencyInjection/Anch.DependencyInjection/` |
| Написать интеграционный тест с БД | [modules/testing.md](modules/testing.md); образцы: `GenericQueryable/Anch.GenericQueryable.IntegrationTests.EntityFramework/Environment/` |
| Тест SecuritySystem без БД | `SecuritySystem/Anch.SecuritySystem.DiTests/` |
| Как запускать/добавлять тесты, CI, публикация | [building-and-testing.md](building-and-testing.md) |
| Центральные версии пакетов / общие свойства сборки | `Directory.Packages.props`, `Directory.Build.props` |

## По имени типа (частые)

| Тип | Файл/проект |
|---|---|
| `ISecuritySystem` | `SecuritySystem/Anch.SecuritySystem.Abstractions/SecuritySystem/` |
| `ISecurityProvider<T>` | `SecuritySystem/Anch.SecuritySystem.Abstractions/Providers/` |
| `IDomainSecurityService<T>` | `SecuritySystem/Anch.SecuritySystem.Abstractions/DomainServices/` |
| `DomainSecurityRule`, `SecurityRule`, `SecurityRole`, `SecurityPathRestriction` | `SecuritySystem/Anch.SecuritySystem.Rules/` |
| `SecurityPath<T>` | `SecuritySystem/Anch.SecuritySystem.Runtime/SecurityPath/SecurityPath.cs` |
| `SecuritySystemSetup` | `SecuritySystem/Anch.SecuritySystem/DependencyInjection/SecuritySystemSetup.cs` |
| `IPermissionSystem` / `IPermissionSource` | `SecuritySystem/Anch.SecuritySystem.Runtime/ExternalSystem/` |
| `IPrincipalManagementService` | `SecuritySystem/Anch.SecuritySystem.Abstractions/ExternalSystem/Management/` |
| `IHierarchicalObjectExpander<T>` | `HierarchicalExpand/Anch.HierarchicalExpand.Abstractions/` |
| `HierarchicalInfo<T>`, `AncestorLinkInfo<,>` | `HierarchicalExpand/Anch.HierarchicalExpand.Abstractions/` |
| `FetchRule<T>`, `IFetchService` | `GenericQueryable/Anch.GenericQueryable.Abstractions/Fetching/` |
| `IGenericQueryableExecutor` | `GenericQueryable/Anch.GenericQueryable.Abstractions/Services/` |
| `SelectOperation<T>` | `OData/Anch.OData.Domain/SelectOperation.cs` |
| `ISelectOperationParser` | `OData/Anch.OData.Abstractions/` |
| `IIdentityInfoSource` | `IdentitySource/Anch.IdentitySource.Abstractions/` |
| `IQueryableSource`, `IGenericRepository` | `Core/Anch.GenericRepository/` |
| `IRelativeDomainPathInfo<,>` | `Core/Anch.RelativePath/` |
| `IServiceProxyFactory` | интерфейс в `Core/Anch.Core/`, реализация в `DependencyInjection/Anch.DependencyInjection/ServiceProxy/` |
| `ITestEnvironment` | `Testing/Anch.Testing.Abstractions/` |
| `DatabaseTestEnvironment` | `Testing/Anch.Testing.Database/` |
| `ICurrentUser` | `Core/Anch.Core/Auth/` |
| `IInitializer` | `Core/Anch.Core/IInitializer.cs` |

## Приёмы поиска

- **Метод расширения** ищите по имени метода: в репозитории используются C# 14 `extension(T x) { … }`-блоки, а не `this T x`.
- **Где зарегистрирован сервис**: `Grep` по `AddScoped<IFoo` / `typeof(IFoo<>)` в `*Setup.cs`.
- **Образец использования API**: тесты (`*.Tests`, `ExampleApp.IntegrationTests`) почти всегда содержат минимальный рабочий пример.
- **Исключайте из поиска**: `bin/`, `obj/`, `graphify-out/`, `configurator-ui/node_modules`, `src/Workflow/**`.
- `graphify-out/GRAPH_REPORT.md`: автосгенерированная карта «сообществ» классов; полезна для общего обзора, но может быть неактуальной.
