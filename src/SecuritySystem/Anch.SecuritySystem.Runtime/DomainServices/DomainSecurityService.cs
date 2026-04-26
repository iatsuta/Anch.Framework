using Anch.SecuritySystem.Providers;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.DomainServices;

public class DomainSecurityService<TDomainObject>(IDomainSecurityProviderFactory<TDomainObject> domainSecurityProviderFactory)
    : DomainSecurityServiceBase<TDomainObject>
{
    protected override ISecurityProvider<TDomainObject> CreateSecurityProvider(SecurityRule securityRule) =>
        domainSecurityProviderFactory.Create(securityRule, null);
}