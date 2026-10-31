using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.Services;

/// <summary>Creates the provider for a role-based rule.</summary>
public interface IRoleBaseSecurityProviderFactory<TDomainObject>
{
    ISecurityProvider<TDomainObject> Create(DomainSecurityRule.RoleBaseSecurityRule securityRule, SecurityPath<TDomainObject>? securityPath);
}