using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing;

public interface ITestEnvironment
{
    void Reset(IServiceProvider serviceProvider);

    IServiceProvider Build(IServiceCollection services);
}