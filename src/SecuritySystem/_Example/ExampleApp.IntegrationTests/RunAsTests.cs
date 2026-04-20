using CommonFramework.Auth;
using CommonFramework.Testing;
using ExampleApp.Domain.Auth.General;

using Microsoft.Extensions.DependencyInjection;
using SecuritySystem.Services;
using SecuritySystem.UserSource;

namespace ExampleApp.IntegrationTests;

public abstract class RunAsManagerTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [CommonFact]
    public async Task StartRunAsUser_AssignsRunAsPrincipalToCurrentUser(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For().CreatePrincipalAsync(ct);

        var runAsUserName = nameof(RunAsManagerTests);
        var runAsUserIdentity = await this.AuthManager.For(runAsUserName).CreatePrincipalAsync(ct);
        var runAsUserId = (Guid)runAsUserIdentity.GetId();

        // Act
        await this.RootServiceProvider.GetRequiredService<ITestingEvaluator<IRunAsManager>>().EvaluateAsync(TestingScopeMode.Write, manager =>
            manager.StartRunAsUserAsync(runAsUserIdentity, ct));

        // Assert
        var currentUserName = await this.RootServiceProvider.GetRequiredService<ITestingEvaluator<ICurrentUser>>()
            .EvaluateAsync(TestingScopeMode.Read, async c => c.Name);

        var currentUserId = await this.RootServiceProvider.GetRequiredService<ITestingEvaluator<ICurrentUserSource<Principal>>>()
            .EvaluateAsync(TestingScopeMode.Read, async c => c.CurrentUser.Id);

        currentUserName.Should().Be(runAsUserName);
        currentUserId.Should().Be(runAsUserId);
    }


    [CommonFact]
    public async Task StartRunAsUser_WhenAlreadyRunningAsUser_DoesNotChangeRunAs(CancellationToken ct)
    {
        // Arrange
        await this.AuthManager.For().CreatePrincipalAsync(ct);

        var runAsUserName = nameof(RunAsManagerTests);
        var runAsUserIdentity = await this.AuthManager.For(runAsUserName).CreatePrincipalAsync(ct);
        var runAsUserId = (Guid)runAsUserIdentity.GetId();

        // Act
        await this.RootServiceProvider.GetRequiredService<ITestingEvaluator<IRunAsManager>>().EvaluateAsync(TestingScopeMode.Write, manager =>
            manager.StartRunAsUserAsync(runAsUserIdentity, ct));

        await this.RootServiceProvider.GetRequiredService<ITestingEvaluator<IRunAsManager>>().EvaluateAsync(TestingScopeMode.Write, manager =>
            manager.StartRunAsUserAsync(runAsUserIdentity, ct));

        // Assert
        var currentUserName = await this.RootServiceProvider.GetRequiredService<ITestingEvaluator<ICurrentUser>>()
            .EvaluateAsync(TestingScopeMode.Read, async c => c.Name);

        var currentUserId = await this.RootServiceProvider.GetRequiredService<ITestingEvaluator<ICurrentUserSource<Principal>>>()
            .EvaluateAsync(TestingScopeMode.Read, async c => c.CurrentUser.Id);

        currentUserName.Should().Be(runAsUserName);
        currentUserId.Should().Be(runAsUserId);
    }
}
