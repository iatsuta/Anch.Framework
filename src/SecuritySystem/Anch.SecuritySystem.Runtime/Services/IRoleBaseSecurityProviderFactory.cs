using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.Services;

public interface IRoleBaseSecurityProviderFactory<TDomainObject>
{
    ISecurityProvider<TDomainObject> Create(DomainSecurityRule.RoleBaseSecurityRule securityRule, SecurityPath<TDomainObject>? securityPath);
}