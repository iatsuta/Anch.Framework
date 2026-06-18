using Anch.SecuritySystem.DomainServices;

namespace Anch.SecuritySystem.Providers;

public class ViewSecurityProvider<TDomainObject>(IDomainSecurityService<TDomainObject> domainSecurityService)
    : ProxySecurityProvider<TDomainObject>(domainSecurityService, SecurityRule.View);