using CommonFramework.Testing;

using ExampleApp.Application;

namespace ExampleApp.IntegrationTests;

public abstract class PermissionExtendedDataTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [CommonFact]
    public async Task SetRoleAsync_WithExtendedValue_ShouldPersistExtendedData(CancellationToken ct)
    {
        // Arrange
        var principalName = "TestPrincipal";

        var extendedValue = "abc";

        var testPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { ExtendedValue = extendedValue };

        // Act
        var principalIdentity = await this.AuthManager.For(principalName).SetRoleAsync(testPermission, ct);

        // Assert
        var managedPrincipal = await this.AuthManager.For(principalIdentity).GetPrincipalAsync(ct);

        var managedPermission = Assert.Single(managedPrincipal.Permissions);

        Assert.Equal(
            extendedValue,
            managedPermission.ExtendedData.GetValueOrDefault(TestPermissionExtensions.ExtendedKey));
    }
}