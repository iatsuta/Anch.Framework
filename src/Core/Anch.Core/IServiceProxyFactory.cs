namespace Anch.Core;

/// <summary>
/// Creates a service instance with container-supplied dependencies, allowing the implementation type to be redirected by configuration.
/// Used where the concrete type depends on the domain type, for example a different denormalizer for a particular entity.
/// </summary>
public interface IServiceProxyFactory
{
    /// <summary>Creates an instance of the service type itself.</summary>
    /// <param name="args">Explicit constructor arguments.</param>
    /// <typeparam name="TService">The service and implementation type.</typeparam>
    TService Create<TService>(params object[] args) => this.Create<TService, TService>(args);

    /// <summary>Creates an instance of an implementation type, returned as the service type.</summary>
    /// <param name="args">Explicit constructor arguments.</param>
    /// <typeparam name="TService">The service type to return.</typeparam>
    /// <typeparam name="TServiceImpl">The implementation type to create (before redirection).</typeparam>
    TService Create<TService, TServiceImpl>(params object[] args)
        where TServiceImpl : TService
        => this.Create<TService>(typeof(TServiceImpl), args);

    /// <summary>Creates an instance of the given type, after applying any configured redirect.</summary>
    /// <param name="instanceServiceType">The type to create.</param>
    /// <param name="args">Explicit constructor arguments.</param>
    /// <typeparam name="TService">The service type to return.</typeparam>
    TService Create<TService>(Type instanceServiceType, params object[] args);
}