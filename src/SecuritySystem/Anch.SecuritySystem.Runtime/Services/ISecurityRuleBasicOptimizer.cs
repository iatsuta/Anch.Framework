namespace Anch.SecuritySystem.Services;

public interface ISecurityRuleBasicOptimizer
{
    DomainSecurityRule Optimize(DomainSecurityRule securityRule);
}
