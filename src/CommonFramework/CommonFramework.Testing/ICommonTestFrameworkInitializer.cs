using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing;

public interface ICommonTestFrameworkInitializer
{
    IServiceCollection CreateServiceCollection() => new ServiceCollection();

    IServiceProvider BuildServiceProvider(IServiceCollection serviceCollection);
}