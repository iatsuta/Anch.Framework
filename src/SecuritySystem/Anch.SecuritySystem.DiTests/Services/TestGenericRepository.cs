using Anch.GenericRepository;

namespace Anch.SecuritySystem.DiTests.Services;

public class TestGenericRepository : IGenericRepository
{
    public Task SaveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct) where TDomainObject : class
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct) where TDomainObject : class
    {
        throw new NotImplementedException();
    }
}