using Anch.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces a role factory rule with the role rule that the factory creates.</summary>
public class RoleFactorySecurityRuleExpander(IServiceProvider serviceProvider) : IRoleFactorySecurityRuleExpander
{
    public DomainSecurityRule.RoleBaseSecurityRule Expand(DomainSecurityRule.RoleFactorySecurityRule securityRule)
    {
        var factory = (IFactory<DomainSecurityRule.RoleBaseSecurityRule>)serviceProvider.GetRequiredService(securityRule.RoleFactoryType);

        return factory.Create().ApplyCustoms(securityRule);
    }
}
