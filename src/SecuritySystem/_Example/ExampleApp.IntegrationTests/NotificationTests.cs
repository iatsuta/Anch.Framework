using CommonFramework.GenericRepository;
using CommonFramework.Testing;
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

    protected override async ValueTask InitializeAsync(CancellationToken ct)
    {
        await base.InitializeAsync(ct);

        this.rootBusinessUnit = await this.SaveBusinessUnit(nameof(this.rootBusinessUnit), null, ct);
        this.child_1_0_BusinessUnit = await this.SaveBusinessUnit(nameof(this.child_1_0_BusinessUnit), this.rootBusinessUnit, ct);
		this.child_1_1_BusinessUnit = await this.SaveBusinessUnit(nameof(this.child_1_1_BusinessUnit), this.child_1_0_BusinessUnit, ct);
		this.child_2_0_BusinessUnit = await this.SaveBusinessUnit(nameof(this.child_2_0_BusinessUnit), this.rootBusinessUnit, ct);
		this.child_2_1_BusinessUnit = await this.SaveBusinessUnit(nameof(this.child_2_1_BusinessUnit), this.child_2_0_BusinessUnit, ct);

		this.rootManagementUnit = await this.SaveManagementUnit(nameof(this.rootManagementUnit), null, ct);
		this.child_1_0_ManagementUnit = await this.SaveManagementUnit(nameof(this.child_1_0_ManagementUnit), this.rootManagementUnit, ct);
		this.child_1_1_ManagementUnit = await this.SaveManagementUnit(nameof(this.child_1_1_ManagementUnit), this.child_1_0_ManagementUnit, ct);
		this.child_2_0_ManagementUnit = await this.SaveManagementUnit(nameof(this.child_2_0_ManagementUnit), this.rootManagementUnit, ct);
		this.child_2_1_ManagementUnit = await this.SaveManagementUnit(nameof(this.child_2_1_ManagementUnit), this.child_2_0_ManagementUnit, ct);

		this.rootEmployee = await this.SaveEmployee(nameof(this.rootEmployee), ct);
	}

    [CommonFact]
    public async Task GetPrincipals_Direct_Test1_Searched(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.child_1_1_BusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit,
                Employee = this.rootEmployee
            }, ct);

        await this.AuthManager.For(this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter, employeeFilter], ct);

        // Assert
        Assert.Single(result);
        Assert.Equal(this.searchNotificationEmployeeLogin1, result.Single());
    }

    [CommonFact]
    public async Task GetPrincipals_Direct_Test2_Missed(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter], ct);

        // Assert
        Assert.Empty(result);
    }

    [CommonFact]
    public async Task GetPrincipals_Direct_Test3_Missed(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter, employeeChildFilter], ct);

        // Assert
        Assert.Empty(result);
    }

    [CommonFact]
    public async Task GetPrincipals_Direct_Test4_Searched(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit,
                Employee = this.rootEmployee
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter, employeeChildFilter], ct);

        // Assert
        Assert.Single(result);
    }

    [CommonFact]
    public async Task GetPrincipals_DirectOrEmpty_Test1_Searched(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter], ct);

        // Assert
        Assert.Single(result);
        Assert.Equal(this.searchNotificationEmployeeLogin1, result.Single());
    }

    [CommonFact]
    public async Task GetPrincipals_DirectOrFirstParentOrEmpty_Test1_Searched(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.child_1_0_BusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter], ct);

        // Assert
        Assert.Single(result);
        Assert.Equal(this.searchNotificationEmployeeLogin1, result.Single());
    }

    [CommonFact]
    public async Task GetPrincipals_DirectOrFirstParentOrEmpty_Test2_Searched(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_0_ManagementUnit
            }, ct);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.rootManagementUnit
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter], ct);

        // Assert
        Assert.Single(result);
        Assert.Equal(this.searchNotificationEmployeeLogin1, result.Single());
    }

    [Theory]
    [CommonInlineData(true)]
    [CommonInlineData(false)]
    public async Task GetPrincipals_DirectOrFirstParentOrEmpty_Test3_Searched(bool swapPriority, CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.child_1_0_BusinessUnit,
                ManagementUnit = this.rootManagementUnit
            }, ct);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_0_ManagementUnit
            }, ct);

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
        var result = await this.GetNotificationPrincipalsAsync(swapPriority ? [mbuChildFilter, fbuChildFilter] : [fbuChildFilter, mbuChildFilter], ct);

        // Assert
        Assert.Single(result);
        Assert.Equal(swapPriority ? this.searchNotificationEmployeeLogin2 : this.searchNotificationEmployeeLogin1, result.Single());
    }

    [CommonFact]
    public async Task GetPrincipals_All_Test1_Searched(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin1).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.child_1_0_BusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);

        await this.AuthManager.For(
            this.searchNotificationEmployeeLogin2).SetRoleAsync(
            new TestPermission(this.testSecurityRole)
            {
                BusinessUnit = this.rootBusinessUnit,
                ManagementUnit = this.child_1_1_ManagementUnit
            }, ct);


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
        var result = await this.GetNotificationPrincipalsAsync([fbuChildFilter, mbuChildFilter], ct);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Contains(this.searchNotificationEmployeeLogin1, result);
        Assert.Contains(this.searchNotificationEmployeeLogin2, result);
    }

    [CommonFact]
    public async Task NotificationPrincipalExtractor_ReturnsUser_ForAssignedRoleAndBusinessUnit(CancellationToken ct)
    {
        // Arrange
        var permissionBuIdentity =
            await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2", ct);

        var searchBuIdentity =
            await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2-Child", ct);

        var notificationFilterGroup = new NotificationFilterGroup<Guid>
        {
            SecurityContextType = typeof(BusinessUnit),
            Idents = [searchBuIdentity.Id],
            ExpandType = NotificationExpandType.DirectOrFirstParent
        };

        var testUserName = nameof(this.NotificationPrincipalExtractor_ReturnsUser_ForAssignedRoleAndBusinessUnit);

        await this.AuthManager.For(testUserName).AddRoleAsync(new TestPermission(this.testSecurityRole) { BusinessUnit = permissionBuIdentity }, ct);

        // Act
        var principalNames = await this.GetNotificationPrincipalsAsync([notificationFilterGroup], ct);

        // Assert
        Assert.Equivalent(new[] { testUserName }, principalNames);
    }

    [CommonFact]
    public async Task NotificationPrincipalExtractor_ReturnsUser_ForTypedFilterGroup(CancellationToken ct)
    {
        // Arrange
        var permissionBuIdentity =
            await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2", ct);

        var searchBuIdentity =
            await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2-Child", ct);


        var testUserName = nameof(this.NotificationPrincipalExtractor_ReturnsUser_ForAssignedRoleAndBusinessUnit);

        await this.AuthManager.For(testUserName).AddRoleAsync(new TestPermission(this.testSecurityRole) { BusinessUnit = permissionBuIdentity }, ct);

        // Act
        var principalNames = await this.GetEvaluator<IServiceProvider>()
            .EvaluateAsync(TestingScopeMode.Read, async sp =>
            {
                var queryableSource = sp.GetRequiredService<IQueryableSource>();
                var extractor = sp.GetRequiredService<INotificationPrincipalExtractor<ExampleApp.Domain.Auth.General.Principal>>();

                var notificationFilterGroup = new TypedNotificationFilterGroup<BusinessUnit>
                {
                    Items = [queryableSource.GetQueryable<BusinessUnit>().Single(bu => bu.Id == searchBuIdentity.Id)],
                    ExpandType = NotificationExpandType.DirectOrFirstParent
                };

                return await extractor.GetPrincipalsAsync([this.testSecurityRole], [notificationFilterGroup]).Select(p => p.Name).ToArrayAsync(ct);
            });

        // Assert
        Assert.Equivalent(new[] { testUserName }, principalNames);
    }

    //[CommonFact]
    //public async Task VirtualPermissionTest()
    //{
    //    // Arrange
    //    var buIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2", ct);

    //    var notificationFilterGroup = new NotificationFilterGroup<Guid>
    //    {
    //        SecurityContextType = typeof(BusinessUnit),
    //        Idents = [buIdentity.Id],
    //        ExpandType = NotificationExpandType.DirectOrFirstParent
    //    };

    //    // Act
    //    var result = await this.GetEvaluator<INotificationPrincipalExtractor<Employee>>()
    //        .EvaluateAsync(TestingScopeMode.Read, async extractor =>
    //            await extractor.GetPrincipalsAsync([testSecurityRole], [notificationFilterGroup])
    //                .Select(employee => employee.Login)
    //                .ToArrayAsync(ct));

    //    // Assert

    //    return;

    //    //Assert.Equivalent(expectedResult, result.OrderBy(v => v.Name));
    //}

    private Task<string[]> GetNotificationPrincipalsAsync(NotificationFilterGroup[] notificationFilterGroups, CancellationToken ct) =>

        this.GetEvaluator<INotificationPrincipalExtractor<ExampleApp.Domain.Auth.General.Principal>>()
            .EvaluateAsync(TestingScopeMode.Read, async extractor =>
                await extractor.GetPrincipalsAsync([this.testSecurityRole], [.. notificationFilterGroups])
                    .Select(p => p.Name)
                    .ToArrayAsync(ct));

    private Task<TypedSecurityIdentity<Guid>> SaveBusinessUnit(string name, TypedSecurityIdentity<Guid>? parent, CancellationToken ct) =>

        this.AuthManager.SaveSecurityContextAsync<BusinessUnit, Guid>(async sp => new BusinessUnit
        {
            Name = name,
            Parent = parent == null ? null : await sp.GetRequiredService<ISecurityRepository<BusinessUnit>>().GetObjectAsync(parent, ct)
        }, ct);

    private Task<TypedSecurityIdentity<Guid>> SaveManagementUnit(string name, TypedSecurityIdentity<Guid>? parent, CancellationToken ct) =>

        this.AuthManager.SaveSecurityContextAsync<ManagementUnit, Guid>(async sp => new ManagementUnit
        {
            Name = name,
            Parent = parent == null ? null : await sp.GetRequiredService<ISecurityRepository<ManagementUnit>>().GetObjectAsync(parent, ct)
        }, ct);

    private Task<TypedSecurityIdentity<Guid>> SaveEmployee(string login, CancellationToken ct) =>

        this.AuthManager.SaveSecurityContextAsync<Employee, Guid>(() => new Employee
        {
            Login = login
        }, ct);
}