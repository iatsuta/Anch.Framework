using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Services;

public interface IManagedPrincipalConverter<in TPrincipal>
{
	ValueTask<ManagedPrincipal> ToManagedPrincipalAsync(TPrincipal dbPrincipal, CancellationToken cancellationToken);
}