using System.Reflection;

namespace Anch.SecuritySystem.SecurityRuleInfo;

public interface IClientSecurityRuleNameExtractor
{
    string ExtractName(PropertyInfo propertyInfo);

    string ExtractName(DomainSecurityRule.DomainModeSecurityRule securityRule);
}
