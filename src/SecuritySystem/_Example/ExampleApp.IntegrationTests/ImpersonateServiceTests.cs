using CommonFramework.Auth;

using ExampleApp.Application;

namespace ExampleApp.IntegrationTests;

public abstract class ImpersonateServiceTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [Fact]
    public async Task EvaluateAsync_ShouldReturnImpersonatedUserName()
    {
        // Arrange
        var userName = nameof(this.EvaluateAsync_ShouldReturnImpersonatedUserName);
        var user = await this.AuthManager.For(userName).SetRoleAsync(ExampleSecurityRole.DefaultRole, this.CancellationToken);

        // Act
        var result = await this.GetEvaluator<ICurrentUser>().EvaluateAsync(TestingScopeMode.Read, user, async service => service.Name);

        // Assert
        result.Should().Be(userName);
    }
}