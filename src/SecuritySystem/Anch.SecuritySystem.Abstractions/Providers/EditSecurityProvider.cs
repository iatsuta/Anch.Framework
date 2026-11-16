using Anch.SecuritySystem.DomainServices;

namespace Anch.SecuritySystem.Providers;

/// <summary>The provider for <see cref="SecurityRule.Edit"/> of a domain type; injected with <c>[EditSecurity]</c>.</summary>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
public class EditSecurityProvider<TDomainObject>(IDomainSecurityService<TDomainObject> domainSecurityService)
    : ProxySecurityProvider<TDomainObject>(domainSecurityService, SecurityRule.Edit);