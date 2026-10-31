namespace Anch.SecuritySystem.Services;

/// <summary>Expands and simplifies a rule.</summary>
public interface ISecurityRuleDeepOptimizer
{
    DomainSecurityRule Optimize(DomainSecurityRule securityRule);
}