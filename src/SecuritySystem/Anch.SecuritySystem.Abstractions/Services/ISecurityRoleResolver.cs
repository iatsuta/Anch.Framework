using System.Collections.Frozen;

namespace Anch.SecuritySystem.Services;

/// <summary>Finds the roles that a rule is satisfied by.</summary>
public interface ISecurityRoleResolver
{
    /// <summary>Returns the roles of the rule.</summary>
    /// <param name="securityRule">The rule.</param>
    /// <param name="includeVirtual">Whether to include virtual roles.</param>
    FrozenSet<FullSecurityRole> Resolve(DomainSecurityRule.RoleBaseSecurityRule securityRule, bool includeVirtual = false);
}