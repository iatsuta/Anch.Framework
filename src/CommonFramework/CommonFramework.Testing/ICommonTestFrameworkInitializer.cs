using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing;

public interface ICommonTestFrameworkInitializer
{
    IServiceProvider BuildServiceProvider(IServiceCollection services);
}