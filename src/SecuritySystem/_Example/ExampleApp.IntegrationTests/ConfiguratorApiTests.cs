using CommonFramework.Testing;
using SecuritySystem;
using SecuritySystem.ExternalSystem.Management;

namespace ExampleApp.IntegrationTests;

public abstract class ConfiguratorApiTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [CommonFact]
    public async Task GetLinkedPrincipalsAsync_ReturnsAllPrincipalsLinkedToRole(CancellationToken ct)
    {
        // Arrange
        var securityRole = SecurityRole.Administrator;

        var additionalAdmin = nameof(this.GetLinkedPrincipalsAsync_ReturnsAllPrincipalsLinkedToRole);

        await this.AuthManager.For(additionalAdmin).AddRoleAsync(securityRole, ct);

        var expectedResults = new[] { additionalAdmin, this.AuthManager.RootUserName }.OrderBy(v => v);

        // Act
        var principalNames = await this.GetEvaluator<IRootPrincipalSourceService>()
            .EvaluateAsync(
                TestingScopeMode.Read,
                async service => await service.GetLinkedPrincipalsAsync([securityRole]).ToListAsync(ct));

        // Assert
        Assert.Equivalent(expectedResults, principalNames.OrderBy(v => v));
    }

    [CommonFact]
    public async Task GetPrincipalsAsync_ReturnsRootUserForRootPrincipal(CancellationToken ct)
    {
        // Arrange

        // Act
        var result = await this.GetEvaluator<IRootPrincipalSourceService>().EvaluateAsync(TestingScopeMode.Read, async service =>
        {
            return await service
                .GetPrincipalsAsync(this.AuthManager.RootUserName, 100).Select(ph => ph.Name)
                .ToListAsync(ct);
        });

        // Assert
        Assert.Equivalent(new[] { this.AuthManager.RootUserName }, result);
    }
}