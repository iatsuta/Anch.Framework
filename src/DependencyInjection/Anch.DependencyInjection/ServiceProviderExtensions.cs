using Anch.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection;

/// <summary>Extensions for <see cref="IServiceProvider"/>.</summary>
public static class ServiceProviderExtensions
{
    /// <summary>Returns the registered service proxy factory.</summary>
    /// <param name="serviceProvider">The service provider.</param>
    public static IServiceProxyFactory GetServiceProxyFactory(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IServiceProxyFactory>();
}