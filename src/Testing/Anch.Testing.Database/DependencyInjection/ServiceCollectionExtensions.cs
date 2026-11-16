using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.DependencyInjection;

/// <summary>Registration of database testing.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseTesting(this IServiceCollection services, Action<IDatabaseTestingSetup> setupAction) =>
        services.Initialize<DatabaseTestingSetup>(setupAction);
}