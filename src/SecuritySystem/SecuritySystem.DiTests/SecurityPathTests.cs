using CommonFramework;
using CommonFramework.Testing;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.DiTests.DomainObjects;
using SecuritySystem.DiTests.Environment;
using SecuritySystem.DiTests.Rules;
using SecuritySystem.DiTests.Services;
using SecuritySystem.Services;
using SecuritySystem.Testing;

namespace SecuritySystem.DiTests;

public class SecurityPathTests
{
    private readonly IServiceProvider rootServiceProvider;

    private readonly BusinessUnit bu1 = new() { Id = Guid.NewGuid() };

    public SecurityPathTests(IServiceProvider rootServiceProvider)
    {
        this.rootServiceProvider = rootServiceProvider;
        this.rootServiceProvider.SetTestQueryable([new BusinessUnitDirectAncestorLink { Ancestor = this.bu1, Child = this.bu1 }]);
        this.rootServiceProvider.SetTestPermissions(new TestPermission(ExampleSecurityRole.TestKeyedRole)
            { Restrictions = { { typeof(BusinessUnit), new[] { this.bu1.Id } } } });
    }

    [Fact]
    public void TryApplyRestriction_RestrictionApplied()
    {
        //Arrange
        var service = this.rootServiceProvider.GetRequiredService<ISecurityPathRestrictionService>();

        var buExpr = ExpressionHelper.Create((Employee employee) => employee.BusinessUnit);
        var locationExpr = ExpressionHelper.Create((Employee employee) => employee.Location);
        var conditionExpr = ExpressionHelper.Create((Employee employee) => employee.TestCheckbox);

        var testSecurityPath = SecurityPath<Employee>.Create(buExpr).And(locationExpr);

        var restriction = SecurityPathRestriction.Create<BusinessUnit>().AddConditionFactory(typeof(TestCheckboxConditionFactory<>));

        var expectedNewSecurityPath = SecurityPath<Employee>.Create(buExpr).And(conditionExpr);

        //Act
        var newSecurityPath = service.ApplyRestriction(testSecurityPath, restriction);

        //Assert
        newSecurityPath.Should().Be(expectedNewSecurityPath);
    }

    [Fact]
    public void TryApplyOverflowRestriction_ResultPathIsEmpty()
    {
        //Arrange
        var service = this.rootServiceProvider.GetRequiredService<ISecurityPathRestrictionService>();

        var testSecurityPath = SecurityPath<Employee>.Create(employee => employee.BusinessUnit);

        var restriction = SecurityPathRestriction.Create<Location>();

        //Act
        var result = service.ApplyRestriction(testSecurityPath, restriction);

        //Assert
        result.Should().Be(SecurityPath<Employee>.Empty);
    }

    [Fact]
    public void TryApplyKeyedRestriction_SecurityPathCorrect()
    {
        //Arrange
        var key = nameof(Employee.AltBusinessUnit);

        var service = this.rootServiceProvider.GetRequiredService<ISecurityPathRestrictionService>();

        var baseSecurityPath = SecurityPath<Employee>.Create(employee => employee.BusinessUnit);
        var altSecurityPath = SecurityPath<Employee>.Create(employee => employee.AltBusinessUnit, key: key);
        var testSecurityPath = baseSecurityPath.And(altSecurityPath);

        var restriction = SecurityPathRestriction.Create<Location>().Add<BusinessUnit>(key: key);

        //Act
        var result = service.ApplyRestriction(testSecurityPath, restriction);

        //Assert
        result.Should().Be(altSecurityPath);
    }

    [Fact]
    public void EmptySecurityPathRestriction_SecurityPathNotModified()
    {
        //Arrange
        var key = nameof(Employee.AltBusinessUnit);

        var service = this.rootServiceProvider.GetRequiredService<ISecurityPathRestrictionService>();

        var baseSecurityPath = SecurityPath<Employee>.Create(employee => employee.BusinessUnit);
        var altSecurityPath = SecurityPath<Employee>.Create(employee => employee.AltBusinessUnit, key: key);

        var testSecurityPath = baseSecurityPath.And(altSecurityPath);

        var restriction = SecurityPathRestriction.Default;

        //Act
        var result = service.ApplyRestriction(testSecurityPath, restriction);

        //Assert
        result.Should().Be(testSecurityPath);
    }

    [CommonFact]
    public async Task KeyedSecurityPath_WithStrictly_EmployeeExcepted(CancellationToken ct)
    {
        // Arrange
        await using var scope = this.rootServiceProvider.CreateAsyncScope();

        var testSecurityPath = SecurityPath<Employee>
            .Create(employee => employee.Location)
            .And(employee => employee.BusinessUnit, true, key: "testKey");

        var securityProvider = scope.ServiceProvider.GetRequiredService<IDomainSecurityProviderFactory<Employee>>()
            .Create(ExampleSecurityRole.TestKeyedRole, testSecurityPath);

        var testEmployee1 = new Employee { BusinessUnit = this.bu1 };
        var testEmployee2 = new Employee();

        //Act
        var result1 = await securityProvider.HasAccessAsync(testEmployee1, ct);
        var result2 = await securityProvider.HasAccessAsync(testEmployee2, ct);

        //Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    [CommonFact]
    public async Task KeyedSecurityPath_WithoutStrictly_EmployeeIncluded(CancellationToken ct)
    {
        // Arrange
        await using var scope = this.rootServiceProvider.CreateAsyncScope();

        var testSecurityPath = SecurityPath<Employee>
            .Create(employee => employee.Location)
            .And(employee => employee.BusinessUnit, key: "testKey");

        var securityProvider = scope.ServiceProvider.GetRequiredService<IDomainSecurityProviderFactory<Employee>>()
            .Create(ExampleSecurityRole.TestKeyedRole, testSecurityPath);

        var testEmployee1 = new Employee { BusinessUnit = this.bu1 };
        var testEmployee2 = new Employee();

        //Act
        var result1 = await securityProvider.HasAccessAsync(testEmployee1, ct);
        var result2 = await securityProvider.HasAccessAsync(testEmployee2, ct);

        //Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }
}