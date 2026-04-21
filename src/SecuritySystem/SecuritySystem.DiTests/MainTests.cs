using CommonFramework.Testing;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.AccessDenied;
using SecuritySystem.DiTests.DomainObjects;
using SecuritySystem.DiTests.Environment;
using SecuritySystem.DiTests.Rules;
using SecuritySystem.DomainServices;
using SecuritySystem.Providers;

using SecuritySystem.Testing;

namespace SecuritySystem.DiTests;

public class MainTests
{
    private readonly BusinessUnit bu1;

    private readonly BusinessUnit bu2;

    private readonly BusinessUnit bu3;

    private readonly Employee employee1;

    private readonly Employee employee2;

    private readonly Employee employee3;

    private readonly Employee employee4;


    private readonly IServiceProvider rootServiceProvider;

    public MainTests(IServiceProvider rootServiceProvider)
    {
        this.rootServiceProvider = rootServiceProvider;

        this.bu1 = new() { Id = Guid.NewGuid() };
        this.bu2 = new BusinessUnit { Id = Guid.NewGuid(), Parent = this.bu1 };
        this.bu3 = new BusinessUnit { Id = Guid.NewGuid() };

        this.employee1 = new Employee { Id = Guid.NewGuid(), BusinessUnit = this.bu1 };
        this.employee2 = new Employee { Id = Guid.NewGuid(), BusinessUnit = this.bu2 };
        this.employee3 = new Employee { Id = Guid.NewGuid(), BusinessUnit = this.bu3 };
        this.employee4 = new Employee { Id = Guid.NewGuid() };


        var queryableSource = this.rootServiceProvider.GetRequiredService<TestQueryableSource>();

        queryableSource.InnerSource.GetQueryable<BusinessUnitDirectAncestorLink>()
            .Returns(this.GetBusinessUnitAncestorLinkSource().AsQueryable());

        queryableSource.InnerSource.GetQueryable<Employee>()
            .Returns(new[] { this.employee1, this.employee2, this.employee3, this.employee4 }.AsQueryable());


        var permissionStorge = this.rootServiceProvider.GetRequiredService<TestPermissionStorge>();

        permissionStorge.Permissions =
        [
            new TestPermission(ExampleSecurityRole.TestRole)
            {
                Restrictions = { { typeof(BusinessUnit), new[] { this.bu1.Id } } }
            }
        ];
    }


    [CommonFact]
    public async Task TestEmployeesSecurity_EmployeeHasAccessCorrect(CancellationToken ct)
    {
        // Arrange
        await using var scope = this.rootServiceProvider.CreateAsyncScope();

        var employeeDomainSecurityService =
            scope.ServiceProvider.GetRequiredService<IDomainSecurityService<Employee>>();
        var counterService = scope.ServiceProvider.GetRequiredService<BusinessUnitAncestorLinkSourceExecuteCounter>();
        var securityProvider = employeeDomainSecurityService.GetSecurityProvider(SecurityRule.View);

        // Act
        var result1 = await securityProvider.HasAccessAsync(this.employee1, ct);
        var result2 = await securityProvider.HasAccessAsync(this.employee2, ct);
        var result3 = await securityProvider.HasAccessAsync(this.employee3, ct);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeFalse();

        counterService.Count.Should().Be(1);
    }

    [CommonFact]
    public async Task CheckEmployeeWithoutSecurity_ExceptionRaised(CancellationToken ct)
    {
        // Arrange
        await using var scope = this.rootServiceProvider.CreateAsyncScope();

        var employeeDomainSecurityService =
            scope.ServiceProvider.GetRequiredService<IDomainSecurityService<Employee>>();
        var accessDeniedExceptionService = scope.ServiceProvider.GetRequiredService<IAccessDeniedExceptionService>();

        var securityProvider = employeeDomainSecurityService.GetSecurityProvider(SecurityRule.View);

        // Act
        var checkAccessAction = () =>
            securityProvider.CheckAccessAsync(this.employee3, accessDeniedExceptionService, ct);

        // Assert
        await checkAccessAction.Should().ThrowAsync<AccessDeniedException>();
    }


    private IEnumerable<BusinessUnitDirectAncestorLink> GetBusinessUnitAncestorLinkSource()
    {
        var counter = this.rootServiceProvider.GetRequiredService<BusinessUnitAncestorLinkSourceExecuteCounter>();
        counter.Count++;

        yield return new BusinessUnitDirectAncestorLink { Ancestor = this.bu1, Child = this.bu1 };
        yield return new BusinessUnitDirectAncestorLink { Ancestor = this.bu2, Child = this.bu2 };
        yield return new BusinessUnitDirectAncestorLink { Ancestor = this.bu3, Child = this.bu3 };

        yield return new BusinessUnitDirectAncestorLink { Ancestor = this.bu1, Child = this.bu2 };
    }
}