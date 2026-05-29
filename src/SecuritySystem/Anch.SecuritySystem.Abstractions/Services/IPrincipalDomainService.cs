namespace Anch.SecuritySystem.Services;

public interface IPrincipalDomainService<TPrincipal>
{
    Task<TPrincipal> GetOrCreateAsync(UserCredential userCredential, CancellationToken cancellationToken = default);

    Task RemoveAsync(TPrincipal principal, bool force = false, CancellationToken cancellationToken = default);
}
