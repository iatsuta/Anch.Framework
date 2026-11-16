using System.Reflection;

namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Derives client rule names from the properties and modes that declare them.</summary>
public class ClientSecurityRuleNameExtractor : IClientSecurityRuleNameExtractor
{
    public string ExtractName(PropertyInfo propertyInfo) => propertyInfo.Name;

    public string ExtractName(DomainSecurityRule.DomainModeSecurityRule securityRule) => $"{securityRule.DomainType.Name}{securityRule.Mode.Name}";
}
