using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEnvironmentHook(EnvironmentHookType type, Action<IServiceProvider> action) =>
            services.AddEnvironmentHook(type, async (sp, _) => action(sp));

        public IServiceCollection AddEnvironmentHook(EnvironmentHookType type, Func<IServiceProvider, CancellationToken, ValueTask> action) =>
            services.AddKeyedSingleton<ITestEnvironmentHook>(type, (sp, _) => new AsyncTestEnvironmentHook(sp, action));
    }

    private class AsyncTestEnvironmentHook(
        IServiceProvider serviceProvider,
        Func<IServiceProvider, CancellationToken, ValueTask> action) : ITestEnvironmentHook
    {
        public ValueTask Process(CancellationToken ct) => action(serviceProvider, ct);
    }
}