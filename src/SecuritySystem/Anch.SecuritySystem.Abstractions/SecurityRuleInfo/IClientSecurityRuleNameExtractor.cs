using System.Reflection;

namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Derives the client-facing name of a rule.</summary>
public interface IClientSecurityRuleNameExtractor
{
    /// <summary>Derives the name from a property that declares the rule.</summary>
    /// <param name="propertyInfo">The property.</param>
    string ExtractName(PropertyInfo propertyInfo);

    /// <summary>Derives the name from a domain type mode.</summary>
    /// <param name="securityRule">The mode of a domain type.</param>
    string ExtractName(DomainSecurityRule.DomainModeSecurityRule securityRule);
}
