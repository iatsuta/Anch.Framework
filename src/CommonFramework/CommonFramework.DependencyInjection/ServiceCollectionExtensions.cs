using CommonFramework.DependencyInjection.ServiceProxy;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommonFramework.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServiceProxyFactory(Action<IServiceProxySetup>? setup = null) =>
            services.Initialize<ServiceProxySetup>(setup);

        public bool AlreadyInitialized<TService>(ServiceLifetime lifetime = ServiceLifetime.Singleton, bool isKeyed = false) =>

            services.Any(sd => sd.IsKeyedService == isKeyed && sd.Lifetime == lifetime && sd.ServiceType == typeof(TService));

        public bool AlreadyInitialized<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton, bool isKeyed = false)

            where TImplementation : TService =>

            services.Any(sd =>
                sd.IsKeyedService == isKeyed && sd.Lifetime == lifetime && sd.ServiceType == typeof(TService) &&
                sd.ImplementationType == typeof(TImplementation));

        public IServiceCollection Initialize<TServiceInitializer>(Action<TServiceInitializer>? setup)
            where TServiceInitializer : IServiceInitializer<IServiceCollection>, new() =>
            services.Initialize<IServiceCollection, TServiceInitializer>(setup);

        public IServiceCollection BindServiceProxy(Type serviceType, Type binderType, bool replace = false)
        {
            return services.AddServiceProxyFactory(b => b.BindRedirect(serviceType, binderType, replace));
        }
    }

    extension<TService>(TService service)
    {
        public TService Initialize<TServiceInitializer>(Action<TServiceInitializer>? setup)
            where TServiceInitializer : IServiceInitializer<TService>, new() =>
            service.Initialize(new TServiceInitializer(), setup);

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
        public IServiceCollection AddScopedFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.AddScoped(sp => selector(sp.GetRequiredService<TSource>()));
        }

        public IServiceCollection AddScopedFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.AddScopedFrom<TService, TServiceImplementation>(v => v);
        }

        public IServiceCollection AddScopedFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.AddScopedFrom<TService, IServiceProvider>(selector);
        }

        public IServiceCollection AddScopedFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.AddScopedFrom<TService, IServiceProxyFactory>(selector);
        }


        public IServiceCollection AddScopedServiceProxy<TService>(Func<IServiceProvider, Type> targetTypeSelector, bool replace = false)
            where TService : class
        {
            services.AddServiceProxyFactory(b => b.SetRedirect(typeof(TService), targetTypeSelector, replace));

            var init = (IServiceProxyFactory spf) => spf.Create<TService>();

            return replace ? services.ReplaceScopedFrom(init) : services.AddScopedFrom(init);
        }

        public IServiceCollection AddScopedServiceProxy<TService>(Type targetType, bool replace = false)
            where TService : class => services.AddScopedServiceProxy<TService>(_ => targetType, replace);

        public IServiceCollection AddScopedServiceProxy<TService, TImplementation>()
            where TService : class
            where TImplementation : TService =>
            services.AddScopedServiceProxy<TService>(typeof(TImplementation));
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddSingletonFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.AddSingleton(sp => selector(sp.GetRequiredService<TSource>()));
        }

        public IServiceCollection AddSingletonFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.AddSingletonFrom<TService, TServiceImplementation>(v => v);
        }

        public IServiceCollection AddSingletonFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.AddSingletonFrom<TService, IServiceProvider>(selector);
        }

        public IServiceCollection AddSingletonFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.AddSingletonFrom<TService, IServiceProxyFactory>(selector);
        }

        public IServiceCollection AddSingletonServiceProxy<TService>(Func<IServiceProvider, Type> targetTypeSelector, bool replace = false)
            where TService : class
        {
            services.AddServiceProxyFactory(b => b.SetRedirect(typeof(TService), targetTypeSelector, replace));

            var init = (IServiceProxyFactory spf) => spf.Create<TService>();

            return replace ? services.ReplaceSingletonFrom(init) : services.AddSingletonFrom(init);
        }

        public IServiceCollection AddSingletonServiceProxy<TService>(Type targetType, bool replace = false)
            where TService : class => services.AddSingletonServiceProxy<TService>(_ => targetType, replace);

        public IServiceCollection AddSingletonServiceProxy<TService, TImplementation>()
            where TService : class
            where TImplementation : TService =>
            services.AddSingletonServiceProxy<TService>(typeof(TImplementation));
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection ReplaceScoped<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.Replace(ServiceDescriptor.Scoped<TService, TServiceImplementation>());
        }

        public IServiceCollection ReplaceScoped(Type serviceType, Type implementationType)
        {
            return services.Replace(ServiceDescriptor.Scoped(serviceType, implementationType));
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection ReplaceScopedFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.Replace(ServiceDescriptor.Scoped<TService>(sp => selector(sp.GetRequiredService<TSource>())));
        }

        public IServiceCollection ReplaceScopedFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.ReplaceScopedFrom<TService, TServiceImplementation>(v => v);
        }

        public IServiceCollection ReplaceScopedFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.ReplaceScopedFrom<TService, IServiceProvider>(selector);
        }

        public IServiceCollection ReplaceScopedFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.ReplaceScopedFrom<TService, IServiceProxyFactory>(selector);
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection ReplaceSingleton<TService>(TService instance)
            where TService : class
        {
            return services.Replace(ServiceDescriptor.Singleton(instance));
        }

        public IServiceCollection ReplaceSingleton<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.Replace(ServiceDescriptor.Singleton<TService, TServiceImplementation>());
        }

        public IServiceCollection ReplaceSingleton(Type serviceType, Type implementationType)
        {
            return services.Replace(ServiceDescriptor.Singleton(serviceType, implementationType));
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection ReplaceSingletonFrom<TService, TSource>(Func<TSource, TService> selector)
            where TService : class
            where TSource : notnull
        {
            return services.Replace(ServiceDescriptor.Singleton<TService>(sp => selector(sp.GetRequiredService<TSource>())));
        }

        public IServiceCollection ReplaceSingletonFrom<TService, TServiceImplementation>()
            where TServiceImplementation : class, TService
            where TService : class
        {
            return services.ReplaceSingletonFrom<TService, TServiceImplementation>(v => v);
        }

        public IServiceCollection ReplaceSingletonFrom<TService>(Func<IServiceProvider, TService> selector)
            where TService : class
        {
            return services.ReplaceSingletonFrom<TService, IServiceProvider>(selector);
        }

        public IServiceCollection ReplaceSingletonFrom<TService>(Func<IServiceProxyFactory, TService> selector)
            where TService : class
        {
            return services.ReplaceSingletonFrom<TService, IServiceProxyFactory>(selector);
        }
    }
}