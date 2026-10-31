using Anch.Core;
using Anch.Core.Auth;
using Anch.GenericRepository;
using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.DependencyInjection.Domain;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.SecurityAccessor;
using Anch.SecuritySystem.SecurityRuleInfo;
using Anch.SecuritySystem.UserSource;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

/// <summary>
/// Fluent configuration of the security system. Calls only collect settings; they are turned into container registrations when
/// <c>AddSecuritySystem</c> finishes. Permission sources (<c>AddGeneralPermission</c>, <c>AddVirtualPermission</c>) and
/// <c>AddNotification</c> extend this interface.
/// </summary>
/// <example>
/// <code>
/// services.AddSecuritySystem(b =&gt; b
///     .SetQueryableSource&lt;MyQueryableSource&gt;()
///     .AddSecurityContext&lt;BusinessUnit&gt;(buGuid)
///     .AddSecurityRole(MyRoles.Manager, new SecurityRoleInfo(roleGuid))
///     .AddDomainSecurity&lt;Order&gt;(MyRoles.Manager, SecurityPath&lt;Order&gt;.Create(o =&gt; o.BusinessUnit)));
/// </code>
/// </example>
public interface ISecuritySystemSetup
{
    /// <summary>
    /// Automatically adds relative paths from a type to itself (v => v).
    /// </summary>
    bool AutoAddSelfRelativePath { get; set; }

    /// <summary>When <c>true</c>, the built-in roles (administrator, system integration) are registered automatically.</summary>
    bool InitializeDefaultRoles { get; set; }

    /// <summary>Sets the rule that defines who is a security administrator.</summary>
    /// <param name="rule">The administrator rule.</param>
    ISecuritySystemSetup SetSecurityAdministratorRule(DomainSecurityRule.RoleBaseSecurityRule rule);

    /// <summary>Registers a security context type: a domain type that permissions can be limited by.</summary>
    /// <param name="identity">Stable identifier of the context type.</param>
    /// <param name="setup">Optional settings: name, display function, identity path, hierarchy.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    ISecuritySystemSetup AddSecurityContext<TSecurityContext>(TypedSecurityIdentity identity,
        Action<ISecurityContextInfoSetup<TSecurityContext>>? setup = null)
        where TSecurityContext : class, ISecurityContext;

    /// <summary>Configures the access rules of a domain type: the view and edit rules, the security path and customizations.</summary>
    /// <param name="setup">Configures the domain type.</param>
    /// <typeparam name="TDomainObject">The protected domain type.</typeparam>
    ISecuritySystemSetup AddDomainSecurity<TDomainObject>(Action<IDomainSecurityServiceSetup<TDomainObject>> setup);

    /// <summary>Configures a domain type with a view rule and a security path; editing is not configured.</summary>
    /// <param name="viewSecurityRule">The rule that grants viewing.</param>
    /// <param name="securityPath">How to reach the security contexts of an object.</param>
    /// <typeparam name="TDomainObject">The protected domain type.</typeparam>
    ISecuritySystemSetup AddDomainSecurity<TDomainObject>(DomainSecurityRule viewSecurityRule,
        SecurityPath<TDomainObject> securityPath)
    {
        return this.AddDomainSecurity(viewSecurityRule, null, securityPath);
    }

    /// <summary>Configures a domain type with view and edit rules and a security path.</summary>
    /// <param name="viewSecurityRule">The rule that grants viewing.</param>
    /// <param name="editSecurityRule">The rule that grants editing; <c>null</c> to leave it unconfigured.</param>
    /// <param name="securityPath">How to reach the security contexts of an object; <c>null</c> to leave it unconfigured.</param>
    /// <typeparam name="TDomainObject">The protected domain type.</typeparam>
    ISecuritySystemSetup AddDomainSecurity<TDomainObject>(DomainSecurityRule viewSecurityRule,
        DomainSecurityRule? editSecurityRule = null,
        SecurityPath<TDomainObject>? securityPath = null)
    {
        return this.AddDomainSecurity<TDomainObject>(b =>
        {
            b.SetView(viewSecurityRule);

            if (editSecurityRule is not null)
            {
                b.SetEdit(editSecurityRule);
            }

            if (securityPath is not null)
            {
                b.SetPath(securityPath);
            }
        });
    }

    /// <summary>Registers a role.</summary>
    /// <param name="securityRole">The role.</param>
    /// <param name="info">Identifier, restrictions, operations and child roles of the role.</param>
    ISecuritySystemSetup AddSecurityRole(SecurityRole securityRole, SecurityRoleInfo info);

    /// <summary>Registers the content of a named rule; the header is replaced by the implementation when a rule is expanded.</summary>
    /// <param name="header">The rule name.</param>
    /// <param name="implementation">The rule it stands for.</param>
    ISecuritySystemSetup AddSecurityRule(DomainSecurityRule.SecurityRuleHeader header, DomainSecurityRule implementation);

    /// <summary>Registers an operation.</summary>
    /// <param name="securityOperation">The operation.</param>
    /// <param name="info">Settings of the operation.</param>
    ISecuritySystemSetup AddSecurityOperation(SecurityOperation securityOperation, SecurityOperationInfo info);

    /// <summary>Adds a permission source in addition to the built-in ones.</summary>
    /// <typeparam name="TPermissionSystemFactory">The factory of the permission source.</typeparam>
    ISecuritySystemSetup AddPermissionSystem<TPermissionSystemFactory>()
        where TPermissionSystemFactory : class, IPermissionSystemFactory;

    /// <summary>Adds a permission source created from the service provider.</summary>
    /// <param name="getFactory">Creates the factory of the permission source.</param>
    ISecuritySystemSetup AddPermissionSystem(Func<IServiceProvider, IPermissionSystemFactory> getFactory);

    /// <summary>Adds a permission source created through the service proxy factory.</summary>
    /// <param name="getFactory">Creates the factory of the permission source.</param>
    ISecuritySystemSetup AddPermissionSystem(Func<IServiceProxyFactory, IPermissionSystemFactory> getFactory);

    /// <summary>Adds a validator that decides whether the current user may run as another user.</summary>
    /// <typeparam name="TValidator">The validator type.</typeparam>
    ISecuritySystemSetup AddRunAsValidator<TValidator>()
        where TValidator : class, IRunAsValidator;

    /// <summary>Adds an extension that registers additional services.</summary>
    /// <param name="extensions">The extension.</param>
    ISecuritySystemSetup AddExtensions(ISecuritySystemExtension extensions);

    /// <summary>Registers additional services together with the security system.</summary>
    /// <param name="addServicesAction">Registers the services.</param>
    ISecuritySystemSetup AddExtensions(Action<IServiceCollection> addServicesAction) =>
        this.AddExtensions(new SecuritySystemExtension(addServicesAction));

    /// <summary>Replaces the creation of the exception thrown when access is denied.</summary>
    /// <typeparam name="TAccessDeniedExceptionService">The service that creates the exception.</typeparam>
    ISecuritySystemSetup SetAccessDeniedExceptionService<TAccessDeniedExceptionService>()
        where TAccessDeniedExceptionService : class, IAccessDeniedExceptionService;

    /// <summary>Configures where application users are taken from (needed for RunAs, impersonation and user lookup).</summary>
    /// <param name="setupUserSource">Optional settings of the user source.</param>
    /// <typeparam name="TUser">The application's user entity type.</typeparam>
    ISecuritySystemSetup AddUserSource<TUser>(Action<IUserSourceSetup<TUser>>? setupUserSource = null)
        where TUser : class;

    /// <summary>Sets the storage of "infinite" accessors, i.e. accessors with unrestricted access.</summary>
    /// <typeparam name="TStorage">The storage type.</typeparam>
    ISecuritySystemSetup SetSecurityAccessorInfinityStorage<TStorage>()
        where TStorage : class, ISecurityAccessorInfinityStorage;

    /// <summary>Adds a listener that is notified about changes of principals and permissions.</summary>
    /// <typeparam name="TPrincipalManagementListener">The listener type.</typeparam>
    ISecuritySystemSetup AddPrincipalManagementListener<TPrincipalManagementListener>()
        where TPrincipalManagementListener : class, IPrincipalManagementListener;

    /// <summary>Adds a service that creates, changes and removes principals and permissions.</summary>
    /// <typeparam name="TPrincipalManagementService">The service type.</typeparam>
    ISecuritySystemSetup AddPrincipalManagementService<TPrincipalManagementService>()
        where TPrincipalManagementService : class, IPrincipalManagementService;

    /// <summary>Sets whose permissions are checked by rules that do not specify it.</summary>
    /// <param name="securityRuleCredential">The default credential.</param>
    ISecuritySystemSetup SetDefaultSecurityRuleCredential(SecurityRuleCredential securityRuleCredential);

    /// <summary>Sets the source that maps the access modes of domain types to client rules (for UI clients).</summary>
    /// <typeparam name="TClientDomainModeSecurityRuleSource">The source type.</typeparam>
    ISecuritySystemSetup SetClientDomainModeSecurityRuleSource<TClientDomainModeSecurityRuleSource>()
        where TClientDomainModeSecurityRuleSource : class, IClientDomainModeSecurityRuleSource;

    /// <summary>Adds a source of client security rules (named rules a UI can request).</summary>
    /// <typeparam name="TClientSecurityRuleInfoSource">The source type.</typeparam>
    ISecuritySystemSetup AddClientSecurityRuleInfoSource<TClientSecurityRuleInfoSource>()
        where TClientSecurityRuleInfoSource : class, IClientSecurityRuleInfoSource;

    /// <summary>Adds a source of client security rules (named rules a UI can request).</summary>
    /// <param name="sourceType">The source type.</param>
    ISecuritySystemSetup AddClientSecurityRuleInfoSource(Type sourceType);

    /// <summary>Sets how the framework reads entities (queries for permissions, principals, security contexts).</summary>
    /// <typeparam name="TQueryableSource">The queryable source implemented over the application's ORM.</typeparam>
    ISecuritySystemSetup SetQueryableSource<TQueryableSource>()
        where TQueryableSource : class, IQueryableSource;

    /// <summary>Sets how the framework reads entities, created from the service provider.</summary>
    /// <param name="getQueryableSource">Creates the queryable source.</param>
    ISecuritySystemSetup SetQueryableSource(Func<IServiceProvider, IQueryableSource> getQueryableSource);

    /// <summary>Sets how the framework saves and removes entities (used when principals and permissions are managed).</summary>
    /// <typeparam name="TGenericRepository">The repository implemented over the application's ORM.</typeparam>
    ISecuritySystemSetup SetGenericRepository<TGenericRepository>()
        where TGenericRepository : class, IGenericRepository;
    /// <summary>Sets the service that reports the user from the environment (for example the authenticated Windows user).</summary>
    /// <typeparam name="TRawCurrentUser">The service type.</typeparam>
    ISecuritySystemSetup SetRawCurrentUser<TRawCurrentUser>()
        where TRawCurrentUser : class, ICurrentUser;

    /// <summary>Sets the service registered as the default current user (<c>ICurrentUser.DefaultKey</c>).</summary>
    /// <typeparam name="TDefaultCurrentUser">The service type.</typeparam>
    ISecuritySystemSetup SetDefaultCurrentUser<TDefaultCurrentUser>()
        where TDefaultCurrentUser : class, ICurrentUser;

    /// <summary>Sets the source of the cancellation token for code that cannot receive one.</summary>
    /// <typeparam name="TDefaultCancellationTokenSource">The service type.</typeparam>
    ISecuritySystemSetup SetDefaultCancellationTokenSource<TDefaultCancellationTokenSource>()
        where TDefaultCancellationTokenSource : class, IDefaultCancellationTokenSource;
}