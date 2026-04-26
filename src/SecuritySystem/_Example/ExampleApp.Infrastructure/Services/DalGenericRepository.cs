using Anch.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Infrastructure.Services;

public class DalGenericRepository(IServiceProvider serviceProvider) : IGenericRepository
{
    public Task SaveAsync<TDomainObject>(TDomainObject data, CancellationToken cancellationToken)
        where TDomainObject : class
    {
        return serviceProvider.GetRequiredService<IDal<TDomainObject>>().SaveAsync(data, cancellationToken);
    }

    public Task RemoveAsync<TDomainObject>(TDomainObject data, CancellationToken cancellationToken)
        where TDomainObject : class
    {
        return serviceProvider.GetRequiredService<IDal<TDomainObject>>().RemoveAsync(data, cancellationToken);
    }
}