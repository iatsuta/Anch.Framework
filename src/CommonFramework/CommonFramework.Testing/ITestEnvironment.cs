using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing;

public interface ITestEnvironment
{
    IServiceProvider BuildServiceProvider(IServiceCollection services);
}