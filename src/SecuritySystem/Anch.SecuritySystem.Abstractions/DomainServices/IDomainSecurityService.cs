using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.DomainServices;

/// <summary>
/// Entry point for protecting domain objects of one type: turns a <see cref="SecurityRule"/> into an <see cref="ISecurityProvider{TDomainObject}"/>
/// that can filter queries and check single objects.
/// </summary>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
public interface IDomainSecurityService<TDomainObject>
{
    /// <summary>Returns the provider that enforces the rule for the domain type.</summary>
    /// <param name="securityRule">The required access, for example <see cref="SecurityRule.View"/>.</param>
    ISecurityProvider<TDomainObject> GetSecurityProvider(SecurityRule securityRule);
}
