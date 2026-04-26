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

public interface ISecuritySystemSetup
{
    /// <summary>
    /// Автоматическое добавление относительных путей на самих себя (v => v)
    /// </summary>
    bool AutoAddSelfRelativePath { get; set; }

    bool InitializeDefaultRoles { get; set; }

    ISecuritySystemSetup SetSecurityAdministratorRule(DomainSecurityRule.RoleBaseSecurityRule rule);

    ISecuritySystemSetup AddSecurityContext<TSecurityContext>(TypedSecurityIdentity identity,
        Action<ISecurityContextInfoSetup<TSecurityContext>>? setup = null)
        where TSecurityContext : class, ISecurityContext;

    ISecuritySystemSetup AddDomainSecurity<TDomainObject>(Action<IDomainSecurityServiceSetup<TDomainObject>> setup);

    ISecuritySystemSetup AddDomainSecurity<TDomainObject>(DomainSecurityRule viewSecurityRule,
        SecurityPath<TDomainObject> securityPath)
    {
        return this.AddDomainSecurity(viewSecurityRule, null, securityPath);
    }

    ISecuritySystemSetup AddDomainSecurity<TDomainObject>(DomainSecurityRule viewSecurityRule,
        DomainSecurityRule? editSecurityRule = null,
        SecurityPath<TDomainObject>? securityPath = null)
    {
        return this.AddDomainSecurity<TDomainObject>(b =>
        {
            b.SetView(viewSecurityRule);

            if (editSecurityRule != null)
            {
                b.SetEdit(editSecurityRule);
            }

            if (securityPath != null)
            {
                b.SetPath(securityPath);
            }
        });
    }

    ISecuritySystemSetup AddSecurityRole(SecurityRole securityRole, SecurityRoleInfo info);

    ISecuritySystemSetup AddSecurityRule(DomainSecurityRule.SecurityRuleHeader header, DomainSecurityRule implementation);

    ISecuritySystemSetup AddSecurityOperation(SecurityOperation securityOperation, SecurityOperationInfo info);

    ISecuritySystemSetup AddPermissionSystem<TPermissionSystemFactory>()
        where TPermissionSystemFactory : class, IPermissionSystemFactory;

    ISecuritySystemSetup AddPermissionSystem(Func<IServiceProvider, IPermissionSystemFactory> getFactory);

    ISecuritySystemSetup AddPermissionSystem(Func<IServiceProxyFactory, IPermissionSystemFactory> getFactory);

    ISecuritySystemSetup AddRunAsValidator<TValidator>()
        where TValidator : class, IRunAsValidator;

    ISecuritySystemSetup AddExtensions(ISecuritySystemExtension extensions);

    ISecuritySystemSetup AddExtensions(Action<IServiceCollection> addServicesAction) =>
        this.AddExtensions(new SecuritySystemExtension(addServicesAction));

    ISecuritySystemSetup SetAccessDeniedExceptionService<TAccessDeniedExceptionService>()
        where TAccessDeniedExceptionService : class, IAccessDeniedExceptionService;

    ISecuritySystemSetup AddUserSource<TUser>(Action<IUserSourceSetup<TUser>>? setupUserSource = null)
        where TUser : class;

    ISecuritySystemSetup SetSecurityAccessorInfinityStorage<TStorage>()
        where TStorage : class, ISecurityAccessorInfinityStorage;

    ISecuritySystemSetup SetPrincipalManagementService<TPrincipalManagementService>()
        where TPrincipalManagementService : class, IPrincipalManagementService;

    ISecuritySystemSetup SetDefaultSecurityRuleCredential(SecurityRuleCredential securityRuleCredential);

    ISecuritySystemSetup SetClientDomainModeSecurityRuleSource<TClientDomainModeSecurityRuleSource>()
        where TClientDomainModeSecurityRuleSource : class, IClientDomainModeSecurityRuleSource;

    ISecuritySystemSetup AddClientSecurityRuleInfoSource<TClientSecurityRuleInfoSource>()
        where TClientSecurityRuleInfoSource : class, IClientSecurityRuleInfoSource;

    ISecuritySystemSetup AddClientSecurityRuleInfoSource(Type sourceType);

    ISecuritySystemSetup SetQueryableSource<TQueryableSource>()
        where TQueryableSource : class, IQueryableSource;

    ISecuritySystemSetup SetQueryableSource(Func<IServiceProvider, IQueryableSource> getQueryableSource);

    ISecuritySystemSetup SetGenericRepository<TGenericRepository>()
        where TGenericRepository : class, IGenericRepository;
    ISecuritySystemSetup SetRawCurrentUser<TRawCurrentUser>()
        where TRawCurrentUser : class, ICurrentUser;

    ISecuritySystemSetup SetDefaultCurrentUser<TDefaultCurrentUser>()
        where TDefaultCurrentUser : class, ICurrentUser;

    ISecuritySystemSetup SetDefaultCancellationTokenSource<TDefaultCancellationTokenSource>()
        where TDefaultCancellationTokenSource : class, IDefaultCancellationTokenSource;
}