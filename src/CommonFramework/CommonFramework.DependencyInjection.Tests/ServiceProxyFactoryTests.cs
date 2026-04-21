using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.DependencyInjection.Tests;

public sealed class ServiceProxyFactoryTests
{
    public sealed class OriginalService : IService;

    private sealed class Service : IService;

    private interface IService;

    public sealed class OriginalService<T> : IService<T>;

    private sealed class Service<T> : IService<T>;

    private interface IService<T>;

    private class Service<T, THiddenGeneric> : IService<T>;

    [Fact]
    public void AddScopedFrom_resolves_service_via_service_proxy_factory_create()
    {
        // arrange
        var sp = new ServiceCollection()
            .AddServiceProxyFactory()
            .AddScopedFrom(spf => spf.Create<IService, Service>())
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        // act
        using var scope = sp.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.IsType<Service>(service);
    }

    [Fact]
    public void AddSingletonFrom_resolves_service_via_service_proxy_factory_create()
    {
        // arrange
        var sp = new ServiceCollection()
            .AddServiceProxyFactory()
            .AddSingletonFrom(spf => spf.Create<IService, Service>())
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        // act
        var service = sp.GetRequiredService<IService>();

        // assert
        Assert.IsType<Service>(service);
    }

    [Fact]
    public void ReplaceScopedFrom_replaces_service_using_service_proxy_factory()
    {
        // arrange
        var provider = new ServiceCollection()
            .AddServiceProxyFactory()
            .AddScoped<IService>(_ => new OriginalService())
            .ReplaceScopedFrom(spf => spf.Create<IService, Service>())
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        // act
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        // assert
        Assert.IsType<Service>(service);
    }

    [Fact]
    public void ReplaceScopedFrom_resolves_generic_service_via_service_proxy_factory_with_redirect()
    {
        // arrange
        var provider = new ServiceCollection()
            .AddServiceProxyFactory(b => b.SetRedirect(typeof(OriginalService<>), typeof(Service<>), false))
            .ReplaceScopedFrom(spf => spf.Create<IService<int>, OriginalService<int>>())
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        // act
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService<int>>();

        // assert
        Assert.IsType<Service<int>>(service);
    }

    [Fact]
    public void ServiceProxyFactory_Creates_CorrectServiceType()
    {
        // arrange
        var sp = new ServiceCollection()
            .AddSingletonServiceProxy<IService<int>, Service<int, string>>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var serviceProxyFactory = sp.GetServiceProxyFactory();

        // act
        var service = serviceProxyFactory.Create<IService<int>>();

        // assert
        Assert.IsType<Service<int, string>>(service);
    }
}