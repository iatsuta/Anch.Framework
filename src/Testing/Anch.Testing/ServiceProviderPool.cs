using Anch.Core;
using Anch.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class ServiceProviderPool(ITestEnvironment testEnvironment) : IServiceProviderPool
{
    private readonly IAsyncLocker asyncLocker = new AsyncLocker();

    private IServiceProviderPool? internalServiceProviderPool;

    public async ValueTask<IServiceProvider> GetAsync(CancellationToken ct)
    {
        var v = await this.GetInternalServiceProviderPool(ct);

        return await v.GetAsync(ct);
    }

    public async ValueTask ReleaseAsync(IServiceProvider serviceProvider, CancellationToken ct)
    {
        var v = await this.GetInternalServiceProviderPool(ct);

        await v.ReleaseAsync(serviceProvider, ct);
    }

    private async ValueTask<IServiceProviderPool> GetInternalServiceProviderPool(CancellationToken ct)
    {
        if (this.internalServiceProviderPool == null)
        {
            using (await this.asyncLocker.CreateScope(ct))
            {
                if (this.internalServiceProviderPool == null)
                {
                    var serviceProviderIndex = ServiceProviderIndex.Main;

                    var services = new ServiceCollection()
                        .AddKeyedSingleton<IServiceProvider>(IServiceProviderPool.MainServiceProviderKey, (sp, _) => sp)
                        .AddSingleton(serviceProviderIndex);

                    var preMainServiceProvider = testEnvironment.BuildServiceProvider(services, serviceProviderIndex);

                    foreach (var initializer in preMainServiceProvider.GetKeyedServices<IInitializer>(IServiceProviderPool.MainServiceProviderKey))
                    {
                        await initializer.Initialize(ct);
                    }

                    var mainServiceProviderSettings = preMainServiceProvider.GetService<MainServiceProviderSettings>() ?? MainServiceProviderSettings.Default;

                    this.internalServiceProviderPool = new InternalServiceProviderPool(testEnvironment, preMainServiceProvider, mainServiceProviderSettings);
                }
            }
        }

        return this.internalServiceProviderPool;
    }
}