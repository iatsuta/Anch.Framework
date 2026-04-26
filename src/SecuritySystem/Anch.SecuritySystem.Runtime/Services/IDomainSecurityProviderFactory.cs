using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.Services;

public interface IDomainSecurityProviderFactory<TDomainObject>
{
    ISecurityProvider<TDomainObject> Create(SecurityRule securityRule, SecurityPath<TDomainObject>? securityPath);
}