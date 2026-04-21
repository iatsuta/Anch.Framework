using SecuritySystem.Expanders;
using SecuritySystem.ExternalSystem;

namespace SecuritySystem.DiTests.Environment;

public class TestPermissionSystem(ISecurityRuleExpander securityRuleExpander, TestPermissionStorge permissionStorge)
    : IPermissionSystem
{
    public Type PermissionType => throw new NotImplementedException();

    public IEnumerable<IPermissionSource> GetPermissionSources(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        [new TestPermissionSource(permissionStorge, securityRuleExpander.FullRoleExpand(securityRule))];

    public IAsyncEnumerable<SecurityRole> GetAvailableSecurityRoles() =>
        permissionStorge.Permissions.Select(p => p.SecurityRole!).ToAsyncEnumerable();
}