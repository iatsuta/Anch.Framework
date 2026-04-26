namespace Anch.SecuritySystem.Expanders;

public interface ISecurityRuleExpander : ISecurityModeExpander,
    ISecurityOperationExpander,
    ISecurityRoleGroupExpander,
    IRoleFactorySecurityRuleExpander,
    IClientSecurityRuleExpander,
    ISecurityRuleHeaderExpander
{
    DomainSecurityRule.ExpandedRoleGroupSecurityRule FullRoleExpand(DomainSecurityRule.RoleBaseSecurityRule securityRule);

    DomainSecurityRule FullDomainExpand(DomainSecurityRule securityRule);

    DomainSecurityRule ToDomain<TDomainObject>(SecurityRule securityRule);
}