# SecuritySystem

Папка `src/SecuritySystem/`. Самый большой модуль (около 450 .cs-файлов + Angular-UI). Система прав доступа на основе ролей и **контекстов безопасности** (security context): «пользователь X имеет роль R на подразделения A, B (и их дочерние)».

Если нужно быстро оценить, как оно используется целиком, читайте [`_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`](../../src/SecuritySystem/_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs) (метод `AddSecuritySystem`): там настроено почти всё.

## 1. Основные понятия

| Понятие | Тип | Смысл |
|---|---|---|
| Контекст безопасности | `ISecurityContext` (маркер-интерфейс, `Anch.SecuritySystem.Domain`) | Доменный тип, по которому ограничиваются права: `BusinessUnit`, `Location`, … Регистрируется `AddSecurityContext<T>(guid, ...)` |
| Роль | `SecurityRole(name)` | Именованная роль (`Administrator`, `SystemIntegration` встроены). Регистрируется `AddSecurityRole(role, SecurityRoleInfo)` |
| Операция | `SecurityOperation(name)` | Именованная операция, которая может «входить» в роли (`SecurityRoleInfo.Operations`) |
| Правило | `SecurityRule` / `DomainSecurityRule` (records) | **Описание требуемого доступа**: роль, набор ролей, операция, `View`/`Edit` для типа, текущий пользователь, And/Or/Negate и т.д. |
| Путь безопасности | `SecurityPath<TDomain>` | Как от доменного объекта дойти до его контекстов безопасности: `x => x.BusinessUnit`, … |
| Разрешение (permission) | зависит от источника | Выдача роли принципалу с ограничениями по контекстам. Источники: GeneralPermission (таблицы БД) и VirtualPermission (вычисляется из доменных данных) |
| Provider | `ISecurityProvider<T>` | Проверка/фильтрация доступа к объектам типа `T` |
| Принципал/пользователь | `User`, `UserCredential`, `IUserSource<TUser>` | Кто обращается |

Идентификаторы: `SecurityIdentity` / `TypedSecurityIdentity<T>`; для ролей/контекстов обычно `Guid` (в примере задаются явно).

## 2. Проекты

| Проект | Содержимое |
|---|---|
| `Anch.SecuritySystem.Domain` | Только `ISecurityContext` |
| `Anch.SecuritySystem.Rules` | **Модель правил**: `SecurityRule`, `DomainSecurityRule` (+ вложенные records), `SecurityRole`, `SecurityOperation`, `SecurityPathRestriction`, `SecurityContextRestriction`, `SecurityRuleCredential`, `User`, `UserCredential`, `SecurityIdentity`, расширения `Or/And/Except/Negate/ToSecurityRule` |
| `Anch.SecuritySystem.Abstractions` | Все публичные контракты: `ISecuritySystem`, `ISecurityProvider<T>`, `IDomainSecurityService<T>`, источники ролей/операций/контекстов, `IPrincipalManagementService`, `ISecurityContextStorage`, `IUserSource`, `IRunAsManager`, `IImpersonateService`, `IAccessDeniedExceptionService`, атрибуты `[ViewSecurity]`/`[EditSecurity]`, `AccessDeniedException`, валидаторы |
| `Anch.SecuritySystem.Runtime` | **Основная реализация** (~170 файлов). Подпапки: `Builders` (построение фильтров), `DomainServices`, `Expanders`, `ExternalSystem` (абстракции источника прав + хранилища контекстов + управление принципалами), `PermissionOptimization`, `Providers`, `SecurityAccessor`, `SecurityPath`, `SecurityRuleInfo`, `Services` (46 файлов), `UserSource` |
| `Anch.SecuritySystem` | **Точка входа DI**: `AddSecuritySystem(Action<ISecuritySystemSetup>)`, все `I*Setup` и их реализации |
| `Anch.SecuritySystem.GeneralPermission.Abstractions/.Runtime` + `Anch.SecuritySystem.GeneralPermission` | Источник прав «общего вида»: Principal/Permission/Role/Restriction как сущности в БД приложения. `AddGeneralPermission(...)` |
| `Anch.SecuritySystem.VirtualPermission.Runtime` + `Anch.SecuritySystem.VirtualPermission` | Источник прав, **вычисляемый из доменных объектов** приложения (например, «руководитель отдела»). `AddVirtualPermission<TPrincipal,TPermission>(...)` |
| `Anch.SecuritySystem.Notification.Abstractions/.Domain/.Runtime` + `Anch.SecuritySystem.Notification` | Определение получателей уведомлений по правилам (`NotificationFilterGroup`, `NotificationExpandType`, `INotificationPermissionExtractor`…). `AddNotification()` |
| `Anch.SecuritySystem.Configurator` | ASP.NET-админка прав: REST-обработчики (`Handlers/*`, `Interfaces/*`, `Models/*DTO`) + встроенный Angular-UI (`configurator-ui/`). `AddConfigurator()`, `UseConfigurator(route = "/admin/configurator")` |
| `Anch.SecuritySystem.Testing` | Хелперы для тестов: `RootAuthManager`, `RootImpersonateService`, `TestingEvaluator`, `TestRootUserInfo`, DI: `AddSecuritySystemTesting(setup?)` (`ISecuritySystemTestingSetup.SetEvaluator`, `SetTestRootUserInfo`) |
| `Anch.SecuritySystem.DiTests` | Юнит-тесты на in-memory окружении (`MainTests`, `SecurityPathTests`, `SecurityRoleTests`, оптимизаторы) |
| `_Example/` | Демо-приложение + интеграционные тесты EF/NHibernate (см. §7) |

## 3. Правила (`DomainSecurityRule`)

Правило это данные. Чтобы получить доступ, правило раскрывается (expand) до набора ролей и проверяется. Типы (в `Anch.SecuritySystem.Rules/DomainSecurityRule.cs`):

- **Роль/операция**: `NonExpandedRolesSecurityRule` (набор ролей, создаётся через `role.ToSecurityRule()` или неявно из `SecurityRole`/`SecurityRole[]`), `OperationSecurityRule`, `NonExpandedRoleGroupSecurityRule`, `ExpandedRolesSecurityRule`, `ExpandedRoleGroupSecurityRule` (после раскрытия), `AnyRoleSecurityRule` (`DomainSecurityRule.AnyRole`), `RoleFactorySecurityRule`.
  Все наследуют `RoleBaseSecurityRule` с кастомизацией: `CustomExpandType` (`HierarchicalExpandType`, по умолчанию `Children`), `CustomRestriction` (`SecurityPathRestriction`), `CustomCredential`.
- **Режим**: `SecurityRule.View`, `SecurityRule.Edit` (`ModeSecurityRule`) и `DomainModeSecurityRule(DomainType, Mode)`: «режим View для типа X». Реализация для типа задаётся `AddDomainSecurity<T>(b => b.SetView(...).SetEdit(...))`.
- **Клиентские правила**: `ClientSecurityRule(name)`, `SecurityRuleHeader(name)`, соответствие «имя → реализация» (`AddSecurityRule(header, impl)`, `IClientSecurityRuleInfoSource`).
- **Текущий пользователь**: `DomainSecurityRule.CurrentUser` (объект доступен, если он «принадлежит» пользователю; `RelativePathKey` задаёт путь).
- **Провайдеры/условия**: `ProviderSecurityRule`, `ProviderFactorySecurityRule`, `ConditionFactorySecurityRule`, `RelativeConditionSecurityRule` (условие по связанному объекту), `FactorySecurityRule`; `DomainSecurityRule.AccessDenied` и `SecurityRule.Disabled` специальные провайдеры.
- **Комбинаторы** (`SecurityRuleExtensions`): `.Or(...)`, `.And(...)`, `.Except(...)`, `.Negate()`, а также `.Or<T>(x => cond)` / `.And<T>(...)`.
- `OverrideAccessDeniedMessageSecurityRule`: подмена текста отказа.

Примеры:
```csharp
SecurityRole.Administrator                                     // неявно → rule
new[] { Role.TestManager, Role.BuManager }                     // любая из ролей
Role.TestManager.ToSecurityRule(HierarchicalExpandType.All)    // с раскрытием иерархии контекстов
Role.A.Or(Role.B).And<TestObject>(o => o.Active)
DomainSecurityRule.CurrentUser
```

**Раскрытие**: `ISecurityRuleExpander` (реализация `RootSecurityRuleExpander` в `Runtime/Expanders`) последовательно раскрывает Mode → Header → Client → Operation → RoleGroup → RoleFactory и т.д. `IDomainSecurityRoleExtractor.ExtractSecurityRule(rule)` даёт итоговую `ExpandedRoleGroupSecurityRule`. Оптимизаторы правил: `SecurityRuleBasicOptimizer`, `SecurityRuleDeepOptimizer`, `ExpandedRoleGroupSecurityRuleSetOptimizer`.

## 4. Как проверяется доступ (два режима работы)

### 4.1. Без доменного объекта: `ISecuritySystem`
```csharp
bool ok = await securitySystem.HasAccessAsync(SecurityRole.Administrator, ct);
await securitySystem.CheckAccessAsync(rule, ct);   // бросает исключение из IAccessDeniedExceptionService
```
`Runtime/SecuritySystem/SecuritySystem.cs`: правило раскрывается → у каждой `IPermissionSystem` берутся `IPermissionSource`, и достаточно, чтобы любой из них вернул `HasAccessAsync`.

### 4.2. Для доменных объектов: `IDomainSecurityService<T>` → `ISecurityProvider<T>`
```csharp
ISecurityProvider<TestObject> provider = domainSecurityService.GetSecurityProvider(SecurityRule.View);
IQueryable<TestObject> visible = provider.Inject(db.Query<TestObject>());      // фильтрация запроса в SQL
bool has = await provider.HasAccessAsync(obj, ct);
await provider.CheckAccessAsync(obj, accessDeniedExceptionService, ct);        // см. Example/Repository.cs
```
`ISecurityProvider<T>` расширяет `IQueryableInjector<T>` (`Inject` добавляет `Where`), а проверка одного объекта по умолчанию сводится к `Inject(...).GenericContainsAsync(obj)` (используется [GenericQueryable](generic-queryable.md)). `GetAccessorDataAsync(obj)` возвращает, кто имеет доступ (`SecurityAccessorData`).

Фильтр строится по `SecurityPath<T>` (какие контексты есть у объекта) и по разрешениям пользователя: `RoleBaseSecurityPathProvider<T>` (`Runtime/Providers`) через `ISecurityFilterFactory<T>` / `IAccessorsFilterFactory<T>` (`Runtime/Builders`).

Папка `Runtime/Builders`: три семейства построителей фильтра: `QueryBuilder` (условия строятся выражениями для SQL), `MaterializedBuilder` (разрешения подгружаются в память), `MixedBuilder/SecurityExpressionBuilderFactory` (выбор между ними), плюс `AccessorsBuilder` (вычисление «кто имеет доступ»). Классы названы по узлам дерева условий: `And/Or/Condition/SingleContext/ManyContext/NestedMany/ByIdents`. (Назначение семейств выведено из имён и структуры, глубоко код не изучался.)

`DomainSecurityService`: `Runtime/DomainServices` (`DomainSecurityService`, `VisitedDomainSecurityService`, `DependencyDomainSecurityService` для зависимых типов через `SetDependency`).

### 4.3. Атрибуты в DI
`[ViewSecurity]`, `[EditSecurity]`, `[DisabledSecurity]` на параметре конструктора: внедряют `ISecurityProvider<T>` нужного режима как keyed-сервис (`FromKeyedServices(nameof(SecurityRule.View))`).

## 5. Настройка (`ISecuritySystemSetup`)

`Anch.SecuritySystem/DependencyInjection/ISecuritySystemSetup.cs`. Основные методы:

| Метод | Назначение |
|---|---|
| `AddSecurityContext<T>(guid, ctx => ...)` | Зарегистрировать контекст. `ISecurityContextInfoSetup`: `SetName`, `SetDisplayFunc`, `SetIdentityPath`, `SetHierarchicalInfo(...)` (см. [hierarchical-expand.md](hierarchical-expand.md)) |
| `AddSecurityRole(role, SecurityRoleInfo)` | Роль: `Identity`, `Restriction` (`SecurityPathRestriction`), `Operations`, `Children`, `Description`, `IsVirtual`, `CustomExpandType` |
| `AddSecurityOperation(op, info)` | Операция |
| `AddSecurityRule(header, impl)` | Именованное правило |
| `AddDomainSecurity<T>(b => ...)` | Правила доступа к типу: `SetView`, `SetEdit`, `SetMode`, `SetPath(SecurityPath<T>)`, `SetDependency<TSource>[(path)]`, `SetCustomService<T>()` |
| `AddUserSource<TUser>(u => ...)` | Откуда брать пользователей: `SetName`, `SetFilter`, `SetRunAs`, `SetMissedService` |
| `AddPermissionSystem(...)` | Свой `IPermissionSystemFactory` (кроме встроенных General/Virtual) |
| `AddPrincipalManagementService/Listener` | Управление принципалами (CRUD) и подписка на изменения |
| `SetQueryableSource<T>()`, `SetGenericRepository<T>()` | Доступ к данным (`Anch.GenericRepository`, см. [core.md](core.md)) |
| `SetRawCurrentUser<T>()`, `SetDefaultCurrentUser<T>()`, `SetDefaultCancellationTokenSource<T>()` | Кто текущий пользователь (`ICurrentUser`) и токен по умолчанию |
| `AddRunAsValidator<T>()` | Проверка «запустить от имени» (RunAs) |
| `SetAccessDeniedExceptionService<T>()` | Тип исключения при отказе |
| `SetSecurityAdministratorRule(rule)` | Правило «администратор безопасности» |
| `SetSecurityAccessorInfinityStorage<T>()` | Хранилище «бесконечных» accessor'ов |
| `AddClientSecurityRuleInfoSource<T>()`, `SetClientDomainModeSecurityRuleSource<T>()` | Клиентские правила (для UI) |
| `AddExtensions(services => ...)` | Произвольные регистрации |
| `AutoAddSelfRelativePath`, `InitializeDefaultRoles` | Флаги |

Расширения поверх `ISecuritySystemSetup`: `AddGeneralPermission(...)`, `AddVirtualPermission(...)`, `AddNotification()`.

## 6. Источники прав

### GeneralPermission
Сущности приложения (в примере `ExampleApp.Domain/Auth/General`): `Principal`, `Permission` (ссылки на Principal и Role, период действия, комментарий, делегирование), `SecurityRole`, `PermissionRestriction` (тип контекста + Id объекта), `SecurityContextType`. Связывание через `PropertyAccessors`/лямбды:

```csharp
.AddGeneralPermission(
    p => p.Principal, p => p.SecurityRole,
    (PermissionRestriction pr) => pr.Permission, pr => pr.SecurityContextType, pr => pr.SecurityContextId,
    b => b.SetSecurityRoleDescription(sr => sr.Description)
          .SetPermissionPeriod(p => p.StartDate, p => p.EndDate)
          .SetPermissionComment(p => p.Comment)
          .SetPermissionDelegation(p => p.DelegatedFrom)
          .SetReadonly()                                   // запретить управление
          .SetCustomPermissionManagementService<MyService>())
```
`GeneralPermission.Runtime`: управление (`IPermissionManagementService`), валидаторы (`Validation/Permission`, `Validation/Principal`), сравнение разрешений.

### VirtualPermission
Права вычисляются из обычных доменных сущностей: «Employee в роли TestManager на BusinessUnit/Location из объекта `TestManager`».
```csharp
.AddVirtualPermission<Employee, TestManager>(
    x => x.Employee,
    b => b.AddRestriction(x => x.BusinessUnit)
          .AddRestriction(x => x.Location)
          .AddSecurityRole(Role.TestManager))
```
Роль с `IsVirtual = true` в `SecurityRoleInfo`. Реализация: `VirtualPermission.Runtime` (`VirtualPermissionSystem`, `VirtualPermissionSource`, `VirtualPrincipalSourceService`).

### Общая абстракция
`IPermissionSystem` (+ `IPermissionSystem<TPermission>`) → `IPermissionSource` (`HasAccessAsync`, `GetPermissionsAsync`, `GetPermissionQuery`). Файлы: `Runtime/ExternalSystem/*`. Создаёт `IPermissionSystemFactory.Create(SecurityRuleCredential)`.

## 7. Пользователи, RunAs, Impersonate

- `ICurrentUser` (`Anch.Core.Auth`): три keyed-варианта: `Raw` (из окружения), `Impersonated`, `Default`. Реализации и источники: `Runtime/CurrentUser.cs`, `ImpersonatedCurrentUser.cs`, `UserSource/*`.
- `IUserSource<TUser>`: поиск пользователя по `UserCredential` (имя или Id).
- **RunAs**: `IRunAsManager.StartRunAsUserAsync/FinishRunAsUserAsync`, проверка `IRunAsValidator` (`ExistsOtherwiseUsersRunAsValidator`).
- `IImpersonateService.WithImpersonateAsync(credential, func)`: выполнить код от имени другого пользователя.
- `SecurityRuleCredential`: `CurrentUserWithRunAs`, `CurrentUserWithoutRunAs`, `AnyUser`, `CustomUser(credential)` (в правиле можно переопределить, чьи права проверять).

## 8. Конфигуратор (админка прав)

`Anch.SecuritySystem.Configurator` (C# + Angular):
- Подключение: `services.AddConfigurator()`, `app.UseConfigurator("/admin/configurator")` (см. `_Example/ExampleApp.Api/Program.cs`).
- Бэкенд: minimal-API обработчики `Handlers/*` (интерфейсы в `Interfaces/*`): принципалы (создание, изменение, удаление), разрешения, бизнес-роли, операции, контексты, модули, RunAs. Расширяется через `IConfiguratorModule` (`AddModule`).
- Фронтенд: `configurator-ui/` (Angular, Angular Material, Tailwind). В UI есть раздел событий (`app/events`), но отдельного C#-обработчика «events» в `Handlers/` нет, он, вероятно, поставляется внешним модулем. Собирается `npm ci && npm run ng build` (так делает CI) и встраивается в сборку как embedded-ресурсы пространства `…configurator_ui.dist`.
- Если меняете DTO (`Models/*Dto.cs`), синхронизируйте `configurator-ui/src/app/**/models` и `api-services`.

## 9. Пример и тесты

`_Example/` (в решении лежит как часть SecuritySystem; в пакеты не должен попадать):

| Проект | Роль |
|---|---|
| `ExampleApp.Domain` | Домен: `BusinessUnit`, `ManagementUnit`, `Employee`, `Location`, `TestObject`, связи предков (`*DirectAncestorLink`, `*UndirectAncestorLink`), `Auth/General/*`, `Auth/Virtual/*` |
| `ExampleApp.Application` | Роли (`ExampleSecurityRole`), `IRepository` |
| `ExampleApp.Infrastructure` | **Главное место для чтения**: `AddInfrastructure` → `AddSecuritySystem(...)` (полная конфигурация), `Repository<T>` с проверкой `ISecurityProvider` |
| `ExampleApp.Infrastructure.EntityFramework` / `.NHibernate` | Реализации под ORM (DbContext/NHibernate-маппинги) |
| `ExampleApp.Api`, `.Api.EntityFramework`, `.Api.NHibernate` | ASP.NET host, `Program.cs` (`AddInfrastructure().AddConfigurator()`, Negotiate-аутентификация, Swagger) |
| `ExampleApp.IntegrationTests` (+ `.EntityFramework`, `.NHibernate`) | Сценарные тесты на SQLite: `GeneralPermissionTests`, `VirtualPermissionTests`, `RestrictionFilterTests`, `RunAsTests`, `ImpersonateServiceTests`, `PermissionDelegatedFromTests`, `NotificationTests`, `ClientSecurityRuleTests`, `CustomCredentialTests`, `ConfiguratorApiTests`, … Лучший источник примеров «как вызвать» |

`Anch.SecuritySystem.DiTests`: проверка без БД (`TestPermissionSystem`, `TestQueryableSource` как in-memory источники).

## 10. Где что менять

| Задача | Место |
|---|---|
| Новый вид правила | `Rules/DomainSecurityRule.cs` + раскрытие в `Runtime/Expanders/*` + обработка в `Runtime/Services/SecurityRuleVisitor.cs`, оптимизаторах |
| Новая настройка в `AddSecuritySystem` | `Anch.SecuritySystem/DependencyInjection/ISecuritySystemSetup.cs` + `SecuritySystemSetup.cs` |
| Как строится SQL-фильтр доступа | `Runtime/Builders/**`, `Runtime/Providers/RoleBaseSecurityPathProvider.cs` |
| Новый источник прав | реализовать `IPermissionSystemFactory`/`IPermissionSystem`/`IPermissionSource`, подключить `AddPermissionSystem` (образцы: GeneralPermission, VirtualPermission) |
| Пути безопасности (`SecurityPath`) | `Runtime/SecurityPath/*` |
| Управление принципалами | `Abstractions/ExternalSystem/Management/*`, `Runtime/ExternalSystem/Management/*` |
| Исключение при отказе доступа | `Abstractions/AccessDenied/*`, `SetAccessDeniedExceptionService<T>()` |
| Тестовый пользователь/права | `Anch.SecuritySystem.Testing` |
