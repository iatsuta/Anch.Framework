using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.AccessDenied;
using SecuritySystem.DiTests.DomainObjects;
using SecuritySystem.DiTests.Rules;
using SecuritySystem.Providers;

namespace SecuritySystem.DiTests;

public class AccessDeniedExceptionServiceTests(IServiceProvider rootServiceProvider)
{
    [Fact]
    public void CreateNewObject_AccessDeniedMessage_IsValid()
    {
        // Arrange
        var service = rootServiceProvider.GetRequiredService<IAccessDeniedExceptionService>();
        var employee = new Employee();

        // Act
        var result = service.GetAccessDeniedException(
            AccessResult.AccessDeniedResult.Create(employee));

        // Assert
        Assert.Equal($"You have no permissions to create object with type = '{nameof(Employee)}'", result.Message);
    }

    [Fact]
    public void ChangeExistObject_AccessDeniedMessage_IsValid()
    {
        // Arrange
        var service = rootServiceProvider.GetRequiredService<IAccessDeniedExceptionService>();
        var employee = new Employee { Id = Guid.NewGuid() };

        // Act
        var result = service.GetAccessDeniedException(
            AccessResult.AccessDeniedResult.Create(employee));

        // Assert
        Assert.Equal($"You have no permissions to access object with type = '{nameof(Employee)}' (id = '{employee.Id}')", result.Message);
    }

    [Fact]
    public void CreateNewObjectWithOperation_AccessDeniedMessage_IsValid()
    {
        // Arrange
        var service = rootServiceProvider.GetRequiredService<IAccessDeniedExceptionService>();
        var employee = new Employee();
        var securityRule = ExampleSecurityOperation.EmployeeView;

        // Act
        var result = service.GetAccessDeniedException(
            AccessResult.AccessDeniedResult.Create(employee, securityRule));

        // Assert
        Assert.Equal($"You have no permissions to create object with type = '{nameof(Employee)}' (securityRule = '{securityRule.Name}')", result.Message);
    }

    [Fact]
    public void ChangeExistObjectWithOperation_AccessDeniedMessage_IsValid()
    {
        // Arrange
        var service = rootServiceProvider.GetRequiredService<IAccessDeniedExceptionService>();
        var employee = new Employee { Id = Guid.NewGuid() };
        var securityRule = ExampleSecurityOperation.EmployeeView;

        // Act
        var result = service.GetAccessDeniedException(
            AccessResult.AccessDeniedResult.Create(employee, securityRule));

        // Assert
        Assert.Equal($"You have no permissions to access object with type = '{nameof(Employee)}' (id = '{employee.Id}', securityRule = '{securityRule.Name}')", result.Message);
    }
}
