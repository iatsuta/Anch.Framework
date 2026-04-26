namespace Anch.DependencyInjection.ServiceProxy;

public interface INativeActivator
{
    TService Create<TService>(Type instanceServiceType, params object[] args);
}