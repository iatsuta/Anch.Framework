using CommonFramework.VisualIdentitySource.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.VisualIdentitySource.Tests;

public class DomainObjectDisplayServiceTests
{
    [Fact]
    public void DomainObjectDisplayService_Format_ReturnsPropertyName_ByDefault()
    {
        // Arrange
        var sp = new ServiceCollection()
            .AddVisualIdentitySource()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var service = sp.GetRequiredService<IDomainObjectDisplayService>();
        var testObject = new TestObject1 { Name = nameof(this.DomainObjectDisplayService_Format_ReturnsPropertyName_ByDefault) };

        // Act
        var result = service.Format(testObject);

        // Assert
        result.Should().Be(testObject.Name);
    }

    [Fact]
    public void DomainObjectDisplayService_Format_ReturnsConfiguredPropertyName()
    {
        // Arrange
        var nameLambda = ExpressionHelper.Create((TestObject2 v) => v.MyName);

        var sp = new ServiceCollection()
            .AddVisualIdentitySource(b => b.SetName(nameLambda))
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var service = sp.GetRequiredService<IDomainObjectDisplayService>();
        var testObject = new TestObject2 { MyName = nameof(this.DomainObjectDisplayService_Format_ReturnsConfiguredPropertyName) };

        // Act
        var result = service.Format(testObject);

        // Assert
        result.Should().Be(testObject.MyName);
    }

    [Fact]
    public void DomainObjectDisplayService_Format_UsesCustomDisplayFunction()
    {
        // Arrange
        var sp = new ServiceCollection()
            .AddVisualIdentitySource(b => b.SetDisplay((TestObject2 v) => v.MyName))
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var service = sp.GetRequiredService<IDomainObjectDisplayService>();
        var testObject = new TestObject2 { MyName = nameof(this.DomainObjectDisplayService_Format_UsesCustomDisplayFunction) };

        // Act
        var result = service.Format(testObject);

        // Assert
        result.Should().Be(testObject.MyName);
    }

    public class TestObject1
    {
        public required string Name { get; set; }
    }

    public class TestObject2
    {
        public required string MyName { get; set; }
    }
}