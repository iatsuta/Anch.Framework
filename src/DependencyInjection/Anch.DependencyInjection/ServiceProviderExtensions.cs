using Anch.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static IServiceProxyFactory GetServiceProxyFactory(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IServiceProxyFactory>();
}