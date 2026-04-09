using System.Collections.Immutable;

using SecuritySystem.AccessDenied;
using SecuritySystem.ExternalSystem;
using SecuritySystem.Providers;
using SecuritySystem.SecurityRuleInfo;

// ReSharper disable once CheckNamespace
namespace SecuritySystem;

public class SecuritySystem(
    IAccessDeniedExceptionService accessDeniedExceptionService,
    ImmutableArray<IPermissionSystem> permissionSystems,
    IDomainSecurityRoleExtractor domainSecurityRoleExtractor) : ISecuritySystem
{
    public ValueTask<bool> HasAccessAsync(DomainSecurityRule securityRule, CancellationToken cancellationToken) =>
        this.HasAccessAsync(domainSecurityRoleExtractor.ExtractSecurityRule(securityRule), cancellationToken);

    public ValueTask CheckAccessAsync(DomainSecurityRule securityRule, CancellationToken cancellationToken) =>
        this.CheckAccess(domainSecurityRoleExtractor.ExtractSecurityRule(securityRule), cancellationToken);

    private ValueTask<bool> HasAccessAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, CancellationToken cancellationToken) =>
        permissionSystems
            .SelectMany(v => v.GetPermissionSources(securityRule))
            .ToAsyncEnumerable()
            .AnyAsync((permissionSource, ct) => permissionSource.HasAccessAsync(ct), cancellationToken);

    private async ValueTask CheckAccess(DomainSecurityRule.RoleBaseSecurityRule securityRule, CancellationToken cancellationToken)
    {
        if (!await this.HasAccessAsync(securityRule, cancellationToken))
        {
            throw accessDeniedExceptionService.GetAccessDeniedException(
                new AccessResult.AccessDeniedResult { SecurityRule = securityRule });
        }
    }
}