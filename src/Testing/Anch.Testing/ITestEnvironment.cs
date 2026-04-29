using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public interface ITestEnvironment
{
    bool AllowParallelization => true;

    IServiceProvider BuildServiceProvider(IServiceCollection services);
}