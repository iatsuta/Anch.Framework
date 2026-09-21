using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

public class GenericQueryableExtension(Action<IServiceCollection> setupAction) : IGenericQueryableExtension
{
    public void AddServices(IServiceCollection services) => setupAction(services);
}