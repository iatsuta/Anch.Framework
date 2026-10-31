using Anch.Core;
using Anch.DependencyInjection.ServiceProxy;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.DependencyInjection;

/// <summary>
/// Helpers for registering services: the setup pattern (<c>Initialize</c>), registration of a service as a projection of another one (<c>AddXxxFrom</c>),
/// service proxy registration (<c>AddXxxServiceProxy</c>) and replacement of registrations (<c>ReplaceXxx</c>).
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Adds the service proxy factory, which creates services whose implementation type can be redirected by configuration.</summary>
        /// <param name="setup">Optional redirect configuration.</param>
        public IServiceCollection AddServiceProxyFactory(Action<IServiceProxySetup>? setup = null) =>
            services.Initialize<ServiceProxySetup>(setup);

        /// <summary>Determines whether a service of the given lifetime is already registered.</summary>
        /// <param name="lifetime">The lifetime to look for.</param>
        /// <param name="isKeyed">Whether to look among keyed registrations.</param>
        /// <typeparam name="TService">The service type.</typeparam>
        public bool AlreadyInitialized<TService>(ServiceLifetime lifetime = ServiceLifetime.Singleton, bool isKeyed = false) =>

            services.Any(sd => sd.IsKeyedService == isKeyed && sd.Lifetime == lifetime && sd.ServiceType == typeof(TService));

        /// <summary>Determines whether the service is already registered with the given implementation type and lifetime.</summary>
        /// <param name="lifetime">The lifetime to look for.</param>
        /// <param name="isKeyed">Whether to look among keyed registrations.</param>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public bool AlreadyInitialized<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton, bool isKeyed = false)

            where TImplementation : TService =>

            services.Any(sd =>
                sd.IsKeyedService == isKeyed && sd.Lifetime == lifetime && sd.ServiceType == typeof(TService) &&
                sd.ImplementationType == typeof(TImplementation));

        /// <summary>Creates the module's setup, lets the caller configure it and applies it to the collection. This is how every <c>AddXxx(setup)</c> method is built.</summary>
        /// <param name="setup">Configures the setup; may be <c>null</c>.</param>
        /// <typeparam name="TServiceInitializer">The setup type.</typeparam>
        public IServiceCollection Initialize<TServiceInitializer>(Action<TServiceInitializer>? setup)
            where TServiceInitializer : IServiceInitializer<IServiceCollection>, new() =>
            services.Initialize<IServiceCollection, TServiceInitializer>(setup);

        /// <summary>Redirects a service type to the implementation type chosen by a binder.</summary>
        /// <param name="serviceType">The service type to redirect.</param>
        /// <param name="binderType">An <c>IServiceProxyBinder</c> that computes the target type.</param>
        /// <param name="replace">Whether the redirect overrides earlier ones.</param>
        public IServiceCollection BindServiceProxy(Type serviceType, Type binderType, bool replace = false)
        {
            return services.AddServiceProxyFactory(b => b.BindRedirect(serviceType, binderType, replace));
        }
    }

    extension<TService>(TService service)
    {
        /// <summary>Creates an initializer, lets the caller configure it and applies it to the container; returns the container.</summary>
        /// <param name="setup">Configures the initializer; may be <c>null</c>.</param>
        /// <typeparam name="TServiceInitializer">The initializer type.</typeparam>
        public TService Initialize<TServiceInitializer>(Action<TServiceInitializer>? setup)
            where TServiceInitializer : IServiceInitializer<TService>, new() =>
            service.Initialize(new TServiceInitializer(), setup);

        /// <summary>Configures the given initializer and applies it to the container; returns the container.</summary>
        /// <param name="initializer">The initializer.</param>
        /// <param name="setup">Configures the initializer; may be <c>null</c>.</param>
        /// <typeparam name="TServiceInitializer">The initializer type.</typeparam>
        public TService Initialize<TServiceInitializer>(TServiceInitializer initializer, Action<TServiceInitializer>? setup)
            where TServiceInitializer : IServiceInitializer<TService>
        {
            setup?.Invoke(initializer);

            initializer.Initialize(service);

            return service;
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>Registers a scoped service as a projection of another registered service.</summary>
        /// <param name="selector">Derives the service from the source service.</param>
        /// <typeparam name="TService">The service to register.</typeparam>
        /// <typeparam name="TSource">The registered service it is taken from.</typeparam>
        public IServiceCollection AddScopedFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.AddScoped(sp => selector(sp.GetRequiredService<TSource>()));
        }

        /// <summary>Registers a scoped service as an alias of another registered service.</summary>
        /// <typeparam name="TService">The service to register.</typeparam>
        /// <typeparam name="TServiceImplementation">The registered service it resolves to.</typeparam>
        public IServiceCollection AddScopedFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.AddScopedFrom<TService, TServiceImplementation>(v => v);
        }

        /// <summary>Registers a scoped service created from the service provider.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to register.</typeparam>
        public IServiceCollection AddScopedFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.AddScopedFrom<TService, IServiceProvider>(selector);
        }

        /// <summary>Registers a scoped service created by the service proxy factory.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to register.</typeparam>
        public IServiceCollection AddScopedFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.AddScopedFrom<TService, IServiceProxyFactory>(selector);
        }


        /// <summary>Registers a scoped service that is created through the proxy factory; the implementation type is chosen at run time.</summary>
        /// <param name="targetTypeSelector">Chooses the implementation type.</param>
        /// <param name="replace">Replace an existing registration and redirect.</param>
        /// <typeparam name="TService">The service type.</typeparam>
        public IServiceCollection AddScopedServiceProxy<TService>(Func<IServiceProvider, Type> targetTypeSelector, bool replace = false)
            where TService : class
        {
            services.AddServiceProxyFactory(b => b.SetRedirect(typeof(TService), targetTypeSelector, replace));

            var init = (IServiceProxyFactory spf) => spf.Create<TService>();

            return replace ? services.ReplaceScopedFrom(init) : services.AddScopedFrom(init);
        }

        /// <summary>Registers a scoped service that is created through the proxy factory with the given implementation type.</summary>
        /// <param name="targetType">The implementation type.</param>
        /// <param name="replace">Replace an existing registration and redirect.</param>
        /// <typeparam name="TService">The service type.</typeparam>
        public IServiceCollection AddScopedServiceProxy<TService>(Type targetType, bool replace = false)
            where TService : class => services.AddScopedServiceProxy<TService>(_ => targetType, replace);

        /// <summary>Registers a scoped service that is created through the proxy factory with the given implementation type.</summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public IServiceCollection AddScopedServiceProxy<TService, TImplementation>()
            where TService : class
            where TImplementation : TService =>
            services.AddScopedServiceProxy<TService>(typeof(TImplementation));
    }

    extension(IServiceCollection services)
    {
        /// <summary>Registers a singleton service as a projection of another registered service.</summary>
        /// <param name="selector">Derives the service from the source service.</param>
        /// <typeparam name="TService">The service to register.</typeparam>
        /// <typeparam name="TSource">The registered service it is taken from.</typeparam>
        public IServiceCollection AddSingletonFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.AddSingleton(sp => selector(sp.GetRequiredService<TSource>()));
        }

        /// <summary>Registers a singleton service as an alias of another registered service.</summary>
        /// <typeparam name="TService">The service to register.</typeparam>
        /// <typeparam name="TServiceImplementation">The registered service it resolves to.</typeparam>
        public IServiceCollection AddSingletonFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.AddSingletonFrom<TService, TServiceImplementation>(v => v);
        }

        /// <summary>Registers a singleton service created from the service provider.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to register.</typeparam>
        public IServiceCollection AddSingletonFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.AddSingletonFrom<TService, IServiceProvider>(selector);
        }

        /// <summary>Registers a singleton service created by the service proxy factory.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to register.</typeparam>
        public IServiceCollection AddSingletonFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.AddSingletonFrom<TService, IServiceProxyFactory>(selector);
        }

        /// <summary>Registers a singleton service that is created through the proxy factory; the implementation type is chosen at run time.</summary>
        /// <param name="targetTypeSelector">Chooses the implementation type.</param>
        /// <param name="replace">Replace an existing registration and redirect.</param>
        /// <typeparam name="TService">The service type.</typeparam>
        public IServiceCollection AddSingletonServiceProxy<TService>(Func<IServiceProvider, Type> targetTypeSelector, bool replace = false)
            where TService : class
        {
            services.AddServiceProxyFactory(b => b.SetRedirect(typeof(TService), targetTypeSelector, replace));

            var init = (IServiceProxyFactory spf) => spf.Create<TService>();

            return replace ? services.ReplaceSingletonFrom(init) : services.AddSingletonFrom(init);
        }

        /// <summary>Registers a singleton service that is created through the proxy factory with the given implementation type.</summary>
        /// <param name="targetType">The implementation type.</param>
        /// <param name="replace">Replace an existing registration and redirect.</param>
        /// <typeparam name="TService">The service type.</typeparam>
        public IServiceCollection AddSingletonServiceProxy<TService>(Type targetType, bool replace = false)
            where TService : class => services.AddSingletonServiceProxy<TService>(_ => targetType, replace);

        /// <summary>Registers a singleton service that is created through the proxy factory with the given implementation type.</summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public IServiceCollection AddSingletonServiceProxy<TService, TImplementation>()
            where TService : class
            where TImplementation : TService =>
            services.AddSingletonServiceProxy<TService>(typeof(TImplementation));
    }

    extension(IServiceCollection services)
    {
        /// <summary>Replaces the registration of a service with a scoped implementation.</summary>
        /// <typeparam name="TService">The service to replace.</typeparam>
        /// <typeparam name="TServiceImplementation">The new implementation.</typeparam>
        public IServiceCollection ReplaceScoped<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.Replace(ServiceDescriptor.Scoped<TService, TServiceImplementation>());
        }

        /// <summary>Replaces the registration of a service with a scoped implementation.</summary>
        /// <param name="serviceType">The service to replace.</param>
        /// <param name="implementationType">The new implementation.</param>
        public IServiceCollection ReplaceScoped(Type serviceType, Type implementationType)
        {
            return services.Replace(ServiceDescriptor.Scoped(serviceType, implementationType));
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>Replaces the registration with a scoped service taken from another registered service.</summary>
        /// <param name="selector">Derives the service from the source service.</param>
        /// <typeparam name="TService">The service to replace.</typeparam>
        /// <typeparam name="TSource">The registered service it is taken from.</typeparam>
        public IServiceCollection ReplaceScopedFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.Replace(ServiceDescriptor.Scoped<TService>(sp => selector(sp.GetRequiredService<TSource>())));
        }

        /// <summary>Replaces the registration with a scoped alias of another registered service.</summary>
        /// <typeparam name="TService">The service to replace.</typeparam>
        /// <typeparam name="TServiceImplementation">The registered service it resolves to.</typeparam>
        public IServiceCollection ReplaceScopedFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.ReplaceScopedFrom<TService, TServiceImplementation>(v => v);
        }

        /// <summary>Replaces the registration with a scoped service created from the service provider.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to replace.</typeparam>
        public IServiceCollection ReplaceScopedFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.ReplaceScopedFrom<TService, IServiceProvider>(selector);
        }

        /// <summary>Replaces the registration with a scoped service created by the service proxy factory.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to replace.</typeparam>
        public IServiceCollection ReplaceScopedFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.ReplaceScopedFrom<TService, IServiceProxyFactory>(selector);
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>Replaces the registration of a service with a singleton instance.</summary>
        /// <param name="instance">The instance.</param>
        /// <typeparam name="TService">The service to replace.</typeparam>
        public IServiceCollection ReplaceSingleton<TService>(TService instance)
            where TService : class
        {
            return services.Replace(ServiceDescriptor.Singleton(instance));
        }

        /// <summary>Replaces the registration of a service with a singleton implementation.</summary>
        /// <typeparam name="TService">The service to replace.</typeparam>
        /// <typeparam name="TServiceImplementation">The new implementation.</typeparam>
        public IServiceCollection ReplaceSingleton<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.Replace(ServiceDescriptor.Singleton<TService, TServiceImplementation>());
        }

        /// <summary>Replaces the registration of a service with a singleton implementation.</summary>
        /// <param name="serviceType">The service to replace.</param>
        /// <param name="implementationType">The new implementation.</param>
        public IServiceCollection ReplaceSingleton(Type serviceType, Type implementationType)
        {
            return services.Replace(ServiceDescriptor.Singleton(serviceType, implementationType));
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>Replaces the registration with a singleton service taken from another registered service.</summary>
        /// <param name="selector">Derives the service from the source service.</param>
        /// <typeparam name="TService">The service to replace.</typeparam>
        /// <typeparam name="TSource">The registered service it is taken from.</typeparam>
        public IServiceCollection ReplaceSingletonFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.Replace(ServiceDescriptor.Singleton<TService>(sp => selector(sp.GetRequiredService<TSource>())));
        }

        /// <summary>Replaces the registration with a singleton alias of another registered service.</summary>
        /// <typeparam name="TService">The service to replace.</typeparam>
        /// <typeparam name="TServiceImplementation">The registered service it resolves to.</typeparam>
        public IServiceCollection ReplaceSingletonFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.ReplaceSingletonFrom<TService, TServiceImplementation>(v => v);
        }

        /// <summary>Replaces the registration with a singleton service created from the service provider.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to replace.</typeparam>
        public IServiceCollection ReplaceSingletonFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.ReplaceSingletonFrom<TService, IServiceProvider>(selector);
        }

        /// <summary>Replaces the registration with a singleton service created by the service proxy factory.</summary>
        /// <param name="selector">Creates the service.</param>
        /// <typeparam name="TService">The service to replace.</typeparam>
        public IServiceCollection ReplaceSingletonFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.ReplaceSingletonFrom<TService, IServiceProxyFactory>(selector);
        }
    }
}