using Anch.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.DependencyInjection.ServiceProxy;

public class ServiceProxySetup : IServiceProxySetup, IServiceInitializer
{
    private readonly List<Action<IServiceCollection>> registerRedirects = [];

    public IServiceProxySetup SetRedirect(Type sourceType, Func<IServiceProvider, Type> targetTypeSelector, bool replace)
    {
        this.registerRedirects.Add(sc => sc.AddSingleton(sp =>
            new ServiceProxyTypeRedirectInfo(sourceType, targetTypeSelector(sp)) { Replace = replace, IsBinder = false }));

        return this;
    }

    public IServiceProxySetup BindRedirect(Type sourceType, Type binderType, bool replace)
    {
        if (!typeof(IServiceProxyBinder).IsAssignableFrom(binderType)
            || sourceType.IsGenericTypeDefinition != binderType.IsGenericTypeDefinition
            || (sourceType.IsGenericTypeDefinition && sourceType.GetGenericArguments().Length != binderType.GetGenericArguments().Length))
        {
            throw new ArgumentOutOfRangeException(nameof(binderType));
        }

        this.registerRedirects.Add(sc =>
        {
            sc.TryAddSingleton(binderType);

            sc.AddSingleton(new ServiceProxyTypeRedirectInfo(sourceType, binderType) { Replace = replace, IsBinder = true });
        });

        return this;
    }

    public void Initialize(IServiceCollection services)
    {
        services.TryAddTransient<INativeActivator, NativeActivator>();
        services.TryAddTransient<IServiceProxyFactory, ServiceProxyFactory>();
        services.TryAddSingleton<IServiceProxyTypeRedirector, ServiceProxyTypeRedirector>();

        foreach (var registerAction in this.registerRedirects)
        {
            registerAction(services);
        }
    }
}