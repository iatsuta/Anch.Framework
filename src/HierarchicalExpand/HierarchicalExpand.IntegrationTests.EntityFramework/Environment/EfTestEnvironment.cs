using CommonFramework.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

[assembly: CommonFramework.Testing.CommonTestFramework<HierarchicalExpand.IntegrationTests.Environment.EfTestEnvironment>]

namespace HierarchicalExpand.IntegrationTests.Environment;

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