using System.Collections.Immutable;

using Anch.SecuritySystem.UserSource;

namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IPrincipalSourceServiceBase
{
    IAsyncEnumerable<ManagedPrincipalHeader> GetPrincipalsAsync(string nameFilter, int limit);

    Task<ManagedPrincipal?> TryGetPrincipalAsync(UserCredential userCredential, CancellationToken ct);

    async Task<ManagedPrincipal> GetPrincipalAsync(UserCredential userCredential, CancellationToken ct) =>

        await this.TryGetPrincipalAsync(userCredential, ct)

        ?? throw new UserSourceException($"Principal with {nameof(userCredential)} '{userCredential}' not found");

    IAsyncEnumerable<string> GetLinkedPrincipalsAsync(ImmutableHashSet<SecurityRole> securityRoles);
}