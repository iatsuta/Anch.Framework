using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.DependencyInjection;

/// <summary>Registers the services of a particular database engine for tests.</summary>
public interface IDatabaseTestingProvider
{
    void AddServices(IServiceCollection services);
}