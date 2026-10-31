namespace Anch.DependencyInjection.ServiceProxy;

/// <summary>Creates objects through the container-aware activator (constructor injection plus explicit arguments).</summary>
public interface INativeActivator
{
    /// <summary>Creates an instance, resolving constructor parameters from the container and the explicit arguments.</summary>
    /// <param name="instanceServiceType">The concrete type to create.</param>
    /// <param name="args">Arguments for constructor parameters that the container cannot supply.</param>
    /// <typeparam name="TService">The type the instance is returned as.</typeparam>
    TService Create<TService>(Type instanceServiceType, params object[] args);
}