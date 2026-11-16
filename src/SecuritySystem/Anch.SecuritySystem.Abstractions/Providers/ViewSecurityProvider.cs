using Anch.SecuritySystem.DomainServices;

namespace Anch.SecuritySystem.Providers;

/// <summary>The provider for <see cref="SecurityRule.View"/> of a domain type; injected with <c>[ViewSecurity]</c>.</summary>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
public class ViewSecurityProvider<TDomainObject>(IDomainSecurityService<TDomainObject> domainSecurityService)
    : ProxySecurityProvider<TDomainObject>(domainSecurityService, SecurityRule.View);