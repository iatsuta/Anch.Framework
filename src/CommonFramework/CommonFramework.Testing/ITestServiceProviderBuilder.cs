using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing;

public interface ITestServiceProviderBuilder
{
    IServiceProvider Build(IServiceCollection services);
}