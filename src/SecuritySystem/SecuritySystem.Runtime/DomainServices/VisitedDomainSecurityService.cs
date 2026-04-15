using SecuritySystem.Expanders;
using SecuritySystem.Providers;
using SecuritySystem.Services;

namespace SecuritySystem.DomainServices;

public class VisitedDomainSecurityService<TDomainObject>(
    ISecurityRuleExpander securityRuleExpander,
    IDomainSecurityProviderFactory<TDomainObject> domainSecurityProviderFactory) : DomainSecurityServiceBase<TDomainObject>
{
    protected sealed override ISecurityProvider<TDomainObject> CreateSecurityProvider(SecurityRule baseSecurityRule) =>

        baseSecurityRule switch
        {
            SecurityRule.ModeSecurityRule securityRule => this.CreateSecurityProvider(securityRule),
            DomainSecurityRule.DomainModeSecurityRule securityRule => this.CreateSecurityProvider(securityRuleExpander.Expand(securityRule)),
            DomainSecurityRule.ClientSecurityRule securityRule => this.CreateSecurityProvider(securityRuleExpander.Expand(securityRule)),
            DomainSecurityRule.OperationSecurityRule securityRule => this.CreateSecurityProvider(securityRule),
            DomainSecurityRule.NonExpandedRolesSecurityRule securityRule => this.CreateSecurityProvider(securityRule),
            DomainSecurityRule.ExpandedRolesSecurityRule securityRule => this.CreateSecurityProvider(securityRule),
            DomainSecurityRule securityRule => this.Create(securityRule),
            _ => throw new ArgumentOutOfRangeException(nameof(baseSecurityRule))
        };

    protected virtual ISecurityProvider<TDomainObject> CreateSecurityProvider(SecurityRule.ModeSecurityRule securityRule) => this.GetSecurityProvider(securityRule.ToDomain<TDomainObject>());

    protected virtual ISecurityProvider<TDomainObject> CreateSecurityProvider(DomainSecurityRule.OperationSecurityRule securityRule) => this.GetSecurityProvider(securityRuleExpander.Expand(securityRule));

    protected virtual ISecurityProvider<TDomainObject> CreateSecurityProvider(DomainSecurityRule.NonExpandedRolesSecurityRule securityRule) => this.GetSecurityProvider(securityRuleExpander.Expand(securityRule));

    protected virtual ISecurityProvider<TDomainObject> CreateSecurityProvider(DomainSecurityRule.ExpandedRolesSecurityRule securityRule) => this.Create(securityRule, null);

    protected virtual ISecurityProvider<TDomainObject> Create(
        DomainSecurityRule securityRule,
        SecurityPath<TDomainObject>? customSecurityPath = null) => domainSecurityProviderFactory.Create(securityRule, customSecurityPath);
}