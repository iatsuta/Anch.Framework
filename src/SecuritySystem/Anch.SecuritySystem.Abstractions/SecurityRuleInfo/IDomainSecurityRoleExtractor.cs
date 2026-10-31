using System.Collections.Immutable;

namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Extracts the roles that a rule is made of.</summary>
public interface IDomainSecurityRoleExtractor
{
    /// <summary>Returns the roles that appear in the rule after expansion.</summary>
    /// <param name="securityRule">The rule.</param>
    ImmutableHashSet<SecurityRole> ExtractSecurityRoles(DomainSecurityRule securityRule);

    /// <summary>Returns the rule in its expanded role form.</summary>
    /// <param name="securityRule">The rule.</param>
    DomainSecurityRule.ExpandedRoleGroupSecurityRule ExtractSecurityRule(DomainSecurityRule securityRule);
}
