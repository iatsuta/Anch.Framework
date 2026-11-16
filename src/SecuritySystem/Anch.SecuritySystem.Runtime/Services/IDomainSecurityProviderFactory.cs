using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.Services;

/// <summary>Creates the provider that enforces a rule for a domain type.</summary>
public interface IDomainSecurityProviderFactory<TDomainObject>
{
    ISecurityProvider<TDomainObject> Create(SecurityRule securityRule, SecurityPath<TDomainObject>? securityPath);
}