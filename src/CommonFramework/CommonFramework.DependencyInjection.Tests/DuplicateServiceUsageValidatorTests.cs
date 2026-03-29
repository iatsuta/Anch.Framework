using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.DependencyInjection.Tests;

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
        Action act = () => services.Validate();

        // assert
        act.Should().Throw<InvalidOperationException>()
            .Where(ex => ex.Message.Contains("has been registered many times"));
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
        Action act = () => services.Validate();

        // assert
        act.Should().NotThrow();
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
        Action act = () => services.Validate();

        // assert
        act.Should().NotThrow();
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
        Action act = () => services.Validate();

        // assert
        act.Should().Throw<InvalidOperationException>()
            .Where(ex => ex.Message.Contains("has been registered many times"));
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
        Action act = () => services.Validate();

        // assert
        act.Should().NotThrow();
    }

    private interface IInnerService
    {
        public const string RootKey = "Root";
    }

    private class InnerService : IInnerService;

    private class Consumer(IInnerService service);

    private class ConsumerWithCollection(IEnumerable<IInnerService> services);

    private class KeyedConsumer([FromKeyedServices(IInnerService.RootKey)] IInnerService service);

    private class KeyedConsumerCollection([FromKeyedServices(IInnerService.RootKey)]IEnumerable<IInnerService> services);
}