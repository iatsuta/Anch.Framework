using CommonFramework.Testing;

using ExampleApp.Application;
using ExampleApp.Domain;

using GenericQueryable;

using SecuritySystem;

namespace ExampleApp.IntegrationTests;

public abstract class DomainSecurityRuleCredentialTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [Theory]
    [CommonMemberData(nameof(GetEmployees_ReturnsExpectedUsers_Cases))]
    public async Task GetEmployees_ReturnsExpectedUsers(SecurityRule securityRule, string?[] expectedUsers, CancellationToken ct)
    {
        // Arrange
        var realExpectedUsers = expectedUsers.Select(userName => userName ?? this.AuthManager.RootUserName);

        // Act
        var employees = await this.GetEvaluator<IRepositoryFactory<Employee>>().EvaluateAsync(TestingScopeMode.Read,
            async rep =>
                await rep.Create(securityRule).GetQueryable().GenericToListAsync(ct));

        // Assert
        realExpectedUsers.OrderBy(v => v).Should().BeEquivalentTo(employees.Select(e => e.Login));
    }

    public IEnumerable<object?[]> GetEmployees_ReturnsExpectedUsers_Cases()
    {
        string? user0 = null;
        string? user1 = "TestEmployee1";
        string? user2 = "TestEmployee2";


        yield return
        [
            DomainSecurityRule.CurrentUser,
            new[] { user0 }
        ];

        yield return
        [
            DomainSecurityRule.CurrentUser with { CustomCredential = new SecurityRuleCredential.CustomUserSecurityRuleCredential(user1) },
            new[] { user1 }
        ];

        yield return
        [
            (DomainSecurityRule.CurrentUser with { CustomCredential = new SecurityRuleCredential.CustomUserSecurityRuleCredential(user1) })
            .Or(DomainSecurityRule.CurrentUser with { CustomCredential = new SecurityRuleCredential.CustomUserSecurityRuleCredential(user2) }),
            new[] { user1, user2 }
        ];

        yield return
        [
            (DomainSecurityRule.CurrentUser with { CustomCredential = new SecurityRuleCredential.CustomUserSecurityRuleCredential(user0) })
                .Or(DomainSecurityRule.CurrentUser with { CustomCredential = new SecurityRuleCredential.CustomUserSecurityRuleCredential(user1) })
                with
                {
                    CustomCredential = new SecurityRuleCredential.CustomUserSecurityRuleCredential(user2)
                },
            new[] { user2 }
        ];
    }
}