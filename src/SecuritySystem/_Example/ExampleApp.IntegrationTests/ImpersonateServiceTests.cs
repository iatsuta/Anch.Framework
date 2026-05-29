using Anch.Core.Auth;
using Anch.SecuritySystem;
using Anch.SecuritySystem.UserSource;
using Anch.Testing.Xunit;

using ExampleApp.Application;
using ExampleApp.Domain.Auth.General;

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


    [AnchFact]
    public async Task EvaluateAsync_ShouldResolveUserWithImpersonatedIdAndName(CancellationToken ct)
    {
        // Arrange
        var userName = nameof(this.EvaluateAsync_ShouldReturnImpersonatedUserName);
        var userId = Guid.NewGuid();
        var user = new User(userName, userId);

        var userSecurityIdentity = await this.AuthManager.For(user).SetRoleAsync(ExampleSecurityRole.DefaultRole, ct);

        // Act
        var result = await this.GetEvaluator<ICurrentUserSource<Principal>>()
            .EvaluateAsync(TestingScopeMode.Read, user, async service => service.ToSimple().CurrentUser);

        // Assert
        Assert.Equal(user, result);
        Assert.Equal(user.Identity, userSecurityIdentity);
    }
}