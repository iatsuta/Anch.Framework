using Anch.SecuritySystem.DomainServices;

namespace Anch.SecuritySystem.Providers;

public class EditSecurityProvider<TDomainObject>(IDomainSecurityService<TDomainObject> domainSecurityService)
    : ProxySecurityProvider<TDomainObject>(domainSecurityService, SecurityRule.Edit);