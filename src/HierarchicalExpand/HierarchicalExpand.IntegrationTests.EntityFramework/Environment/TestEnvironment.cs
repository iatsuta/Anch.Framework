using CommonFramework.GenericRepository;

using GenericQueryable.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#if DEBUG
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif

[assembly: CommonFramework.Testing.CommonTestFramework<HierarchicalExpand.IntegrationTests.Environment.TestEnvironment>]

namespace HierarchicalExpand.IntegrationTests.Environment;

public class TestEnvironment : TestEnvironmentBase
{
    protected override IServiceCollection InitializeServices(IServiceCollection services) =>

        services
            .AddDbContext<AppDbContext>(optionsBuilder => optionsBuilder
                .UseSqlite("Data Source=test.db")
                .UseLazyLoadingProxies()
                .UseGenericQueryable())
            .AddScoped<AutoCommitSession>()
            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>()

            .AddScoped<IDbSchemaInitializer, DbSchemaInitializer>();
}