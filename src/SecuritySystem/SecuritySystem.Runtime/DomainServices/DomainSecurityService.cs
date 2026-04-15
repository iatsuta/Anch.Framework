using SecuritySystem.Providers;
using SecuritySystem.Services;

namespace SecuritySystem.DomainServices;

public class DomainSecurityService<TDomainObject>(IDomainSecurityProviderFactory<TDomainObject> domainSecurityProviderFactory)
    : DomainSecurityServiceBase<TDomainObject>
{
    protected override ISecurityProvider<TDomainObject> CreateSecurityProvider(SecurityRule securityRule) =>
        domainSecurityProviderFactory.Create(securityRule, null);
}