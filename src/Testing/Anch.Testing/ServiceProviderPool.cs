using Anch.Core;
using Anch.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class ServiceProviderPool(ITestEnvironment testEnvironment, bool? allowParallelization) : IServiceProviderPool
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
                    var serviceProviderBuildContext = ServiceProviderBuildContext.Main;

                    var services = new ServiceCollection()
                        .AddKeyedSingleton<IServiceProvider>(ITestEnvironment.MainServiceProviderKey, (sp, _) => sp)
                        .AddSingleton(serviceProviderBuildContext.Index)
                        .AddSingleton<IParallelizationSettings, ParallelizationSettings>();

                    if (allowParallelization != null)
                    {
                        services.AddSingleton(new AllowParallelizationConstraint(allowParallelization.Value));
                    }

                    var preMainServiceProvider = testEnvironment.BuildServiceProvider(services, serviceProviderBuildContext);

                    foreach (var initializer in preMainServiceProvider.GetKeyedServices<IInitializer>(ITestEnvironment.MainServiceProviderKey))
                    {
                        await initializer.Initialize(ct);
                    }

                    var mainServiceProviderSettings = preMainServiceProvider.GetService<IMainServiceProviderSettings>();

                    this.internalServiceProviderPool = new InternalServiceProviderPool(
                        testEnvironment,
                        preMainServiceProvider,
                        preMainServiceProvider.GetRequiredService<IParallelizationSettings>(),
                        mainServiceProviderSettings?.ReturnToPool ?? true);
                }
            }
        }

        return this.internalServiceProviderPool;
    }

    public async ValueTask DisposeAsync()
    {
        using (this.asyncLocker)
        {
            await using (this.internalServiceProviderPool) ;
        }
    }
}