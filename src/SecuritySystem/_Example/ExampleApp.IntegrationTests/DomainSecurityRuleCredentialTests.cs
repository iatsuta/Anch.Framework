using Anch.GenericQueryable;
using Anch.SecuritySystem;
using Anch.Testing.Xunit;

using ExampleApp.Application;
using ExampleApp.Domain;

namespace ExampleApp.IntegrationTests;

public abstract class DomainSecurityRuleCredentialTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [Theory]
    [AnchMemberData(nameof(GetEmployees_ReturnsExpectedUsers_Cases))]
    public async Task GetEmployees_ReturnsExpectedUsers(SecurityRule securityRule, string?[] expectedUsers, CancellationToken ct)
    {
        // Arrange
        var realExpectedUsers = expectedUsers.Select(userName => userName ?? this.AuthManager.RootUserName);

        // Act
        var employees = await this.GetEvaluator<IRepositoryFactory<Employee>>().EvaluateAsync(TestingScopeMode.Read,
            async rep =>
                await rep.Create(securityRule).GetQueryable().GenericToListAsync(ct));

        // Assert
        Assert.Equivalent(realExpectedUsers.OrderBy(v => v), employees.Select(e => e.Login));
    }

    public IEnumerable<object?[]> GetEmployees_ReturnsExpectedUsers_Cases()
    {
        var user0 = default(string?);
        var user1 = "TestEmployee1";
        var user2 = "TestEmployee2";

        yield return
        [
            DomainSecurityRule.CurrentUser,
            new[] { user0 }
        ];

        yield return
        [
            DomainSecurityRule.CurrentUser with { CustomCredential = user1 },
            new[] { user1 }
        ];

        yield return
        [
            (DomainSecurityRule.CurrentUser with { CustomCredential = user1 })
            .Or(DomainSecurityRule.CurrentUser with { CustomCredential = user2 }),
            new[] { user1, user2 }
        ];

        yield return
        [
            (DomainSecurityRule.CurrentUser with { CustomCredential = user0 })
                .Or(DomainSecurityRule.CurrentUser with { CustomCredential = user1 })
                with
                {
                    CustomCredential = user2
                },
            new[] { user2 }
        ];
    }
}