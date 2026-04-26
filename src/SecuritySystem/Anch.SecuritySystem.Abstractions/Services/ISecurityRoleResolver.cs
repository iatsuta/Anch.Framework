using System.Collections.Frozen;

namespace Anch.SecuritySystem.Services;

public interface ISecurityRoleResolver
{
    FrozenSet<FullSecurityRole> Resolve(DomainSecurityRule.RoleBaseSecurityRule securityRule, bool includeVirtual = false);
}