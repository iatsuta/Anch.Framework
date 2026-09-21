using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

public interface IGenericQueryableExtension
{
    public void AddServices(IServiceCollection services);
}