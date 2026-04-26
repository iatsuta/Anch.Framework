using Anch.GenericRepository;
using Anch.HierarchicalExpand.IntegrationTests.Environment;
using Anch.Testing;
using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<EfTestEnvironment>]

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class EfTestEnvironment : TestEnvironment
{
    protected override IServiceCollection InitializeServices(IServiceCollection services) =>

        services
            .AddDbContext<AppDbContext>()
            .AddScoped<EfAutoCommitSession>()
            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>()

            .AddSingleton<IEmptySchemaInitializer, EfEmptySchemaInitializer>();
}