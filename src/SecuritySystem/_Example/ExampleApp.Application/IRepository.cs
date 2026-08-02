namespace ExampleApp.Application;

public interface IRepository<TDomainObject>
{
    Task SaveAsync(TDomainObject domainObject, CancellationToken ct);

    Task RemoveAsync(TDomainObject domainObject, CancellationToken ct);

    IQueryable<TDomainObject> GetQueryable();
}