using System.Collections.Frozen;

namespace SecuritySystem.Services;

public interface ISecurityRoleResolver
{
    FrozenSet<FullSecurityRole> Resolve(DomainSecurityRule.RoleBaseSecurityRule securityRule, bool includeVirtual = false);
}