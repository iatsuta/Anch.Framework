using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.SecurityRuleInfo;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Creates a security system that checks the permissions of a given user.</summary>
public class SecuritySystemFactory(
    IAccessDeniedExceptionService accessDeniedExceptionService,
    IDomainSecurityRoleExtractor domainSecurityRoleExtractor,
    IEnumerable<IPermissionSystemFactory> permissionSystems) : ISecuritySystemFactory
{
    public ISecuritySystem Create(SecurityRuleCredential securityRuleCredential)
    {
        return new SecuritySystem(
            accessDeniedExceptionService,
            [.. permissionSystems.Select(f => f.Create(securityRuleCredential))],
            domainSecurityRoleExtractor);
    }
}