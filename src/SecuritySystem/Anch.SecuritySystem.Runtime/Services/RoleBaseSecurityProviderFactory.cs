using Anch.Core;
using Anch.SecuritySystem.Builders._Factory;
using Anch.SecuritySystem.Expanders;
using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.Services;

public class RoleBaseSecurityProviderFactory<TDomainObject>(
    ISecurityFilterFactory<TDomainObject> securityFilterFactory,
    IAccessorsFilterFactory<TDomainObject> accessorsFilterFactory,
    ISecurityRuleExpander securityRuleExpander,
    ISecurityPathRestrictionService securityPathRestrictionService,
    SecurityPath<TDomainObject>? defaultSecurityPath = null,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null) : IRoleBaseSecurityProviderFactory<TDomainObject>
{
    public virtual ISecurityProvider<TDomainObject> Create(DomainSecurityRule.RoleBaseSecurityRule securityRule, SecurityPath<TDomainObject>? securityPath) =>

        securityRuleExpander
            .FullRoleExpand(securityRule)
            .GetActualChildren()
            .Select(innerSecurityRule => this.Create(innerSecurityRule, securityPath))
            .Or();

    private ISecurityProvider<TDomainObject> Create(DomainSecurityRule.ExpandedRolesSecurityRule securityRule, SecurityPath<TDomainObject>? securityPath) =>

        new RoleBaseSecurityPathProvider<TDomainObject>(
            securityFilterFactory,
            accessorsFilterFactory,
            securityRule,
            securityPathRestrictionService.ApplyRestriction(
                securityPath ?? defaultSecurityPath,
                securityRule.CustomRestriction ?? SecurityPathRestriction.Default),
            defaultCancellationTokenSource);
}