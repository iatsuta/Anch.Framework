namespace Anch.SecuritySystem.Expanders;

public interface ISecurityModeExpander
{
    DomainSecurityRule? TryExpand(DomainSecurityRule.DomainModeSecurityRule securityRule);
}
