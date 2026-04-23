using CommonFramework.DependencyInjection;
using CommonFramework.Testing;

using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.IntegrationTests.Environment;

public abstract class TestEnvironment : ITestEnvironment
{
    public virtual IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        services
            .AddSingleton<IMainConnectionStringSource, MainConnectionStringSource>()
            .AddSingleton<IGenericQueryableSetupConfigurator, GenericQueryableSetupConfigurator>()
            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
}