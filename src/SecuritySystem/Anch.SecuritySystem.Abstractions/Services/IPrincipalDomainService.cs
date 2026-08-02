namespace Anch.SecuritySystem.Services;

public interface IPrincipalDomainService<TPrincipal>
{
    Task<TPrincipal> GetOrCreateAsync(UserCredential userCredential, CancellationToken ct);

    Task RemoveAsync(TPrincipal principal, bool force, CancellationToken ct);
}
