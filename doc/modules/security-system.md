# SecuritySystem

Folder `src/SecuritySystem/`. The largest module (about 450 .cs files + an Angular UI). A permission system based on roles and **security contexts**: "user X has role R on departments A, B (and their children)".

To quickly see how it is used as a whole, read [`_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`](../../src/SecuritySystem/_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs) (the `AddSecuritySystem` method): almost everything is configured there.

## 1. Core concepts

| Concept | Type | Meaning |
|---|---|---|
| Security context | `ISecurityContext` (marker interface, `Anch.SecuritySystem.Domain`) | A domain type by which permissions are restricted: `BusinessUnit`, `Location`, … Registered with `AddSecurityContext<T>(guid, ...)` |
| Role | `SecurityRole(name)` | A named role (`Administrator`, `SystemIntegration` are built in). Registered with `AddSecurityRole(role, SecurityRoleInfo)` |
| Operation | `SecurityOperation(name)` | A named operation that can be "included" in roles (`SecurityRoleInfo.Operations`) |
| Rule | `SecurityRule` / `DomainSecurityRule` (records) | **A description of the required access**: a role, a set of roles, an operation, `View`/`Edit` for a type, the current user, And/Or/Negate, etc. |
| Security path | `SecurityPath<TDomain>` | How to get from a domain object to its security contexts: `x => x.BusinessUnit`, … |
| Permission | depends on the source | Granting a role to a principal with restrictions by contexts. Sources: GeneralPermission (database tables) and VirtualPermission (computed from domain data) |
| Provider | `ISecurityProvider<T>` | Checks/filters access to objects of type `T` |
| Principal/user | `User`, `UserCredential`, `IUserSource<TUser>` | Who is making the request |

Identifiers: `SecurityIdentity` / `TypedSecurityIdentity<T>`; for roles/contexts usually `Guid` (set explicitly in the example).

## 2. Projects

| Project | Contents |
|---|---|
| `Anch.SecuritySystem.Domain` | Only `ISecurityContext` |
| `Anch.SecuritySystem.Rules` | **The rule model**: `SecurityRule`, `DomainSecurityRule` (+ nested records), `SecurityRole`, `SecurityOperation`, `SecurityPathRestriction`, `SecurityContextRestriction`, `SecurityRuleCredential`, `User`, `UserCredential`, `SecurityIdentity`, extensions `Or/And/Except/Negate/ToSecurityRule` |
| `Anch.SecuritySystem.Abstractions` | All public contracts: `ISecuritySystem`, `ISecurityProvider<T>`, `IDomainSecurityService<T>`, role/operation/context sources, `IPrincipalManagementService`, `ISecurityContextStorage`, `IUserSource`, `IRunAsManager`, `IImpersonateService`, `IAccessDeniedExceptionService`, attributes `[ViewSecurity]`/`[EditSecurity]`, `AccessDeniedException`, validators |
| `Anch.SecuritySystem.Runtime` | **The main implementation** (~170 files). Subfolders: `Builders` (filter construction), `DomainServices`, `Expanders`, `ExternalSystem` (permission source abstractions + context storage + principal management), `PermissionOptimization`, `Providers`, `SecurityAccessor`, `SecurityPath`, `SecurityRuleInfo`, `Services` (46 files), `UserSource` |
| `Anch.SecuritySystem` | **DI entry point**: `AddSecuritySystem(Action<ISecuritySystemSetup>)`, all `I*Setup` and their implementations |
| `Anch.SecuritySystem.GeneralPermission.Abstractions/.Runtime` + `Anch.SecuritySystem.GeneralPermission` | A "general" permission source: Principal/Permission/Role/Restriction as entities in the application database. `AddGeneralPermission(...)` |
| `Anch.SecuritySystem.VirtualPermission.Runtime` + `Anch.SecuritySystem.VirtualPermission` | A permission source **computed from the application's domain objects** (for example, "department head"). `AddVirtualPermission<TPrincipal,TPermission>(...)` |
| `Anch.SecuritySystem.Notification.Abstractions/.Domain/.Runtime` + `Anch.SecuritySystem.Notification` | Determining notification recipients by rules (`NotificationFilterGroup`, `NotificationExpandType`, `INotificationPermissionExtractor`…). `AddNotification()` |
| `Anch.SecuritySystem.Configurator` | ASP.NET permission admin: REST handlers (`Handlers/*`, `Interfaces/*`, `Models/*DTO`) + embedded Angular UI (`configurator-ui/`). `AddConfigurator()`, `UseConfigurator(route = "/admin/configurator")` |
| `Anch.SecuritySystem.Testing` | Test helpers: `RootAuthManager`, `RootImpersonateService`, `TestingEvaluator`, `TestRootUserInfo`, DI: `AddSecuritySystemTesting(setup?)` (`ISecuritySystemTestingSetup.SetEvaluator`, `SetTestRootUserInfo`) |
| `Anch.SecuritySystem.DiTests` | Unit tests on an in-memory environment (`MainTests`, `SecurityPathTests`, `SecurityRoleTests`, optimizers) |
| `_Example/` | Demo application + EF/NHibernate integration tests (see §9) |

## 3. Rules (`DomainSecurityRule`)

A rule is data. To grant access, the rule is expanded into a set of roles and checked. Types (in `Anch.SecuritySystem.Rules/DomainSecurityRule.cs`):

- **Role/operation**: `NonExpandedRolesSecurityRule` (a set of roles, created via `role.ToSecurityRule()` or implicitly from `SecurityRole`/`SecurityRole[]`), `OperationSecurityRule`, `NonExpandedRoleGroupSecurityRule`, `ExpandedRolesSecurityRule`, `ExpandedRoleGroupSecurityRule` (after expansion), `AnyRoleSecurityRule` (`DomainSecurityRule.AnyRole`), `RoleFactorySecurityRule`.
  All inherit `RoleBaseSecurityRule` with customization: `CustomExpandType` (`HierarchicalExpandType`, `Children` by default), `CustomRestriction` (`SecurityPathRestriction`), `CustomCredential`.
- **Mode**: `SecurityRule.View`, `SecurityRule.Edit` (`ModeSecurityRule`) and `DomainModeSecurityRule(DomainType, Mode)`: "View mode for type X". The implementation for a type is set with `AddDomainSecurity<T>(b => b.SetView(...).SetEdit(...))`.
- **Client rules**: `ClientSecurityRule(name)`, `SecurityRuleHeader(name)`, a "name → implementation" mapping (`AddSecurityRule(header, impl)`, `IClientSecurityRuleInfoSource`).
- **Current user**: `DomainSecurityRule.CurrentUser` (an object is accessible if it "belongs" to the user; `RelativePathKey` sets the path).
- **Providers/conditions**: `ProviderSecurityRule`, `ProviderFactorySecurityRule`, `ConditionFactorySecurityRule`, `RelativeConditionSecurityRule` (a condition on a related object), `FactorySecurityRule`; `DomainSecurityRule.AccessDenied` and `SecurityRule.Disabled` are special providers.
- **Combinators** (`SecurityRuleExtensions`): `.Or(...)`, `.And(...)`, `.Except(...)`, `.Negate()`, and also `.Or<T>(x => cond)` / `.And<T>(...)`.
- `OverrideAccessDeniedMessageSecurityRule`: overrides the denial message.

Examples:
```csharp
SecurityRole.Administrator                                     // implicitly → rule
new[] { Role.TestManager, Role.BuManager }                     // any of the roles
Role.TestManager.ToSecurityRule(HierarchicalExpandType.All)    // with context hierarchy expansion
Role.A.Or(Role.B).And<TestObject>(o => o.Active)
DomainSecurityRule.CurrentUser
```

**Expansion**: `ISecurityRuleExpander` (implementation `RootSecurityRuleExpander` in `Runtime/Expanders`) successively expands Mode → Header → Client → Operation → RoleGroup → RoleFactory, etc. `IDomainSecurityRoleExtractor.ExtractSecurityRule(rule)` yields the final `ExpandedRoleGroupSecurityRule`. Rule optimizers: `SecurityRuleBasicOptimizer`, `SecurityRuleDeepOptimizer`, `ExpandedRoleGroupSecurityRuleSetOptimizer`.

## 4. How access is checked (two modes)

### 4.1. Without a domain object: `ISecuritySystem`
```csharp
bool ok = await securitySystem.HasAccessAsync(SecurityRole.Administrator, ct);
await securitySystem.CheckAccessAsync(rule, ct);   // throws the exception from IAccessDeniedExceptionService
```
`Runtime/SecuritySystem/SecuritySystem.cs`: the rule is expanded → the `IPermissionSource`s are taken from every `IPermissionSystem`, and it is enough for any of them to return `HasAccessAsync`.

### 4.2. For domain objects: `IDomainSecurityService<T>` → `ISecurityProvider<T>`
```csharp
ISecurityProvider<TestObject> provider = domainSecurityService.GetSecurityProvider(SecurityRule.View);
IQueryable<TestObject> visible = provider.Inject(db.Query<TestObject>());      // query filtering in SQL
bool has = await provider.HasAccessAsync(obj, ct);
await provider.CheckAccessAsync(obj, accessDeniedExceptionService, ct);        // see Example/Repository.cs
```
`ISecurityProvider<T>` extends `IQueryableInjector<T>` (`Inject` adds a `Where`), and checking a single object by default boils down to `Inject(...).GenericContainsAsync(obj)` (uses [GenericQueryable](generic-queryable.md)). `GetAccessorDataAsync(obj)` returns who has access (`SecurityAccessorData`).

The filter is built from the `SecurityPath<T>` (which contexts the object has) and the user's permissions: `RoleBaseSecurityPathProvider<T>` (`Runtime/Providers`) via `ISecurityFilterFactory<T>` / `IAccessorsFilterFactory<T>` (`Runtime/Builders`).

The `Runtime/Builders` folder: three families of filter builders: `QueryBuilder` (conditions are built as expressions for SQL), `MaterializedBuilder` (permissions are loaded into memory), `MixedBuilder/SecurityExpressionBuilderFactory` (choice between them), plus `AccessorsBuilder` (computing "who has access"). Classes are named after the nodes of the condition tree: `And/Or/Condition/SingleContext/ManyContext/NestedMany/ByIdents`. (The purpose of the families is inferred from names and structure; the code was not studied in depth.)

`DomainSecurityService`: `Runtime/DomainServices` (`DomainSecurityService`, `VisitedDomainSecurityService`, `DependencyDomainSecurityService` for dependent types via `SetDependency`).

### 4.3. Attributes in DI
`[ViewSecurity]`, `[EditSecurity]`, `[DisabledSecurity]` on a constructor parameter: inject an `ISecurityProvider<T>` of the required mode as a keyed service (`FromKeyedServices(nameof(SecurityRule.View))`).

## 5. Configuration (`ISecuritySystemSetup`)

`Anch.SecuritySystem/DependencyInjection/ISecuritySystemSetup.cs`. Main methods:

| Method | Purpose |
|---|---|
| `AddSecurityContext<T>(guid, ctx => ...)` | Register a context. `ISecurityContextInfoSetup`: `SetName`, `SetDisplayFunc`, `SetIdentityPath`, `SetHierarchicalInfo(...)` (see [hierarchical-expand.md](hierarchical-expand.md)) |
| `AddSecurityRole(role, SecurityRoleInfo)` | Role: `Identity`, `Restriction` (`SecurityPathRestriction`), `Operations`, `Children`, `Description`, `IsVirtual`, `CustomExpandType` |
| `AddSecurityOperation(op, info)` | Operation |
| `AddSecurityRule(header, impl)` | Named rule |
| `AddDomainSecurity<T>(b => ...)` | Access rules for a type: `SetView`, `SetEdit`, `SetMode`, `SetPath(SecurityPath<T>)`, `SetDependency<TSource>[(path)]`, `SetCustomService<T>()` |
| `AddUserSource<TUser>(u => ...)` | Where to get users from: `SetName`, `SetFilter`, `SetRunAs`, `SetMissedService` |
| `AddPermissionSystem(...)` | A custom `IPermissionSystemFactory` (besides the built-in General/Virtual) |
| `AddPrincipalManagementService/Listener` | Principal management (CRUD) and change subscription |
| `SetQueryableSource<T>()`, `SetGenericRepository<T>()` | Data access (`Anch.GenericRepository`, see [core.md](core.md)) |
| `SetRawCurrentUser<T>()`, `SetDefaultCurrentUser<T>()`, `SetDefaultCancellationTokenSource<T>()` | Who the current user is (`ICurrentUser`) and the default token |
| `AddRunAsValidator<T>()` | "Run as" (RunAs) validation |
| `SetAccessDeniedExceptionService<T>()` | Exception type on denial |
| `SetSecurityAdministratorRule(rule)` | The "security administrator" rule |
| `SetSecurityAccessorInfinityStorage<T>()` | Storage of "infinite" accessors |
| `AddClientSecurityRuleInfoSource<T>()`, `SetClientDomainModeSecurityRuleSource<T>()` | Client rules (for the UI) |
| `AddExtensions(services => ...)` | Arbitrary registrations |
| `AutoAddSelfRelativePath`, `InitializeDefaultRoles` | Flags |

Extensions on top of `ISecuritySystemSetup`: `AddGeneralPermission(...)`, `AddVirtualPermission(...)`, `AddNotification()`.

## 6. Permission sources

### GeneralPermission
Application entities (in the example `ExampleApp.Domain/Auth/General`): `Principal`, `Permission` (references to Principal and Role, validity period, comment, delegation), `SecurityRole`, `PermissionRestriction` (context type + object Id), `SecurityContextType`. Linked via `PropertyAccessors`/lambdas:

```csharp
.AddGeneralPermission(
    p => p.Principal, p => p.SecurityRole,
    (PermissionRestriction pr) => pr.Permission, pr => pr.SecurityContextType, pr => pr.SecurityContextId,
    b => b.SetSecurityRoleDescription(sr => sr.Description)
          .SetPermissionPeriod(p => p.StartDate, p => p.EndDate)
          .SetPermissionComment(p => p.Comment)
          .SetPermissionDelegation(p => p.DelegatedFrom)
          .SetReadonly()                                   // disable management
          .SetCustomPermissionManagementService<MyService>())
```
`GeneralPermission.Runtime`: management (`IPermissionManagementService`), validators (`Validation/Permission`, `Validation/Principal`), permission comparison.

### VirtualPermission
Permissions are computed from ordinary domain entities: "Employee in the TestManager role on the BusinessUnit/Location from a `TestManager` object".
```csharp
.AddVirtualPermission<Employee, TestManager>(
    x => x.Employee,
    b => b.AddRestriction(x => x.BusinessUnit)
          .AddRestriction(x => x.Location)
          .AddSecurityRole(Role.TestManager))
```
A role with `IsVirtual = true` in `SecurityRoleInfo`. Implementation: `VirtualPermission.Runtime` (`VirtualPermissionSystem`, `VirtualPermissionSource`, `VirtualPrincipalSourceService`).

### Common abstraction
`IPermissionSystem` (+ `IPermissionSystem<TPermission>`) → `IPermissionSource` (`HasAccessAsync`, `GetPermissionsAsync`, `GetPermissionQuery`). Files: `Runtime/ExternalSystem/*`. Created by `IPermissionSystemFactory.Create(SecurityRuleCredential)`.

## 7. Users, RunAs, Impersonate

- `ICurrentUser` (`Anch.Core.Auth`): three keyed variants: `Raw` (from the environment), `Impersonated`, `Default`. Implementations and sources: `Runtime/CurrentUser.cs`, `ImpersonatedCurrentUser.cs`, `UserSource/*`.
- `IUserSource<TUser>`: looks up a user by `UserCredential` (name or Id).
- **RunAs**: `IRunAsManager.StartRunAsUserAsync/FinishRunAsUserAsync`, validated by `IRunAsValidator` (`ExistsOtherwiseUsersRunAsValidator`).
- `IImpersonateService.WithImpersonateAsync(credential, func)`: runs code on behalf of another user.
- `SecurityRuleCredential`: `CurrentUserWithRunAs`, `CurrentUserWithoutRunAs`, `AnyUser`, `CustomUser(credential)` (a rule can override whose permissions are checked).

## 8. Configurator (permission admin)

`Anch.SecuritySystem.Configurator` (C# + Angular):
- Setup: `services.AddConfigurator()`, `app.UseConfigurator("/admin/configurator")` (see `_Example/ExampleApp.Api/Program.cs`).
- Backend: minimal-API handlers `Handlers/*` (interfaces in `Interfaces/*`): principals (create, update, delete), permissions, business roles, operations, contexts, modules, RunAs. Extended via `IConfiguratorModule` (`AddModule`).
- Frontend: `configurator-ui/` (Angular, Angular Material, Tailwind). The UI has an events section (`app/events`), but there is no separate C# "events" handler in `Handlers/`; it is probably supplied by an external module. Built with `npm ci && npm run ng build` (as CI does) and embedded into the assembly as embedded resources of the `…configurator_ui.dist` namespace.
- If you change DTOs (`Models/*Dto.cs`), synchronize `configurator-ui/src/app/**/models` and `api-services`.

## 9. Example and tests

`_Example/` (part of SecuritySystem in the solution; must not end up in packages):

| Project | Role |
|---|---|
| `ExampleApp.Domain` | Domain: `BusinessUnit`, `ManagementUnit`, `Employee`, `Location`, `TestObject`, ancestor links (`*DirectAncestorLink`, `*UndirectAncestorLink`), `Auth/General/*`, `Auth/Virtual/*` |
| `ExampleApp.Application` | Roles (`ExampleSecurityRole`), `IRepository` |
| `ExampleApp.Infrastructure` | **The main place to read**: `AddInfrastructure` → `AddSecuritySystem(...)` (full configuration), `Repository<T>` with an `ISecurityProvider` check |
| `ExampleApp.Infrastructure.EntityFramework` / `.NHibernate` | ORM implementations (DbContext/NHibernate mappings) |
| `ExampleApp.Api`, `.Api.EntityFramework`, `.Api.NHibernate` | ASP.NET host, `Program.cs` (`AddInfrastructure().AddConfigurator()`, Negotiate authentication, Swagger) |
| `ExampleApp.IntegrationTests` (+ `.EntityFramework`, `.NHibernate`) | Scenario tests on SQLite: `GeneralPermissionTests`, `VirtualPermissionTests`, `RestrictionFilterTests`, `RunAsTests`, `ImpersonateServiceTests`, `PermissionDelegatedFromTests`, `NotificationTests`, `ClientSecurityRuleTests`, `CustomCredentialTests`, `ConfiguratorApiTests`, … The best source of "how to call it" examples |

`Anch.SecuritySystem.DiTests`: checks without a database (`TestPermissionSystem`, `TestQueryableSource` as in-memory sources).

## 10. What to change where

| Task | Place |
|---|---|
| A new kind of rule | `Rules/DomainSecurityRule.cs` + expansion in `Runtime/Expanders/*` + handling in `Runtime/Services/SecurityRuleVisitor.cs`, the optimizers |
| A new setting in `AddSecuritySystem` | `Anch.SecuritySystem/DependencyInjection/ISecuritySystemSetup.cs` + `SecuritySystemSetup.cs` |
| How the SQL access filter is built | `Runtime/Builders/**`, `Runtime/Providers/RoleBaseSecurityPathProvider.cs` |
| A new permission source | implement `IPermissionSystemFactory`/`IPermissionSystem`/`IPermissionSource`, plug in via `AddPermissionSystem` (samples: GeneralPermission, VirtualPermission) |
| Security paths (`SecurityPath`) | `Runtime/SecurityPath/*` |
| Principal management | `Abstractions/ExternalSystem/Management/*`, `Runtime/ExternalSystem/Management/*` |
| Exception on access denial | `Abstractions/AccessDenied/*`, `SetAccessDeniedExceptionService<T>()` |
| Test user/permissions | `Anch.SecuritySystem.Testing` |
