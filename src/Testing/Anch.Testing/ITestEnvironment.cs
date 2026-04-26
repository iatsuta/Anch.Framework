using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public interface ITestEnvironment
{
    IServiceProvider BuildServiceProvider(IServiceCollection services);
}