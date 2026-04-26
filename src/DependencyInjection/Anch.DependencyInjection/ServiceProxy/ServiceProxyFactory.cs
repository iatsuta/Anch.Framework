using Anch.Core;

namespace Anch.DependencyInjection.ServiceProxy;

public class ServiceProxyFactory(INativeActivator nativeActivator, IServiceProxyTypeRedirector redirector) : IServiceProxyFactory
{
    public virtual TService Create<TService>(Type instanceServiceType, object[] args)
    {
        var realInstanceServiceType = redirector.TryRedirect(instanceServiceType) ?? instanceServiceType;

        return nativeActivator.Create<TService>(realInstanceServiceType, args);
    }
}