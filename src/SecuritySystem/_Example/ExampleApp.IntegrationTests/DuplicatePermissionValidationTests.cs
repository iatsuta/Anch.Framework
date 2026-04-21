using CommonFramework.Testing;
using ExampleApp.Application;
using ExampleApp.Domain;

using SecuritySystem;
using SecuritySystem.Validation;

namespace ExampleApp.IntegrationTests;

public abstract class DuplicatePermissionValidationTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [CommonFact]
    public async Task AddRoleAsync_WhenDuplicatePermissionExists_ShouldThrowValidationException(CancellationToken ct)
    {
        // Arrange
        var principalName = "TestPrincipal";
        var buIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>("TestRootBu", ct);

        Task<SecurityIdentity> Assign() => this.AuthManager.For(principalName)
            .AddRoleAsync(new TestPermission(ExampleSecurityRole.BuManager) { BusinessUnit = buIdentity }, ct);

        await Assign();

        // Act
        var error = await Assert.ThrowsAsync<SecuritySystemValidationException>(async () => await Assign());

        // Assert

        Assert.Contains($"Principal \"{principalName}\" has duplicate permissions", error.Message);
    }


    [CommonFact]
    public async Task AddRoleAsync_WhenPermissionPeriodsDoNotIntersect_ShouldNotThrow(CancellationToken ct)
    {
        // Arrange
        var principalName = "TestPrincipal";
        var buIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>("TestRootBu", ct);

        Task<SecurityIdentity> Assign(PermissionPeriod period) => this.AuthManager.For(principalName)
            .AddRoleAsync(new TestPermission(ExampleSecurityRole.BuManager) { BusinessUnit = buIdentity, Period = period }, ct);

        await Assign(new PermissionPeriod(new DateTime(2000, 1, 1), new DateTime(2009, 1, 1)));

        // Act
        var ex = await Record.ExceptionAsync(async () => await Assign(new PermissionPeriod(new DateTime(2010, 1, 1), new DateTime(2019, 1, 1))));

        // Assert
        Assert.Null(ex);
    }
}