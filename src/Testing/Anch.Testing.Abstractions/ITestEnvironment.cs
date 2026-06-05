using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public interface ITestEnvironment
{
    public const string MainServiceProviderKey = "ServiceProviderPool.Main";

    public const string PooledServiceProviderKey = "ServiceProviderPool.Pooled";

    IServiceProvider BuildServiceProvider(IServiceCollection services, ServiceProviderBuildContext buildContext);
}