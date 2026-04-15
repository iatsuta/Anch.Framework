using SecuritySystem.Providers;

namespace SecuritySystem.Services;

public interface IDomainSecurityProviderFactory<TDomainObject>
{
    ISecurityProvider<TDomainObject> Create(SecurityRule securityRule, SecurityPath<TDomainObject>? securityPath);
}