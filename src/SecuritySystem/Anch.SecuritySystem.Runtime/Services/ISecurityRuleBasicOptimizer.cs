namespace Anch.SecuritySystem.Services;

/// <summary>Simplifies a rule without expanding it.</summary>
public interface ISecurityRuleBasicOptimizer
{
    DomainSecurityRule Optimize(DomainSecurityRule securityRule);
}
