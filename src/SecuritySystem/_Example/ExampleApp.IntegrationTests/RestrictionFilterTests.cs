using CommonFramework.Testing;
using ExampleApp.Application;
using ExampleApp.Domain;

using SecuritySystem;
using SecuritySystem.Validation;

namespace ExampleApp.IntegrationTests;

public abstract class RestrictionFilterTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    private readonly SecurityRole securityRole = ExampleSecurityRole.DefaultRole;

    private readonly SecurityRule restrictionRule = ExampleSecurityRole.DefaultRole.ToSecurityRule(
        customRestriction: SecurityPathRestriction.Default.Add<BusinessUnit>(filter: bu => bu.AllowedForFilterRole));

    private readonly string testLogin = "RestrictionFilterTests";

    private TypedSecurityIdentity<Guid> defaultBu = null!;

    private TypedSecurityIdentity<Guid> buWithAllowedFilter = null!;


    protected override async ValueTask InitializeAsync(CancellationToken ct)
    {
        await base.InitializeAsync(ct);

        this.defaultBu = await this.AuthManager.SaveSecurityContextAsync<BusinessUnit, Guid>(
            () => new BusinessUnit { Name = nameof(this.defaultBu) }, ct);

        this.buWithAllowedFilter = await this.AuthManager.SaveSecurityContextAsync<BusinessUnit, Guid>(
            () => new BusinessUnit { Name = nameof(this.buWithAllowedFilter), AllowedForFilterRole = true }, ct);
    }

    [CommonFact]
    public async Task CreatePermissionWithRestrictionFilter_ApplyInvalidBusinessUnit_ExceptionRaised(CancellationToken ct)
    {
        // Arrange

        // Act
        var error = await Assert.ThrowsAsync<SecuritySystemValidationException>(async () =>
            await this.AuthManager.For(this.testLogin).SetRoleAsync(
                new TestPermission(ExampleSecurityRole.WithRestrictionFilterRole)
                {
                    BusinessUnit = this.defaultBu
                }, ct));

        // Assert

        Assert.Contains($"SecurityContext: '{this.defaultBu.Id}' denied by filter", error.Message);
    }

    [CommonFact]
    public async Task CreatePermissionWithRestrictionFilter_ApplyCorrectBusinessUnit_ExceptionNotRaised(CancellationToken ct)
    {
        // Arrange

        // Act
        var ex = await Record.ExceptionAsync(async () =>
            await this.AuthManager.For(this.testLogin).SetRoleAsync(
                new TestPermission(ExampleSecurityRole.WithRestrictionFilterRole)
                {
                    BusinessUnit = this.buWithAllowedFilter
                }, ct));

        // Assert
        Assert.Null(ex);
    }


    [CommonFact]
    public async Task CreateCustomRestrictionRule_ApplyUnrestrictedPermission_OnlyCorrectBuFounded(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(this.testLogin).SetRoleAsync(this.securityRole, ct);
        this.AuthManager.For(this.testLogin).LoginAs();

        // Act
        var allowedBuList = await this.AuthManager.GetIdentityListAsync<BusinessUnit, Guid>(this.restrictionRule, ct);

        // Assert
        Assert.Equivalent(new[] { this.buWithAllowedFilter }, allowedBuList);
    }

    [CommonFact]
    public async Task CreateCustomRestrictionRule_ApplySingleCorrectBU_OnlyCorrectBuFounded(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For(this.testLogin).SetRoleAsync(new TestPermission(this.securityRole) { BusinessUnits = [this.defaultBu, this.buWithAllowedFilter] }, ct);
        this.AuthManager.For(this.testLogin).LoginAs();

        // Act
        var allowedBuList = await this.AuthManager.GetIdentityListAsync<BusinessUnit, Guid>(this.restrictionRule, ct);

        // Assert
        Assert.Equivalent(new[] { this.buWithAllowedFilter }, allowedBuList);
    }

    //[CommonFact]
    //public async Task CreateCustomRestrictionRule_SearchAccessorsForUnrestrictedPermission_EmployeeFounded()
    //{
    //    // Arrange
    //    await this.AuthManager.For(this.testLogin).SetRoleAsync(this.securityRole, ct);

    //    // Act
    //    await using var scope = rootServiceProvider.CreateAsyncScope();

    //    var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
    //    var domainSecurityService = scope.ServiceProvider.GetRequiredService<IDomainSecurityService<BusinessUnit>>();
    //    var securityAccessorResolver = scope.ServiceProvider.GetRequiredService<ISecurityAccessorResolver>();

    //    var bu = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Id == this.buWithAllowedFilter.Id).GenericSingleAsync(ct);

    //    var accessorData = domainSecurityService.GetSecurityProvider(this.restrictionRule).GetAccessorData(bu);

    //    var accessors = securityAccessorResolver.Resolve(accessorData).ToList();

    //    // Assert
    //    Assert.Contains(this.testLogin, accessors);
    //}

    //[CommonFact]
    //public async Task CreateCustomRestrictionRule_SearchAccessorsForCorrectBU_EmployeeFounded()
    //{
    //    // Arrange
    //    await this.AuthManager.For(this.testLogin)
    //        .SetRoleAsync(new TestPermissionBuilder(this.securityRole) { BusinessUnits = [this.defaultBu, this.buWithAllowedFilter] },
    //            ct);

    //    // Act
    //    await using var scope = rootServiceProvider.CreateAsyncScope();

    //    var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
    //    var domainSecurityService = scope.ServiceProvider.GetRequiredService<IDomainSecurityService<BusinessUnit>>();
    //    var securityAccessorResolver = scope.ServiceProvider.GetRequiredService<ISecurityAccessorResolver>();

    //    var bu = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Id == this.buWithAllowedFilter.Id).GenericSingleAsync(ct);

    //    var accessorData = domainSecurityService.GetSecurityProvider(this.restrictionRule).GetAccessorData(bu);

    //    var accessors = securityAccessorResolver.Resolve(accessorData).ToList();

    //    // Assert
    //    Assert.Contains(this.testLogin, accessors);
    //}

    //[CommonFact]
    //public async Task CreateCustomRestrictionRule_SearchAccessorsForIncorrectBU_EmployeeNotFounded()
    //{
    //    // Arrange
    //    await this.AuthManager.For(this.testLogin)
    //        .SetRoleAsync(new TestPermissionBuilder(this.securityRole) { BusinessUnits = [this.defaultBu, this.buWithAllowedFilter] },
    //            ct);

    //    // Act
    //    await using var scope = rootServiceProvider.CreateAsyncScope();

    //    var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
    //    var domainSecurityService = scope.ServiceProvider.GetRequiredService<IDomainSecurityService<BusinessUnit>>();
    //    var securityAccessorResolver = scope.ServiceProvider.GetRequiredService<ISecurityAccessorResolver>();

    //    var bu = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Id == this.defaultBu.Id).GenericSingleAsync(ct);

    //    var accessorData = domainSecurityService.GetSecurityProvider(this.restrictionRule).GetAccessorData(bu);

    //    var accessors = securityAccessorResolver.Resolve(accessorData).ToList();

    //    // Assert
    //    Assert.DoesNotContain(this.testLogin, accessors);
    //}
}
