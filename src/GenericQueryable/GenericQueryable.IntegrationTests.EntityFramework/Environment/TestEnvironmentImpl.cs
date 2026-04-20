using CommonFramework;
using CommonFramework.GenericRepository;

using GenericQueryable.EntityFramework;
using GenericQueryable.IntegrationTests.Environment;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: CommonFramework.Testing.CommonTestFramework<TestServiceProviderBuilder>]

namespace GenericQueryable.IntegrationTests.Environment;

public class TestServiceProviderBuilder : TestServiceProviderBuilderBase
{
    public override IServiceProvider Build(IServiceCollection services) =>

        services

            .AddDbContext<TestDbContext>(optionsBuilder => optionsBuilder
                .UseSqlite("Data Source=test.db")
                .UseGenericQueryable(SetupGenericQueryable))

            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>()

            .AddScoped<IDbSchemeInitializer, DbSchemeInitializer>()

            .Pipe(base.Build);
}