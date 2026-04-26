namespace Anch.SecuritySystem.Expanders;

public interface IRoleFactorySecurityRuleExpander
{
    DomainSecurityRule.RoleBaseSecurityRule Expand(DomainSecurityRule.RoleFactorySecurityRule securityRule);
}
