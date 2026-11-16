using System.Collections.Immutable;

using Anch.SecuritySystem.UserSource;

namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>Reads principals and their permissions from a permission source.</summary>
public interface IPrincipalSourceServiceBase
{
    /// <summary>Searches principals by name.</summary>
    /// <param name="nameFilter">Text the name must contain.</param>
    /// <param name="limit">The maximum number of principals to return.</param>
    IAsyncEnumerable<ManagedPrincipalHeader> GetPrincipalsAsync(string nameFilter, int limit);

    /// <summary>Finds a principal with its permissions.</summary>
    /// <param name="userCredential">The principal.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The principal, or <c>null</c> if it does not exist.</returns>
    Task<ManagedPrincipal?> TryGetPrincipalAsync(UserCredential userCredential, CancellationToken ct);

    /// <summary>Finds a principal with its permissions; throws if it does not exist.</summary>
    /// <param name="userCredential">The principal.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="UserSource.UserSourceException">The principal does not exist.</exception>
    async Task<ManagedPrincipal> GetPrincipalAsync(UserCredential userCredential, CancellationToken ct) =>

        await this.TryGetPrincipalAsync(userCredential, ct)

        ?? throw new UserSourceException($"Principal with {nameof(userCredential)} '{userCredential}' not found");

    /// <summary>Returns the names of the principals that have any of the given roles.</summary>
    /// <param name="securityRoles">The roles to look for.</param>
    IAsyncEnumerable<string> GetLinkedPrincipalsAsync(ImmutableHashSet<SecurityRole> securityRoles);
}