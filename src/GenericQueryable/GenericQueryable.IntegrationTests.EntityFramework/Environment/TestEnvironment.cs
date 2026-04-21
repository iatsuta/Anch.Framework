using CommonFramework;
using CommonFramework.GenericRepository;

using GenericQueryable.EntityFramework;
using GenericQueryable.IntegrationTests.Environment;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#if DEBUG
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif

[assembly: CommonFramework.Testing.CommonTestFramework<TestEnvironment>]

namespace GenericQueryable.IntegrationTests.Environment;

public class TestEnvironment : TestEnvironmentBase
{
    public override IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        services

            .AddDbContext<TestDbContext>(optionsBuilder => optionsBuilder
                .UseSqlite("Data Source=test.db")
                .UseGenericQueryable(SetupGenericQueryable))

            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>()

            .AddScoped<IDbSchemaInitializer, DbSchemaInitializer>()

            .Pipe(base.BuildServiceProvider);
}