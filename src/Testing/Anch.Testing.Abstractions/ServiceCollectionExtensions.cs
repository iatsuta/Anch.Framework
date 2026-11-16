using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

/// <summary>Registration of environment hooks.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers a hook class. Hooks of type <see cref="EnvironmentHookType.Before"/> run in reverse registration order, <see cref="EnvironmentHookType.After"/> in registration order.</summary>
        /// <param name="type">When the hook runs.</param>
        /// <typeparam name="TTestEnvironmentHook">The hook type.</typeparam>
        public IServiceCollection AddEnvironmentHook<TTestEnvironmentHook>(EnvironmentHookType type)
            where TTestEnvironmentHook : class, ITestEnvironmentHook =>

            services.AddKeyedSingleton<ITestEnvironmentHook, TTestEnvironmentHook>(type);

        /// <summary>Registers a synchronous hook.</summary>
        /// <param name="type">When the hook runs.</param>
        /// <param name="action">The code to run; receives the container of the test.</param>
        public IServiceCollection AddEnvironmentHook(EnvironmentHookType type, Action<IServiceProvider> action) =>
            services.AddEnvironmentHook(type, async (sp, _) => action(sp));

        /// <summary>Registers an asynchronous hook.</summary>
        /// <param name="type">When the hook runs.</param>
        /// <param name="action">The code to run; receives the container of the test and a cancellation token.</param>
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