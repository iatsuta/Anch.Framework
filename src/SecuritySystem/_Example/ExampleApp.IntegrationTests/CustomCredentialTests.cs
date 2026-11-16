using Anch.SecuritySystem;
using Anch.Testing.Xunit;

using ExampleApp.Application;

namespace ExampleApp.IntegrationTests;

public abstract class CustomCredentialTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [AnchFact]
    public async Task HasAccessAsync_ChecksAccessForCustomCredentialUser(CancellationToken ct)
    {
        //Arrange
        var user1 = "custom_cred_user1";
        var user2 = "custom_cred_user2";

        await this.AuthManager.For(user1).AddRoleAsync(ExampleSecurityRole.DefaultRole, ct);
        await this.AuthManager.For(user2).AddRoleAsync(ExampleSecurityRole.OtherRole, ct);

        //Act
        var user1Res = await this.GetEvaluator<ISecuritySystem>().EvaluateAsync(TestingScopeMode.Read,
            async ss => (
                Def: await ss.HasAccessAsync(ExampleSecurityRole.DefaultRole.ToSecurityRule() with { CustomCredential = user1 }, ct),
                Oth: await ss.HasAccessAsync(ExampleSecurityRole.OtherRole.ToSecurityRule() with { CustomCredential = user1 }, ct)));

        var user2Res = await this.GetEvaluator<ISecuritySystem>().EvaluateAsync(TestingScopeMode.Read,
            async ss => (
                Def: await ss.HasAccessAsync(ExampleSecurityRole.DefaultRole.ToSecurityRule() with { CustomCredential = user2 }, ct),
                Oth: await ss.HasAccessAsync(ExampleSecurityRole.OtherRole.ToSecurityRule() with { CustomCredential = user2 }, ct)));

        //Assert
        Assert.True(user1Res.Def);
        Assert.False(user1Res.Oth);
        Assert.False(user2Res.Def);
        Assert.True(user2Res.Oth);
    }
}