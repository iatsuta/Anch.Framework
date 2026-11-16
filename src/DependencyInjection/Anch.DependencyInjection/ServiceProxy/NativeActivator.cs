using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection.ServiceProxy;

/// <summary>Creates objects with <c>ActivatorUtilities</c>: constructor parameters come from the container and from explicit arguments.</summary>
public class NativeActivator(IServiceProvider serviceProvider) : INativeActivator
{
    public virtual TService Create<TService>(Type instanceServiceType, object[] args)
    {
        return (TService)ActivatorUtilities.CreateInstance(serviceProvider, instanceServiceType, args);
    }
}