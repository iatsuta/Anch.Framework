using Anch.GenericQueryable.IntegrationTests.Environment;
using Anch.GenericRepository;
using Anch.Testing;
using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<EfTestEnvironment>]

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class EfTestEnvironment : TestEnvironment
{
    protected override IServiceCollection AddServices(IServiceCollection services) =>

        services
            .AddDbContext<TestDbContext>()
            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>()

            .AddSingleton<IEmptySchemaInitializer, EfEmptySchemaInitializer>();
}