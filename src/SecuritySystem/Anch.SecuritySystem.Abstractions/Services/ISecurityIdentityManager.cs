using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

/// <summary>Reads and writes the identifier of an entity as a security identifier.</summary>
/// <typeparam name="TDomainObject">The entity type.</typeparam>
public interface ISecurityIdentityManager<TDomainObject>
{
    /// <summary>Converts identifiers to the identifier type of the entity.</summary>
    ISecurityIdentityConverter Converter { get; }

    /// <summary>An expression that selects the identifier of an entity, usable inside queries.</summary>
    Expression<Func<TDomainObject, TypedSecurityIdentity>> SecurityIdentityExpression { get; }

    /// <summary>Returns the identifier of the entity.</summary>
    /// <param name="domainObject">The entity.</param>
    TypedSecurityIdentity GetIdentity(TDomainObject domainObject);

    /// <summary>Sets the identifier of the entity.</summary>
    /// <param name="domainObject">The entity.</param>
    /// <param name="securityIdentity">The identifier to set.</param>
    void SetIdentity(TDomainObject domainObject, SecurityIdentity securityIdentity);
}