using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class ServiceProviderPool(ITestEnvironment testEnvironment, bool? allowParallelization) : IServiceProviderPool
{
    private readonly Lazy<IServiceProviderPool> lazyInternalServiceProviderPool = new(() =>
    {
        var serviceProviderBuildContext = ServiceProviderBuildContext.Main;

        var services = new ServiceCollection()
            .AddKeyedSingleton<IServiceProvider>(ITestEnvironment.MainServiceProviderKey, (sp, _) => sp)
            .AddSingleton(serviceProviderBuildContext.Index)
            .AddSingleton<IParallelizationSettings, ParallelizationSettings>();

        if (allowParallelization is not null)
        {
            services.AddSingleton(new AllowParallelizationConstraint(allowParallelization.Value));
        }

        var mainServiceProvider = testEnvironment.BuildServiceProvider(services, serviceProviderBuildContext);

        var mainServiceProviderSettings = mainServiceProvider.GetService<IMainServiceProviderSettings>();

        return new InternalServiceProviderPool(
            testEnvironment,
            mainServiceProvider,
            mainServiceProvider.GetRequiredService<IParallelizationSettings>(),
            mainServiceProviderSettings?.ReturnToPool ?? true);
    });

    public ValueTask<IServiceProvider> GetAsync(CancellationToken ct) => this.lazyInternalServiceProviderPool.Value.GetAsync(ct);

    public ValueTask ReleaseAsync(IServiceProvider serviceProvider, CancellationToken ct) =>
        this.lazyInternalServiceProviderPool.Value.ReleaseAsync(serviceProvider, ct);

    public async ValueTask DisposeAsync()
    {
        if (this.lazyInternalServiceProviderPool.IsValueCreated)
        {
            await this.lazyInternalServiceProviderPool.Value.DisposeAsync();
        }
    }
}