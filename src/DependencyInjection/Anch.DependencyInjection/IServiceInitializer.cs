using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection;

/// <summary>Applies collected module settings to a container. A module's setup class implements it: its fluent methods only accumulate settings, and <c>Initialize</c> turns them into registrations.</summary>
/// <typeparam name="TServiceContainer">The container type, usually <see cref="IServiceCollection"/>.</typeparam>
public interface IServiceInitializer<in TServiceContainer>
{
    /// <summary>Registers the services described by the collected settings.</summary>
    /// <param name="services">The container to register in.</param>
    void Initialize(TServiceContainer services);
}

/// <summary>An initializer for <see cref="IServiceCollection"/>.</summary>
public interface IServiceInitializer : IServiceInitializer<IServiceCollection>;