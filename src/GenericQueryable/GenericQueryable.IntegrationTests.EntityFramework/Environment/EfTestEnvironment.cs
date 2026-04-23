using CommonFramework;
using CommonFramework.GenericRepository;

using GenericQueryable.IntegrationTests.Environment;

using Microsoft.Extensions.DependencyInjection;

#if DEBUG
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif

[assembly: CommonFramework.Testing.CommonTestFramework<EfTestEnvironment>]

namespace GenericQueryable.IntegrationTests.Environment;

public class EfTestEnvironment : TestEnvironment
{
    protected override IServiceCollection AddServices(IServiceCollection services) =>

        services
            .AddDbContext<TestDbContext>()
            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>()
            .AddScoped<IDbSchemaInitializer, EfSchemaInitializer>();
}