using CommonFramework.DependencyInjection;
using CommonFramework.Testing;

using Microsoft.Extensions.DependencyInjection;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerClass)]
[assembly: CommonTestFramework<CommonFramework.Tests.Initialization.TestFrameworkInitializer>]

namespace CommonFramework.Tests.Initialization;

public class TestFrameworkInitializer : ICommonTestFrameworkInitializer
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return services
            .AddValidator<DuplicateServiceUsageValidator>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }
}