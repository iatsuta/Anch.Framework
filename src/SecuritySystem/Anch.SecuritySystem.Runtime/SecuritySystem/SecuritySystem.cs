using System.Collections.Immutable;

using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.Providers;
using Anch.SecuritySystem.SecurityRuleInfo;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public class SecuritySystem(
    IAccessDeniedExceptionService accessDeniedExceptionService,
    ImmutableArray<IPermissionSystem> permissionSystems,
    IDomainSecurityRoleExtractor domainSecurityRoleExtractor) : ISecuritySystem
{
    public Task<bool> HasAccessAsync(DomainSecurityRule securityRule, CancellationToken cancellationToken) =>
        this.HasAccessAsync(domainSecurityRoleExtractor.ExtractSecurityRule(securityRule), cancellationToken);

    public Task CheckAccessAsync(DomainSecurityRule securityRule, CancellationToken cancellationToken) =>
        this.CheckAccess(domainSecurityRoleExtractor.ExtractSecurityRule(securityRule), cancellationToken);

    private async Task<bool> HasAccessAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, CancellationToken cancellationToken) =>
        await permissionSystems
            .SelectMany(v => v.GetPermissionSources(securityRule))
            .ToAsyncEnumerable()
            .AnyAsync(async (permissionSource, ct) => await permissionSource.HasAccessAsync(ct), cancellationToken);

    private async Task CheckAccess(DomainSecurityRule.RoleBaseSecurityRule securityRule, CancellationToken cancellationToken)
    {
        if (!await this.HasAccessAsync(securityRule, cancellationToken))
        {
            throw accessDeniedExceptionService.GetAccessDeniedException(
                new AccessResult.AccessDeniedResult { SecurityRule = securityRule });
        }
    }
}