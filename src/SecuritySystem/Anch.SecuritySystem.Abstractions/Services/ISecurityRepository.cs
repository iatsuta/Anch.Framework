namespace Anch.SecuritySystem.Services;

/// <summary>Loads entities for the security system.</summary>
/// <typeparam name="TDomainObject">The entity type.</typeparam>
public interface ISecurityRepository<TDomainObject>
    where TDomainObject : class
{
    /// <summary>Loads an entity by its identifier.</summary>
    /// <param name="securityIdentity">The identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<TDomainObject> GetObjectAsync(SecurityIdentity securityIdentity, CancellationToken ct);
}