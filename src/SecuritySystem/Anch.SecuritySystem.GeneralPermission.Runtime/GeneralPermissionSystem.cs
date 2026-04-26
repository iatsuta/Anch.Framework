using Anch.Core;
using Anch.GenericQueryable;
using Anch.IdentitySource;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

public class GeneralPermissionSystem<TPermission>(
    IServiceProxyFactory serviceProxyFactory,
    GeneralPermissionBindingInfo<TPermission> generalBindingInfo,
    IIdentityInfoSource identityInfoSource,
    SecurityRuleCredential defaultSecurityRuleCredential) : IPermissionSystem<TPermission>
{
    private readonly Lazy<IPermissionSystem<TPermission>> lazyInnerService = new(() =>
    {
        var securityRoleIdentityInfo = identityInfoSource.GetIdentityInfo(generalBindingInfo.SecurityRoleType);

        var innerServiceType = typeof(GeneralPermissionSystem<,,>).MakeGenericType(
            generalBindingInfo.PermissionType,
            generalBindingInfo.SecurityRoleType,
            securityRoleIdentityInfo.IdentityType);

        return serviceProxyFactory.Create<IPermissionSystem<TPermission>>(
            innerServiceType,
            defaultSecurityRuleCredential);
    });

    private IPermissionSystem<TPermission> InnerService => this.lazyInnerService.Value;

    public Type PermissionType => this.InnerService.PermissionType;

    public IPermissionRestrictionSource<TPermission, TSecurityContextIdent> GetRestrictionSource<TSecurityContext, TSecurityContextIdent>(
        SecurityContextRestrictionFilterInfo<TSecurityContext>? restrictionFilterInfo)
        where TSecurityContext : class, ISecurityContext
        where TSecurityContextIdent : notnull =>
        this.InnerService.GetRestrictionSource<TSecurityContext, TSecurityContextIdent>(restrictionFilterInfo);

    public IEnumerable<IPermissionSource<TPermission>> GetPermissionSources(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        this.InnerService.GetPermissionSources(securityRule);

    IEnumerable<IPermissionSource> IPermissionSystem.GetPermissionSources(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        ((IPermissionSystem)this.InnerService).GetPermissionSources(securityRule);

    public IAsyncEnumerable<SecurityRole> GetAvailableSecurityRoles() => this.InnerService.GetAvailableSecurityRoles();
}

public class GeneralPermissionSystem<TPermission, TSecurityRole, TSecurityRoleIdent>(
    IServiceProxyFactory serviceProxyFactory,
    GeneralPermissionBindingInfo<TPermission, TSecurityRole> generalBindingInfo,
    IIdentityInfo<TSecurityRole, TSecurityRoleIdent> securityRoleIdentityInfo,
    IAvailablePermissionSource<TPermission> availablePermissionSource,
    ISecurityRoleSource securityRoleSource,
    SecurityRuleCredential defaultSecurityRuleCredential)
    : IPermissionSystem<TPermission>

    where TPermission : class
    where TSecurityRole : class
    where TSecurityRoleIdent : notnull
{
    public Type PermissionType { get; } = typeof(TPermission);

    public IPermissionRestrictionSource<TPermission, TSecurityContextIdent> GetRestrictionSource<TSecurityContext, TSecurityContextIdent>(
        SecurityContextRestrictionFilterInfo<TSecurityContext>? restrictionFilterInfo)
        where TSecurityContext : class, ISecurityContext
        where TSecurityContextIdent : notnull =>

        serviceProxyFactory
            .Create<IPermissionRestrictionSource<TPermission, TSecurityContextIdent>,
                GeneralPermissionRestrictionSource<TPermission, TSecurityContext, TSecurityContextIdent>>(
                new Tuple<SecurityContextRestrictionFilterInfo<TSecurityContext>?>(restrictionFilterInfo));

    public IEnumerable<IPermissionSource<TPermission>> GetPermissionSources(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
    [
        serviceProxyFactory.Create<IPermissionSource<TPermission>, GeneralPermissionSource<TPermission>>(
            securityRule.TryApply(defaultSecurityRuleCredential))
    ];

    public IAsyncEnumerable<SecurityRole> GetAvailableSecurityRoles()
    {
        return AsyncEnumerable
            .Select<TSecurityRoleIdent, FullSecurityRole>(availablePermissionSource
                .GetQueryable(DomainSecurityRule.AnyRole with { CustomCredential = defaultSecurityRuleCredential })
                .Select(generalBindingInfo.SecurityRole.Path.Select(securityRoleIdentityInfo.Id.Path))
                .Distinct()
                .GenericAsAsyncEnumerable(), ident => securityRoleSource.GetSecurityRole(TypedSecurityIdentity.Create<TSecurityRoleIdent>(ident)));
    }
}