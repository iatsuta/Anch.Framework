using System.Collections.Immutable;

using Anch.SecuritySystem.UserSource;

namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IPrincipalSourceServiceBase
{
    IAsyncEnumerable<ManagedPrincipalHeader> GetPrincipalsAsync(string nameFilter, int limit);

    Task<ManagedPrincipal?> TryGetPrincipalAsync(UserCredential userCredential, CancellationToken cancellationToken);

    async Task<ManagedPrincipal> GetPrincipalAsync(UserCredential userCredential, CancellationToken cancellationToken) =>

        await this.TryGetPrincipalAsync(userCredential, cancellationToken)

        ?? throw new UserSourceException($"Principal with {nameof(userCredential)} '{userCredential}' not found");

    IAsyncEnumerable<string> GetLinkedPrincipalsAsync(ImmutableHashSet<SecurityRole> securityRoles);
}