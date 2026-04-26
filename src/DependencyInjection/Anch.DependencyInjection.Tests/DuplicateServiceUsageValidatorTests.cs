using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection.Tests;

public class DuplicateServiceUsageValidatorTests
{
    [Fact]
    public void Validate_ShouldThrow_WhenMultipleServicesUsedAsSingleInstanceDependency()
    {
        // arrange
        var services = new ServiceCollection();

        services.AddSingleton<IInnerService, InnerService>();
        services.AddSingleton<IInnerService, InnerService>();

        services.AddSingleton<Consumer>();

        services.AddValidator<DuplicateServiceUsageValidator>();

        // act
        var ex = Assert.Throws<InvalidOperationException>(() => services.Validate());

        // assert
        Assert.Contains("has been registered many times", ex.Message);
    }

    [Fact]
    public void Validate_ShouldNotThrow_WhenServiceUsedAsCollectionDependency()
    {
        // arrange
        var services = new ServiceCollection();

        services.AddSingleton<IInnerService, InnerService>();
        services.AddSingleton<IInnerService, InnerService>();

        services.AddSingleton<ConsumerWithCollection>();

        services.AddValidator<DuplicateServiceUsageValidator>();

        // act
        var ex = Record.Exception(() => services.Validate());

        // assert
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_ShouldNotThrow_WhenDuplicateServiceIsExcludedByValidator()
    {
        // arrange
        var services = new ServiceCollection();

        services.AddSingleton<IInnerService, InnerService>();
        services.AddSingleton<IInnerService, InnerService>();

        services.AddSingleton<Consumer>();

        services.AddValidator(new DuplicateServiceUsageValidator([typeof(IInnerService)]));

        // act
        var ex = Record.Exception(() => services.Validate());

        // assert
        Assert.Null(ex);
    }


    [Fact]
    public void Validate_ShouldThrow_WhenKeyedServiceUsedMultipleTimes()
    {
        // arrange
        var services = new ServiceCollection();

        services.AddKeyedSingleton<IInnerService, InnerService>(IInnerService.RootKey);
        services.AddKeyedSingleton<IInnerService, InnerService>(IInnerService.RootKey);

        services.AddSingleton<KeyedConsumer>();

        services.AddValidator<DuplicateServiceUsageValidator>();

        // act
        var ex = Assert.Throws<InvalidOperationException>(() => services.Validate());

        // assert
        Assert.Contains("has been registered many times", ex.Message);
    }

    [Fact]
    public void Validate_ShouldNotThrow_WhenKeyedServiceUsedOnce()
    {
        // arrange
        var services = new ServiceCollection();

        services.AddKeyedSingleton<IInnerService, InnerService>(IInnerService.RootKey);
        services.AddKeyedSingleton<IInnerService, InnerService>(IInnerService.RootKey);

        services.AddSingleton<KeyedConsumerCollection>();

        services.AddValidator<DuplicateServiceUsageValidator>();

        // act
        var ex = Record.Exception(() => services.Validate());

        // assert
        Assert.Null(ex);
    }

    private interface IInnerService
    {
        public const string RootKey = "Root";
    }

    private class InnerService : IInnerService;

    private class Consumer(IInnerService _);

    private class ConsumerWithCollection(IEnumerable<IInnerService> _);

    private class KeyedConsumer([FromKeyedServices(IInnerService.RootKey)] IInnerService _);

    private class KeyedConsumerCollection([FromKeyedServices(IInnerService.RootKey)]IEnumerable<IInnerService> _);
}