using Anch.SecuritySystem.Providers;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.DomainServices;

/// <summary>The default domain security service: builds the provider for a rule through the provider factory.</summary>
public class DomainSecurityService<TDomainObject>(IDomainSecurityProviderFactory<TDomainObject> domainSecurityProviderFactory)
    : DomainSecurityServiceBase<TDomainObject>
{
    protected override ISecurityProvider<TDomainObject> CreateSecurityProvider(SecurityRule securityRule) =>
        domainSecurityProviderFactory.Create(securityRule, null);
}