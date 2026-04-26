using Anch.Core.Auth;
using Anch.SecuritySystem.Testing;
using ExampleApp.Application;

namespace ExampleApp.IntegrationTests;

public abstract class ImpersonateServiceTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [AnchFact]
    public async Task EvaluateAsync_ShouldReturnImpersonatedUserName(CancellationToken ct)
    {
        // Arrange
        var userName = nameof(this.EvaluateAsync_ShouldReturnImpersonatedUserName);
        var user = await this.AuthManager.For(userName).SetRoleAsync(ExampleSecurityRole.DefaultRole, ct);

        // Act
        var result = await this.GetEvaluator<ICurrentUser>().EvaluateAsync(TestingScopeMode.Read, user, async service => service.Name);

        // Assert
        Assert.Equal(userName, result);
    }
}