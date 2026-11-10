using System.Collections.Immutable;

using Anch.Core;
using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.AvailableSecurity;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.Providers;
using Anch.SecuritySystem.Services;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>
/// The default <see cref="ISecuritySystem"/>: prepares the rule with the access optimizer and evaluates it with three-valued logic
/// (<c>true</c>, <c>false</c>, <c>null</c> for a rule that cannot be decided without a domain object, which counts as allowed).
/// A plain role rule is checked against the roles available to the user, any other role rule is checked by the permission systems.
/// </summary>
public class SecuritySystem(
    ISecurityRuleOptimizer securityRuleOptimizer,
    IAccessDeniedExceptionService accessDeniedExceptionService,
    IAvailableSecurityRoleSource availableSecurityRoleSource,
    ImmutableArray<IPermissionSystem> permissionSystems) : ISecuritySystem
{
    private ImmutableHashSet<SecurityRole>? cachedSecurityRoles;


    public async Task<bool> HasAccessAsync(DomainSecurityRule securityRule, CancellationToken ct) =>

        await this.HasAccessAsyncInternal(securityRuleOptimizer.Optimize(securityRule), ct) ?? true;

    /// <summary>Evaluates an optimized rule.</summary>
    /// <param name="securityRule">The optimized rule.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns><c>true</c> or <c>false</c> when the rule is decided, <c>null</c> when it cannot be decided without a domain object.</returns>
    protected virtual async Task<bool?> HasAccessAsyncInternal(DomainSecurityRule securityRule, CancellationToken ct)
    {
        switch (securityRule)
        {
            case DomainSecurityRule.RoleBaseSecurityRule roleBaseSecurityRule:
                return await this.HasAccessAsync(roleBaseSecurityRule, ct);

            case DomainSecurityRule.OrSecurityRule { Left: var left, Right: var right }:
            {
                var l = await this.HasAccessAsyncInternal(left.TryApply(securityRule.CustomCredential), ct);
                if (l == true) return true;

                var r = await this.HasAccessAsyncInternal(right.TryApply(securityRule.CustomCredential), ct);
                if (r == true) return true;

                return l == false && r == false ? false : null;
            }

            case DomainSecurityRule.AndSecurityRule { Left: var left, Right: var right }:
            {
                var l = await this.HasAccessAsyncInternal(left.TryApply(securityRule.CustomCredential), ct);
                if (l == false) return false;

                var r = await this.HasAccessAsyncInternal(right.TryApply(securityRule.CustomCredential), ct);
                if (r == false) return false;

                return l == true && r == true ? true : null;
            }

            case DomainSecurityRule.NegateSecurityRule { InnerRule: var innerRule }:
            {
                return !await this.HasAccessAsyncInternal(innerRule.TryApply(securityRule.CustomCredential), ct);
            }

            default:
                return null;
        }
    }

    public async Task CheckAccessAsync(DomainSecurityRule securityRule, CancellationToken ct)
    {
        if (!await this.HasAccessAsync(securityRule, ct))
        {
            throw accessDeniedExceptionService.GetAccessDeniedException(new AccessResult.AccessDeniedResult { SecurityRule = securityRule });
        }
    }

    private async Task<bool> HasAccessAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, CancellationToken ct)
    {
        if (securityRule.CustomCredential is null && securityRule.CustomRestriction is null &&
            securityRule is DomainSecurityRule.NonExpandedRolesSecurityRule roleSecurityRule)
        {
            var securityRoles = await this.GetAvailableSecurityRoles(ct);

            return securityRoles.Overlaps(roleSecurityRule.SecurityRoles);
        }
        else
        {
            return await permissionSystems
                .SelectMany(v => v.GetPermissionSources(securityRule))
                .ToAsyncEnumerable()
                .AnyAsync(async (permissionSource, lct) => await permissionSource.HasAccessAsync(lct), ct);
        }
    }

    private async ValueTask<ImmutableHashSet<SecurityRole>> GetAvailableSecurityRoles(CancellationToken ct) =>
        this.cachedSecurityRoles ??= await availableSecurityRoleSource.GetAvailableSecurityRoles().ToImmutableHashSetAsync<SecurityRole>(ct);
}