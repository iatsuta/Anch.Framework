using Anch.Core;
using Anch.GenericQueryable;
using Anch.SecuritySystem.Expanders;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.VirtualPermission;

public class VirtualPermissionSystem<TPermission>(
    IServiceProxyFactory serviceProxyFactory,
    ISecurityRuleExpander securityRuleExpander,
    SecurityRuleCredential securityRuleCredential,
    VirtualPermissionBindingInfo<TPermission> virtualBindingInfo)
    : IPermissionSystem<TPermission>

    where TPermission : class
{
    public Type PermissionType { get; } = typeof(TPermission);

    public IPermissionRestrictionSource<TPermission, TSecurityContextIdent> GetRestrictionSource<TSecurityContext, TSecurityContextIdent>(
        SecurityContextRestrictionFilterInfo<TSecurityContext>? restrictionFilterInfo)
        where TSecurityContext : class, ISecurityContext
        where TSecurityContextIdent : notnull
    {
        return serviceProxyFactory
            .Create<
                IPermissionRestrictionSource<TPermission, TSecurityContextIdent>,
                VirtualPermissionRestrictionSource<TPermission, TSecurityContext, TSecurityContextIdent>>(Tuple.Create(restrictionFilterInfo));
    }

    public IEnumerable<IPermissionSource<TPermission>> GetPermissionSources(DomainSecurityRule.RoleBaseSecurityRule securityRule)
    {
        return

            from expandedRolesSecurityRule in securityRuleExpander.FullRoleExpand(securityRule).Children

            from itemBindingInfo in virtualBindingInfo.Items

            where expandedRolesSecurityRule.SecurityRoles.Contains(itemBindingInfo.SecurityRole)

            select this.CreatePermissionSource(expandedRolesSecurityRule, itemBindingInfo);
    }

    public IAsyncEnumerable<SecurityRole> GetAvailableSecurityRoles() =>
        virtualBindingInfo
            .Items
            .ToAsyncEnumerable()
            .Where(async (itemBindingInfo, ct) =>
                await this.CreatePermissionSource(itemBindingInfo.SecurityRole, itemBindingInfo)
                    .GetPermissionQuery()
                    .GenericAnyAsync(ct))
            .Select(itemBindingInfo => itemBindingInfo.SecurityRole)
            .Distinct();

    private IPermissionSource<TPermission> CreatePermissionSource(
        DomainSecurityRule.RoleBaseSecurityRule securityRule,
        VirtualPermissionSecurityRoleItemBindingInfo<TPermission> itemBindingInfo)
    {
        return serviceProxyFactory.Create<IPermissionSource<TPermission>>(
            virtualBindingInfo,
            itemBindingInfo,
            securityRule.TryApply(securityRuleCredential));
    }
}