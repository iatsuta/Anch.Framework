using Anch.Testing.Tests;
using Anch.Testing.Xunit;

using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<TestEnvironment>]

namespace Anch.Testing.Tests;

public class TestEnvironment : ITestEnvironment
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services, ServiceProviderBuildContext buildContext)
    {
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }
}