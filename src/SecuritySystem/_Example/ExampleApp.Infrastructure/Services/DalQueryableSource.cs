using Anch.GenericRepository;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Infrastructure.Services;

public class DalQueryableSource(IServiceProvider serviceProvider) : IQueryableSource
{
    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        return serviceProvider.GetRequiredService<IDal<TDomainObject>>().GetQueryable();
    }
}