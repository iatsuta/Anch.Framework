using System.Collections.Frozen;

namespace Anch.SecuritySystem.Services;

/// <summary>Finds the identifiers of the roles that a rule is satisfied by.</summary>
public interface ISecurityRoleIdentsResolver
{
    /// <summary>Returns the role identifiers of the rule, grouped by identifier type.</summary>
    /// <param name="securityRule">The rule.</param>
    /// <param name="includeVirtual">Whether to include virtual roles.</param>
    FrozenDictionary<Type, Array> Resolve(DomainSecurityRule.RoleBaseSecurityRule securityRule, bool includeVirtual = false);
}