using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.Database.DependencyInjection;

public interface IDatabaseTestingProvider
{
    void AddServices(IServiceCollection services);
}