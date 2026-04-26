using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.DependencyInjection;

public interface IDatabaseTestingProvider
{
    void AddServices(IServiceCollection services);
}