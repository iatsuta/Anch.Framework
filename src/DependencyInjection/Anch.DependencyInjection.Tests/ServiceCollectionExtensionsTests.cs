using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    private sealed class Source
    {
        public Service Service { get; set; } = new ();
    }

    public sealed class OriginalService : IService;

    private sealed class Service : IService;

    private interface IService;


    [Fact]
    public void AddScopedFrom_resolves_service_via_selector_applied_to_resolved_source()
    {
        // arrange
        var expectedSource = new Source();

        var provider = new ServiceCollection()
            .AddScoped(_ => expectedSource)
            .AddScopedFrom<Service, Source>(s => s.Service)
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        // act
        using var scope = provider.CreateScope();

        var source = scope.ServiceProvider.GetRequiredService<Source>();
        var service = scope.ServiceProvider.GetRequiredService<Service>();

        // assert
        Assert.Equal(expectedSource, source);
        Assert.Equal(expectedSource.Service, service);
    }

    [Fact]
    public void AddScopedFrom_registers_service_as_self_implementation()
    {
        // arrange
        var expectedService = new Service();

        var provider = new ServiceCollection()
            .AddScoped(_ => expectedService)
            .AddScopedFrom<IService, Service>()
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        using var scope = provider.CreateScope();

        var implementation = scope.ServiceProvider.GetRequiredService<Service>();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.Same(expectedService, implementation);
        Assert.Same(expectedService, service);
    }

    [Fact]
    public void AddScopedFrom_registers_service_resolved_from_service_provider()
    {
        // arrange
        var expectedService = new Service();

        var provider = new ServiceCollection()
            .AddScoped(_ => expectedService)
            .AddScopedFrom<IService>(sp => sp.GetRequiredService<Service>())
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        using var scope = provider.CreateScope();

        var implementation = scope.ServiceProvider.GetRequiredService<Service>();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.Same(expectedService, implementation);
        Assert.Same(expectedService, service);
    }

    [Fact]
    public void AddSingletonFrom_resolves_service_via_selector_applied_to_resolved_source()
    {
        // arrange
        var expectedSource = new Source();

        var provider = new ServiceCollection()
            .AddSingleton(_ => expectedSource)
            .AddSingletonFrom<Service, Source>(s => s.Service)
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var source = provider.GetRequiredService<Source>();
        var service = provider.GetRequiredService<Service>();

        // assert
        Assert.Equal(expectedSource, source);
        Assert.Equal(expectedSource.Service, service);
    }

    [Fact]
    public void AddSingletonFrom_registers_service_as_self_implementation()
    {
        // arrange
        var expectedService = new Service();

        var provider = new ServiceCollection()
            .AddSingleton(_ => expectedService)
            .AddSingletonFrom<IService, Service>()
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var implementation = provider.GetRequiredService<Service>();
        var service = provider.GetRequiredService<IService>();

        // assert
        Assert.Same(expectedService, implementation);
        Assert.Same(expectedService, service);
    }

    [Fact]
    public void AddSingletonFrom_registers_service_resolved_from_service_provider()
    {
        // arrange
        var expectedService = new Service();

        var provider = new ServiceCollection()
            .AddSingleton(_ => expectedService)
            .AddSingletonFrom<IService>(sp => sp.GetRequiredService<Service>())
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var implementation = provider.GetRequiredService<Service>();
        var service = provider.GetRequiredService<IService>();

        // assert
        Assert.Same(expectedService, implementation);
        Assert.Same(expectedService, service);
    }

    [Fact]
    public void ReplaceScopedFrom_replaces_existing_service_using_selector_and_source()
    {
        // arrange
        var originalService = new Service();
        var replacementService = new Service();

        var originalSource = new Source { Service = originalService };
        var replacementSource = new Source { Service = replacementService };

        var provider = new ServiceCollection()
            .AddScoped(_ => originalSource)
            .AddScoped<Service>(_ => originalService)
            .ReplaceScopedFrom<Service, Source>(_ => replacementSource.Service)
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<Service>();

        // assert
        Assert.Same(replacementService, service);
    }

    [Fact]
    public void ReplaceScopedFrom_replaces_existing_service_with_self_implementation()
    {
        // arrange
        var originalService = new Service();
        var replacementService = new Service();

        var provider = new ServiceCollection()
            .AddScoped<IService>(_ => originalService)
            .AddScoped(_ => replacementService)
            .ReplaceScopedFrom<IService, Service>()
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.Same(replacementService, service);
    }

    [Fact]
    public void ReplaceScopedFrom_replaces_existing_service_using_service_provider_selector()
    {
        // arrange
        var originalService = new Service();
        var replacementService = new Service();

        var provider = new ServiceCollection()
            .AddScoped<IService>(_ => originalService)
            .AddScoped(_ => replacementService)
            .ReplaceScopedFrom<IService>(sp => sp.GetRequiredService<Service>())
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.Same(replacementService, service);
    }

    [Fact]
    public void ReplaceSingletonFrom_replaces_existing_service_using_selector_and_source()
    {
        // arrange
        var originalService = new Service();
        var replacementService = new Service();

        var originalSource = new Source { Service = originalService };
        var replacementSource = new Source { Service = replacementService };

        var provider = new ServiceCollection()
            .AddSingleton(_ => originalSource)
            .AddSingleton<Service>(_ => originalService)
            .ReplaceSingletonFrom<Service, Source>(_ => replacementSource.Service)
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var service = provider.GetRequiredService<Service>();

        // assert
        Assert.Same(replacementService, service);
    }

    [Fact]
    public void ReplaceSingletonFrom_replaces_existing_service_with_self_implementation()
    {
        // arrange
        var originalService = new Service();
        var replacementService = new Service();

        var provider = new ServiceCollection()
            .AddSingleton<IService>(_ => originalService)
            .AddSingleton(_ => replacementService)
            .ReplaceSingletonFrom<IService, Service>()
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var service = provider.GetRequiredService<IService>();

        // assert
        Assert.Same(replacementService, service);
    }

    [Fact]
    public void ReplaceSingletonFrom_replaces_existing_service_using_service_provider_selector()
    {
        // arrange
        var originalService = new Service();
        var replacementService = new Service();

        var provider = new ServiceCollection()
            .AddSingleton<IService>(_ => originalService)
            .AddSingleton(_ => replacementService)
            .ReplaceSingletonFrom<IService>(sp => sp.GetRequiredService<Service>())
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var service = provider.GetRequiredService<IService>();

        // assert
        Assert.Same(replacementService, service);
    }

    [Fact]
    public void ReplaceScoped_replaces_existing_service_with_generic_implementation()
    {
        // arrange
        var provider = new ServiceCollection()
            .AddScoped<IService, OriginalService>()
            .ReplaceScoped<IService, Service>()
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.IsType<Service>(service);
    }

    [Fact]
    public void ReplaceScoped_replaces_existing_service_with_type_implementation()
    {// arrange
        var provider = new ServiceCollection()
            .AddScoped(typeof(IService), typeof(OriginalService))
            .ReplaceScoped(typeof(IService), typeof(Service))
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.IsType<Service>(service);
    }

    [Fact]
    public void ReplaceSingleton_replaces_existing_service_with_instance()
    {
        // arrange
        var originalService = new Service();
        var replacementService = new Service();

        var provider = new ServiceCollection()
            .AddSingleton<IService>(_ => originalService)
            .ReplaceSingleton<IService>(replacementService)
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var service = provider.GetRequiredService<IService>();

        // assert
        Assert.Same(replacementService, service);
    }

    [Fact]
    public void ReplaceSingleton_replaces_existing_service_with_generic_implementation()
    {
        // arrange
        var provider = new ServiceCollection()
            .AddSingleton<IService, OriginalService>()
            .ReplaceSingleton<IService, Service>()
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var service = provider.GetRequiredService<IService>();

        // assert
        Assert.IsType<Service>(service);
    }

    [Fact]
    public void ReplaceSingleton_replaces_existing_service_with_type_implementation()
    {
        // arrange
        var provider = new ServiceCollection()
            .AddSingleton(typeof(IService), typeof(OriginalService))
            .ReplaceSingleton(typeof(IService), typeof(Service))
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        // act
        var service = provider.GetRequiredService<IService>();

        // assert
        Assert.IsType<Service>(service);
    }
}