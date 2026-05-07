using Anch.GenericRepository;
using Anch.Testing.Xunit;
using Anch.Workflow.IntegrationTests.Environment;

using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<EfTestEnvironment>]

namespace Anch.Workflow.IntegrationTests.Environment;

public class EfTestEnvironment : TestEnvironment
{
    protected override IServiceCollection AddServices(IServiceCollection services) =>

        services
            .AddDbContext<TestDbContext>()
            .AddScoped<EfAutoCommitSession>()
            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>()

            .AddSingleton<IEmptySchemaInitializer, EfEmptySchemaInitializer>();
}