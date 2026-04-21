using CommonFramework.Testing;

using ExampleApp.Application;
using ExampleApp.Domain;

using SecuritySystem;
using SecuritySystem.Validation;

namespace ExampleApp.IntegrationTests;

public abstract class PermissionDelegationFromTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [CommonFact]
    public async Task SetRoleAsync_ShouldPreserveDelegatedFromIdentity(CancellationToken ct)
    {
        // Arrange
        var sourcePrincipalName = "DelegatedFromPrincipal";
        var targetPrincipalName = "TargetPrincipal";
        var delegatedFromPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()) };

        var subPermission = new TestPermission(ExampleSecurityRole.DefaultRole)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()), DelegatedFrom = delegatedFromPermission.Identity };

        await this.AuthManager.For(sourcePrincipalName).SetRoleAsync(delegatedFromPermission, ct);

        // Act
        var principalIdentity = await this.AuthManager.For(targetPrincipalName).SetRoleAsync(subPermission, ct);

        // Assert
        var managedPrincipal = await this.AuthManager.For(principalIdentity).GetPrincipalAsync(ct);

        var managedPermission = Assert.Single(managedPrincipal.Permissions);

        Assert.Equal(subPermission.Identity, managedPermission.Identity);
        Assert.Equal(subPermission.DelegatedFrom, managedPermission.DelegatedFrom);
    }

    [CommonFact]
    public async Task AddRoleAsync_ShouldThrow_WhenDelegatingToOriginalPrincipal(CancellationToken ct)
    {
        // Arrange
        var delegatedFromPermission = new TestPermission(SecurityRole.Administrator)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()) };

        var subPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { DelegatedFrom = delegatedFromPermission.Identity };


        var principalIdentity = await this.AuthManager.For("DelegatedFromPrincipal").SetRoleAsync(delegatedFromPermission, ct);

        // Act
        var error = await Assert.ThrowsAsync<SecuritySystemValidationException>(async () =>
            await this.AuthManager.For(principalIdentity).AddRoleAsync(subPermission, ct));

        // Assert

        Assert.Equal("Invalid delegation target: the permission cannot be delegated to its original principal", error.Message);
    }

    [CommonFact]
    public async Task SetRoleAsync_ShouldPreserveDelegatedFrom_WhenAssigningToChildBusinessUnit(CancellationToken ct)
    {
        // Arrange
        var sourcePrincipalName = "DelegatedFromPrincipal";
        var targetPrincipalName = "TargetPrincipal";
        var sourceBuIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>("TestRootBu", ct);
        var targetBuIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}1", ct);

        var delegatedFromPermission = new TestPermission(ExampleSecurityRole.DefaultRole)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()), BusinessUnit = sourceBuIdentity };

        var subPermission = new TestPermission(ExampleSecurityRole.DefaultRole)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()), BusinessUnit = targetBuIdentity, DelegatedFrom = delegatedFromPermission.Identity };


        await this.AuthManager.For(sourcePrincipalName).SetRoleAsync(delegatedFromPermission, ct);

        // Act
        var principalIdentity = await this.AuthManager.For(targetPrincipalName).SetRoleAsync(subPermission, ct);

        // Assert
        var managedPrincipal = await this.AuthManager.For(principalIdentity).GetPrincipalAsync(ct);

        var managedPermission = Assert.Single(managedPrincipal.Permissions);

        Assert.Equal(subPermission.Identity, managedPermission.Identity);
        Assert.Equal(subPermission.DelegatedFrom, managedPermission.DelegatedFrom);
        Assert.Equivalent(subPermission.Restrictions, managedPermission.Restrictions);
    }

    [CommonFact]
    public async Task SetRoleAsync_ShouldThrow_WhenDelegationExceedsSourceBusinessUnit(CancellationToken ct)
    {
        // Arrange
        var sourcePrincipalName = "DelegatedFromPrincipal";
        var targetPrincipalName = "TargetPrincipal";
        var sourceBuIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>("TestRootBu", ct);
        var invalidObjects = $"{nameof(BusinessUnit)}: Unrestricted";

        var delegatedFromPermission = new TestPermission(ExampleSecurityRole.DefaultRole)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()), BusinessUnit = sourceBuIdentity };

        var subPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { DelegatedFrom = delegatedFromPermission.Identity };


        await this.AuthManager.For(sourcePrincipalName).SetRoleAsync(delegatedFromPermission, ct);

        // Act
        var error = await Assert.ThrowsAsync<SecuritySystemValidationException>(async () =>
            await this.AuthManager.For(targetPrincipalName).SetRoleAsync(subPermission, ct));

        // Assert

        Assert.Equal(
            $"Invalid security context delegation: the security contexts of \"{targetPrincipalName}\" exceed those granted by \"{sourcePrincipalName}\": {invalidObjects}",
            error.Message);
    }

    [CommonFact]
    public async Task SetRoleAsync_WhenTargetBusinessUnitExceedsSource_ShouldFail(CancellationToken ct)
    {
        // Arrange
        var sourcePrincipalName = "DelegatedFromPrincipal";
        var targetPrincipalName = "TargetPrincipal";
        var sourceBuIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}1", ct);
        var targetBuIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>($"Test{nameof(BusinessUnit)}2", ct);
        var invalidObjects = $"{nameof(BusinessUnit)}: {targetBuIdentity.Id}";

        var delegatedFromPermission = new TestPermission(ExampleSecurityRole.DefaultRole)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()), BusinessUnit = sourceBuIdentity };

        var subPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { BusinessUnit = targetBuIdentity, DelegatedFrom = delegatedFromPermission.Identity };


        await this.AuthManager.For(sourcePrincipalName).SetRoleAsync(delegatedFromPermission, ct);

        // Act
        var error = await Assert.ThrowsAsync<SecuritySystemValidationException>(async () =>
            await this.AuthManager.For(targetPrincipalName).SetRoleAsync(subPermission, ct));

        // Assert

        Assert.Equal(
            $"Invalid security context delegation: the security contexts of \"{targetPrincipalName}\" exceed those granted by \"{sourcePrincipalName}\": {invalidObjects}",
            error.Message);
    }

    [CommonFact]
    public async Task SetRoleAsync_ShouldThrow_WhenDelegatedRoleIsNotSubsetOfSource(CancellationToken ct)
    {
        // Arrange
        var sourcePrincipalName = "DelegatedFromPrincipal";
        var targetPrincipalName = "TargetPrincipal";

        var sourceRole = ExampleSecurityRole.DefaultRole;
        var targetRole = SecurityRole.Administrator;

        var delegatedFromPermission = new TestPermission(sourceRole)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()) };

        var subPermission = new TestPermission(targetRole) { DelegatedFrom = delegatedFromPermission.Identity };

        await this.AuthManager.For(sourcePrincipalName).SetRoleAsync(delegatedFromPermission, ct);

        // Act
        var error = await Assert.ThrowsAsync<SecuritySystemValidationException>(async () =>
            await this.AuthManager.For(targetPrincipalName).SetRoleAsync(subPermission, ct));

        // Assert

        Assert.Equal(
            $"Invalid delegated permission role: the selected role \"{targetRole}\" is not a subset of \"{sourceRole}\"",
            error.Message);
    }

    [CommonFact]
    public async Task SetRoleAsync_ShouldThrow_WhenDelegatedPeriodIsNotSubsetOfSource(CancellationToken ct)
    {
        // Arrange
        var sourcePrincipalName = "DelegatedFromPrincipal";
        var targetPrincipalName = "TargetPrincipal";

        var today = DateTime.Today;

        var sourcePeriod = new PermissionPeriod(today, today);
        var targetPeriod = PermissionPeriod.Eternity with { StartDate = DateTime.MinValue };

        var expectedErrorMessage = $"Invalid delegated permission period: the selected period \"{targetPeriod}\" is not a subset of \"{sourcePeriod}\"";

        var delegatedFromPermission = new TestPermission(ExampleSecurityRole.DefaultRole)
            { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()), Period = sourcePeriod };

        var subPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { Period = targetPeriod, DelegatedFrom = delegatedFromPermission.Identity };

        await this.AuthManager.For(sourcePrincipalName).SetRoleAsync(delegatedFromPermission, ct);

        // Act
        var error = await Assert.ThrowsAsync<SecuritySystemValidationException>(async () =>
            await this.AuthManager.For(targetPrincipalName).SetRoleAsync(subPermission, ct));

        // Assert

        Assert.Equal(expectedErrorMessage, error.Message);
    }
}