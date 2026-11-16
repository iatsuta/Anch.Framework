using Anch.Core;

namespace Anch.DependencyInjection.ServiceProxy;

/// <summary>Default <see cref="IServiceProxyFactory"/>: applies the configured redirects and then creates the object with the native activator.</summary>
/// <param name="nativeActivator">Creates the object.</param>
/// <param name="redirector">Finds the redirect target of the requested type.</param>
public class ServiceProxyFactory(INativeActivator nativeActivator, IServiceProxyTypeRedirector redirector) : IServiceProxyFactory
{
    /// <inheritdoc/>
    public virtual TService Create<TService>(Type instanceServiceType, object[] args)
    {
        var realInstanceServiceType = redirector.TryRedirect(instanceServiceType) ?? instanceServiceType;

        return nativeActivator.Create<TService>(realInstanceServiceType, args);
    }
}