using System.Collections.Concurrent;
using System.Collections.Frozen;
using SecuritySystem.Expanders;

namespace SecuritySystem.Services;

public class SecurityRoleResolver(ISecurityRuleExpander securityRuleExpander, ISecurityRoleSource securityRoleSource) : ISecurityRoleResolver
{
    private readonly ConcurrentDictionary<(DomainSecurityRule.RoleBaseSecurityRule, bool), FrozenSet<FullSecurityRole>> cache = [];

    public FrozenSet<FullSecurityRole> Resolve(DomainSecurityRule.RoleBaseSecurityRule baseSecurityRule, bool includeVirtual = false) =>

        this.cache.GetOrAdd((baseSecurityRule.WithDefaultCustoms(), includeVirtual), pair =>

            securityRuleExpander
                .FullRoleExpand(pair.Item1)
                .Children
                .SelectMany(c => c.SecurityRoles)
                .Distinct()
                .Select(securityRoleSource.GetSecurityRole)
                .Where(sr => includeVirtual || !sr.Information.IsVirtual)
                .ToFrozenSet());
}