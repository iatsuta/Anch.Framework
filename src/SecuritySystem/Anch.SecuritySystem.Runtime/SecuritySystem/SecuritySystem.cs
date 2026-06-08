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
    public Task<bool> HasAccessAsync(DomainSecurityRule securityRule, CancellationToken ct) =>
        this.HasAccessAsync(domainSecurityRoleExtractor.ExtractSecurityRule(securityRule), ct);

    public Task CheckAccessAsync(DomainSecurityRule securityRule, CancellationToken ct) =>
        this.CheckAccess(domainSecurityRoleExtractor.ExtractSecurityRule(securityRule), ct);

    private async Task<bool> HasAccessAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, CancellationToken ct) =>
        await permissionSystems
            .SelectMany(v => v.GetPermissionSources(securityRule))
            .ToAsyncEnumerable()
            .AnyAsync(async (permissionSource, lct) => await permissionSource.HasAccessAsync(lct), ct);

    private async Task CheckAccess(DomainSecurityRule.RoleBaseSecurityRule securityRule, CancellationToken ct)
    {
        if (!await this.HasAccessAsync(securityRule, ct))
        {
            throw accessDeniedExceptionService.GetAccessDeniedException(
                new AccessResult.AccessDeniedResult { SecurityRule = securityRule });
        }
    }
}