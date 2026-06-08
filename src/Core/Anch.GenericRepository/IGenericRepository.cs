namespace Anch.GenericRepository;

public interface IGenericRepository
{
    Task SaveAsync<TDomainObject>(TDomainObject data, CancellationToken ct)
        where TDomainObject : class;

    Task RemoveAsync<TDomainObject>(TDomainObject data, CancellationToken ct)
        where TDomainObject : class;
}