using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.SecurityRuleInfo;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public class SecuritySystemFactory(
    IAccessDeniedExceptionService accessDeniedExceptionService,
    IDomainSecurityRoleExtractor domainSecurityRoleExtractor,
    IEnumerable<IPermissionSystemFactory> permissionSystems) : ISecuritySystemFactory
{
    public ISecuritySystem Create(SecurityRuleCredential securityRuleCredential)
    {
        return new SecuritySystem(
            accessDeniedExceptionService,
            [..permissionSystems.Select(f => f.Create(securityRuleCredential))],
            domainSecurityRoleExtractor);
    }
}