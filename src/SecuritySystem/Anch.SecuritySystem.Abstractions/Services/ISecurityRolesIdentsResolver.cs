using System.Collections.Frozen;

namespace Anch.SecuritySystem.Services;

public interface ISecurityRoleIdentsResolver
{
    FrozenDictionary<Type, Array> Resolve(DomainSecurityRule.RoleBaseSecurityRule securityRule, bool includeVirtual = false);
}