using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class ServiceProviderPool(ITestEnvironment testEnvironment, bool? allowParallelization) : IServiceProviderPool
{
    private readonly IServiceProviderPool internalServiceProviderPool = CreateInternal(testEnvironment, allowParallelization);

    public ValueTask<IServiceProvider> GetAsync(CancellationToken ct) => this.internalServiceProviderPool.GetAsync(ct);

    public ValueTask ReleaseAsync(IServiceProvider serviceProvider, CancellationToken ct) => this.internalServiceProviderPool.ReleaseAsync(serviceProvider, ct);

    public ValueTask DisposeAsync() => this.internalServiceProviderPool.DisposeAsync();

    private static IServiceProviderPool CreateInternal(ITestEnvironment testEnvironment, bool? allowParallelization)
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

        var mainServiceProvider = testEnvironment.BuildServiceProvider(services, serviceProviderBuildContext);

        var mainServiceProviderSettings = mainServiceProvider.GetService<IMainServiceProviderSettings>();

        return new InternalServiceProviderPool(
            testEnvironment,
            mainServiceProvider,
            mainServiceProvider.GetRequiredService<IParallelizationSettings>(),
            mainServiceProviderSettings?.ReturnToPool ?? true);
    }
}