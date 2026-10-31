namespace Anch.SecuritySystem.Services;

/// <summary>Creates and removes principal entities.</summary>
/// <typeparam name="TPrincipal">The principal entity type.</typeparam>
public interface IPrincipalDomainService<TPrincipal>
{
    /// <summary>Returns the principal of the user, creating it if it does not exist.</summary>
    /// <param name="userCredential">The user.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<TPrincipal> GetOrCreateAsync(UserCredential userCredential, CancellationToken ct);

    /// <summary>Removes a principal.</summary>
    /// <param name="principal">The principal.</param>
    /// <param name="force">Remove even if the principal still has permissions.</param>
    /// <param name="ct">Cancellation token.</param>
    Task RemoveAsync(TPrincipal principal, bool force, CancellationToken ct);
}
