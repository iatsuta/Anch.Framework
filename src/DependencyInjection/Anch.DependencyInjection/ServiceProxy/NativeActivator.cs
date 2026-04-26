using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection.ServiceProxy;

public class NativeActivator(IServiceProvider serviceProvider) : INativeActivator
{
    public virtual TService Create<TService>(Type instanceServiceType, object[] args)
    {
        return (TService)ActivatorUtilities.CreateInstance(serviceProvider, instanceServiceType, args);
    }
}