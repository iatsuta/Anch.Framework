using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Services;

/// <summary>Converts a principal entity into a managed principal.</summary>
public interface IManagedPrincipalConverter<in TPrincipal>
{
    Task<ManagedPrincipal> ToManagedPrincipalAsync(TPrincipal dbPrincipal, CancellationToken ct);
}