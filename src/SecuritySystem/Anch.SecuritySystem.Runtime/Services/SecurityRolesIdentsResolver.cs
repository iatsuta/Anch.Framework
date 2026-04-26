using System.Collections.Concurrent;
using System.Collections.Frozen;
using Anch.Core;

namespace Anch.SecuritySystem.Services;

public class SecurityRoleIdentsResolver(ISecurityRoleResolver securityRoleResolver) : ISecurityRoleIdentsResolver
{
    private readonly ConcurrentDictionary<(DomainSecurityRule.RoleBaseSecurityRule, bool), FrozenDictionary<Type, Array>> cache = [];

    public FrozenDictionary<Type, Array> Resolve(DomainSecurityRule.RoleBaseSecurityRule baseSecurityRule, bool includeVirtual = false) =>

        this.cache.GetOrAdd((baseSecurityRule.WithDefaultCustoms(), includeVirtual), pair =>

            securityRoleResolver.Resolve(pair.Item1, pair.Item2)
                .Select(sr => sr.Identity)
                .GroupBy(i => i.IdentType, i => i.GetId())
                .ToFrozenDictionary(g => g.Key, g => g.ToArray(g.Key)));
}