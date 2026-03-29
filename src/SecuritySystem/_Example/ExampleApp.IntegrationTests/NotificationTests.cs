using ExampleApp.Application;
using ExampleApp.Domain;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem;
using SecuritySystem.Notification;
using SecuritySystem.Notification.Domain;
using SecuritySystem.Services;

namespace ExampleApp.IntegrationTests;

public abstract class NotificationTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    private readonly SecurityRole testSecurityRole = ExampleSecurityRole.NotificationRole;

    private TypedSecurityIdentity<Guid> rootBusinessUnit = null!;

    private TypedSecurityIdentity<Guid> child_1_0_BusinessUnit = null!;

    private TypedSecurityIdentity<Guid> child_1_1_BusinessUnit = null!;

    private TypedSecurityIdentity<Guid> child_2_0_BusinessUnit = null!;

    private TypedSecurityIdentity<Guid> child_2_1_BusinessUnit = null!;

    private TypedSecurityIdentity<Guid> rootManagementUnit = null!;

    private TypedSecurityIdentity<Guid> child_1_0_ManagementUnit = null!;

    private TypedSecurityIdentity<Guid> child_1_1_ManagementUnit = null!;

    private TypedSecurityIdentity<Guid> child_2_0_ManagementUnit = null!;

    private TypedSecurityIdentity<Guid> child_2_1_ManagementUnit = null!;

    private TypedSecurityIdentity<Guid> rootEmployee = null!;

    private readonly string searchNotificationEmployeeLogin1 = nameof(searchNotificationEmployeeLogin1);

    private readonly string searchNotificationEmployeeLogin2 = nameof(searchNotificationEmployeeLogin2);

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        this.rootBusinessUnit = await this.SaveBusinessUnit(nameof(this.rootBusinessUnit));
        this.child_1_0_BusinessUnit = await this.SaveBusinessUnit(nameof(child_1_0_BusinessUnit), this.rootBusinessUnit);
        this.child_1_1_BusinessUnit = await this.SaveBusinessUnit(nameof(child_1_1_BusinessUnit), this.child_1_0_BusinessUnit);
        this.child_2_0_BusinessUnit = await this.SaveBusinessUnit(nameof(child_2_0_BusinessUnit), this.rootBusinessUnit);
        this.child_2_1_BusinessUnit = await this.SaveBusinessUnit(nameof(child_2_1_BusinessUnit), this.child_2_0_BusinessUnit);

        this.rootManagementUnit = await this.SaveManagementUnit(nameof(this.rootManagementUnit));
        this.child_1_0_ManagementUnit = await this.SaveManagementUnit(nameof(child_1_0_ManagementUnit), this.rootManagementUnit);
        this.child_1_1_ManagementUnit = await this.SaveManagementUnit(nameof(child_1_1_ManagementUnit), this.child_1_0_ManagementUnit);
        this.child_2_0_ManagementUnit = await this.SaveManagementUnit(nameof(child_2_0_ManagementUnit), this.rootManagementUnit);
        this.child_2_1_ManagementUnit = await this.SaveManagementUnit(nameof(child_2_1_ManagementUnit), this.child_2_0_ManagementUnit);

        this.rootEmployee = await this.SaveEmployee(nameof(this.rootEmployee));
    }

    [Fact]
    public async Task GetPrincipals_Direct_Test1_Searched()
    {
        // Arrange
        await this.AuthManager.For(this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.child_1_1_BusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit,
                Employee = this.rootEmployee
            }, this.CancellationToken);

        await this.AuthManager.For(this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };

        var employeeFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(Employee),
            ExpandType = NotificationExpandType.DirectOrFirstParent,
            Idents = [this.rootEmployee.Id]
        };

        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter, employeeFilter);

        // Assert
        result.Length.Should().Be(1);
        result.Single().Should().Be(this.searchNotificationEmployeeLogin1);
    }

    [Fact]
    public async Task GetPrincipals_Direct_Test2_Missed()
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };

        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter);

        // Assert
        result.Length.Should().Be(0);
    }

    [Fact]
    public async Task GetPrincipals_Direct_Test3_Missed()
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.DirectOrEmpty,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.DirectOrEmpty,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };


        var employeeChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(Employee),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.rootEmployee.Id]
        };

        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter, employeeChildFilter);

        // Assert
        result.Length.Should().Be(0);
    }

    [Fact]
    public async Task GetPrincipals_Direct_Test4_Searched()
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit,
                Employee = this.rootEmployee
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.DirectOrFirstParentOrEmpty,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.DirectOrFirstParentOrEmpty,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };


        var employeeChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(Employee),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.rootEmployee.Id]
        };

        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter, employeeChildFilter);

        // Assert
        result.Length.Should().Be(1);
    }

    [Fact]
    public async Task GetPrincipals_DirectOrEmpty_Test1_Searched()
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.DirectOrEmpty,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };

        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter);

        // Assert
        result.Length.Should().Be(1);
        result.Single().Should().Be(this.searchNotificationEmployeeLogin1);
    }

    [Fact]
    public async Task GetPrincipals_DirectOrFirstParentOrEmpty_Test1_Searched()
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.child_1_0_BusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.DirectOrFirstParentOrEmpty,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };

        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter);

        // Assert
        result.Length.Should().Be(1);
        result.Single().Should().Be(this.searchNotificationEmployeeLogin1);
    }

    [Fact]
    public async Task GetPrincipals_DirectOrFirstParentOrEmpty_Test2_Searched()
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_0_ManagementUnit
            }, this.CancellationToken);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.rootManagementUnit
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.DirectOrFirstParentOrEmpty,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.DirectOrFirstParentOrEmpty,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };
        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter);

        // Assert
        result.Length.Should().Be(1);
        result.Single().Should().Be(this.searchNotificationEmployeeLogin1);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetPrincipals_DirectOrFirstParentOrEmpty_Test3_Searched(bool swapPriority)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.child_1_0_BusinessUnit,
                ManagementUnit = this.rootManagementUnit
            }, this.CancellationToken);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_0_ManagementUnit
            }, this.CancellationToken);

        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.DirectOrFirstParentOrEmpty,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.DirectOrFirstParentOrEmpty,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };
        // Act
        var result = swapPriority
                         ? await this.GetNotificationPrincipalsAsync(mbuChildFilter, fbuChildFilter)
                         : await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter);

        // Assert
        result.Length.Should().Be(1);
        result.Single().Should().Be(swapPriority ? this.searchNotificationEmployeeLogin2 : this.searchNotificationEmployeeLogin1);
    }

    [Fact]
    public async Task GetPrincipals_All_Test1_Searched()
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.child_1_0_BusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, this.CancellationToken);


        var fbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            ExpandType = NotificationExpandType.All,
            Idents = [this.child_1_1_BusinessUnit.Id]
        };

        var mbuChildFilter = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(ManagementUnit),
            ExpandType = NotificationExpandType.Direct,
            Idents = [this.child_1_1_ManagementUnit.Id]
        };

        // Act
        var result = await this.GetNotificationPrincipalsAsync(fbuChildFilter, mbuChildFilter);

        // Assert
        result.Length.Should().Be(2);
        result.Should().Contain(this.searchNotificationEmployeeLogin1);
        result.Should().Contain(this.searchNotificationEmployeeLogin2);
    }

    private Task<string[]> GetNotificationPrincipalsAsync(params NotificationFilterGroup[] notificationFilterGroups) =>

        this.GetEvaluator<INotificationPrincipalExtractor<ExampleApp.Domain.Auth.General.Principal>>()
            .EvaluateAsync(TestingScopeMode.Read, async extractor =>
                await extractor.GetPrincipalsAsync([testSecurityRole], [.. notificationFilterGroups])
                    .Select(p => p.Name)
                    .ToArrayAsync(this.CancellationToken));

    [Fact]
    public async Task NotificationPrincipalExtractor_ReturnsUser_ForAssignedRoleAndBusinessUnit()
    {
        // Arrange
        var permissionBuIdentity =
            await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2", this.CancellationToken);

        var searchBuIdentity =
            await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2-Child", this.CancellationToken);

        var notificationFilterGroup = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            Idents = [searchBuIdentity.Id],
            ExpandType = NotificationExpandType.DirectOrFirstParent
        };

        var testUserName = nameof(NotificationPrincipalExtractor_ReturnsUser_ForAssignedRoleAndBusinessUnit);

        await this.AuthManager.For(testUserName).AddRoleAsync(new TestPermission(testSecurityRole) { BusinessUnit = permissionBuIdentity }, this.CancellationToken);

        // Act
        var principalNames = await this.GetNotificationPrincipalsAsync(notificationFilterGroup);

        // Assert
        principalNames.Should().BeEquivalentTo(testUserName);
    }

    private Task<TypedSecurityIdentity<Guid>> SaveBusinessUnit(string name, TypedSecurityIdentity<Guid>? parent = null) =>

        this.AuthManager.SaveSecurityContextAsync<BusinessUnit, Guid>(async sp => new BusinessUnit
        {
            Name = name,
            Parent = parent == null ? null : await sp.GetRequiredService<ISecurityRepository<BusinessUnit>>().GetObjectAsync(parent, this.CancellationToken)
        }, this.CancellationToken);

    private Task<TypedSecurityIdentity<Guid>> SaveManagementUnit(string name, TypedSecurityIdentity<Guid>? parent = null) =>

        this.AuthManager.SaveSecurityContextAsync<ManagementUnit, Guid>(async sp => new ManagementUnit
        {
            Name = name,
            Parent = parent == null ? null : await sp.GetRequiredService<ISecurityRepository<ManagementUnit>>().GetObjectAsync(parent, this.CancellationToken)
        }, this.CancellationToken);

    private Task<TypedSecurityIdentity<Guid>> SaveEmployee(string login) =>

        this.AuthManager.SaveSecurityContextAsync<Employee, Guid>(() => new Employee
        {
            Login = login
        }, this.CancellationToken);


    //[Fact]
    //public async Task VirtualPermissionTest()
    //{
    //    // Arrange
    //    var buIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2", this.CancellationToken);

    //    var notificationFilterGroup = new NotificationFilterGroup<Guid>
    //    {
    //        SecurityContextType = typeof(BusinessUnit),
    //        Idents = [buIdentity.Id],
    //        ExpandType = NotificationExpandType.DirectOrFirstParent
    //    };

    //    // Act
    //    var result = await this.GetEvaluator<INotificationPrincipalExtractor<Employee>>().EvaluateAsync<string[]>(TestingScopeMode.Read,
    //        async
    //            notificationPrincipalExtractor =>
    //            await notificationPrincipalExtractor
    //                .GetPrincipalsAsync([ExampleSecurityRole.TestManager], [notificationFilterGroup])
    //                .Select(employee => employee.Login)
    //                .ToArrayAsync(this.CancellationToken));

    //    // Assert

    //    return;

    //    //result.OrderBy(v => v.Name).Should().BeEquivalentTo(expectedResult);
    //}
}