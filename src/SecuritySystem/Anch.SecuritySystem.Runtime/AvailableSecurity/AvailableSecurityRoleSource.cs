using Anch.Core;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Lists the roles that the current user has, collected from all permission systems.</summary>
public class AvailableSecurityRoleSource(IEnumerable<IPermissionSystem> permissionSystems, ISecurityRoleSource securityRoleSource)
    : IAvailableSecurityRoleSource
{
    public IAsyncEnumerable<FullSecurityRole> GetAvailableSecurityRoles(bool expandChildren)
    {
        var roles = permissionSystems
            .ToAsyncEnumerable()
            .SelectMany(ps => ps.GetAvailableSecurityRoles())
            .Distinct()
            .Select(securityRoleSource.GetSecurityRole);

        return
            expandChildren
                ? roles.GetAllElements(sr => sr.Information.Children.ToAsyncEnumerable().Select(securityRoleSource.GetSecurityRole)).Distinct()
                : roles;
    }
}