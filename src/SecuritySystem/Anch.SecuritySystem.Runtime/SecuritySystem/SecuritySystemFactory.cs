using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.AvailableSecurity;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.Services;

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Creates a security system that checks the permissions of a given user.</summary>
public class SecuritySystemFactory(
    [FromKeyedServices(ISecurityRuleOptimizer.AccessKey)] ISecurityRuleOptimizer securityRuleOptimizer,
    IAccessDeniedExceptionService accessDeniedExceptionService,
    IAvailableSecurityRoleSourceFactory availableSecurityRoleSourceFactory,
    IEnumerable<IPermissionSystemFactory> permissionSystems) : ISecuritySystemFactory
{
    public ISecuritySystem Create(SecurityRuleCredential securityRuleCredential)
    {
        return new SecuritySystem(
            securityRuleOptimizer,
            accessDeniedExceptionService,
            availableSecurityRoleSourceFactory.Create(securityRuleCredential),
            [.. permissionSystems.Select(f => f.Create(securityRuleCredential))]);
    }
}