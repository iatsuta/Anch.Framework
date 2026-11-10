using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Creates a role source over the permission systems of the given user.</summary>
public class AvailableSecurityRoleSourceFactory(IEnumerable<IPermissionSystemFactory> permissionSystemFactories, ISecurityRoleSource securityRoleSource)
    : IAvailableSecurityRoleSourceFactory
{
    public IAvailableSecurityRoleSource Create(SecurityRuleCredential securityRuleCredential)
    {
        return new AvailableSecurityRoleSource(permissionSystemFactories.Select(f => f.Create(securityRuleCredential)), securityRoleSource);
    }
}