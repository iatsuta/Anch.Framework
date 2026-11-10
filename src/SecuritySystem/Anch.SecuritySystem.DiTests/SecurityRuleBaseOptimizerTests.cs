using Anch.SecuritySystem.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DiTests;

public class SecurityRuleBaseOptimizerTests(IServiceProvider rootServiceProvider)
{
    [Theory]
    [MemberData(nameof(OptimizeSecurityRule_RuleOptimized_Data))]
    public void OptimizeSecurityRule_RuleOptimized(DomainSecurityRule securityRule, DomainSecurityRule expectedOptimizedSecurityRule)
    {
        //Arrange
        var service = rootServiceProvider.GetRequiredKeyedService<ISecurityRuleOptimizer>(ISecurityRuleOptimizer.BasicKey);

        //Act
        var optimizedSecurityPath = service.Optimize(securityRule);

        //Assert
        Assert.Equal(expectedOptimizedSecurityRule, optimizedSecurityPath);
    }

    public static IEnumerable<object[]> OptimizeSecurityRule_RuleOptimized_Data() =>
        GetCases().Select(pair => new[] { pair.Item1, pair.Item2 });

    private static IEnumerable<(DomainSecurityRule, DomainSecurityRule)> GetCases()
    {
        yield return (DomainSecurityRule.CurrentUser.And(SecurityRule.Disabled), DomainSecurityRule.CurrentUser);
        yield return (SecurityRule.Disabled.And(DomainSecurityRule.CurrentUser), DomainSecurityRule.CurrentUser);

        yield return (DomainSecurityRule.CurrentUser.Or(SecurityRule.Disabled), SecurityRule.Disabled);

        yield return (SecurityRole.Administrator.Or(SecurityRole.Administrator), SecurityRole.Administrator);

        yield return
        (
            SecurityRole.Administrator.Or(SecurityRole.SystemIntegration),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration }
        );

        yield return
        (
            SecurityRole.Administrator.Or(DomainSecurityRule.CurrentUser.Or(SecurityRole.SystemIntegration)),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration }.Or(DomainSecurityRule.CurrentUser)
        );
        yield return
        (
            SecurityRole.Administrator.Or(SecurityRole.SystemIntegration.Or(DomainSecurityRule.CurrentUser)),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration }.Or(DomainSecurityRule.CurrentUser)
        );

        yield return
        (
            SecurityRole.Administrator.Or(DomainSecurityRule.CurrentUser).Or(SecurityRole.SystemIntegration),
            DomainSecurityRule.CurrentUser.Or(new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration })
        );

        yield return
        (
            DomainSecurityRule.CurrentUser.Or(SecurityRole.Administrator).Or(SecurityRole.SystemIntegration),
            DomainSecurityRule.CurrentUser.Or(new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration })
        );

        yield return
        (
            DomainSecurityRule.CurrentUser.Negate().Negate(),
            DomainSecurityRule.CurrentUser
        );

        // Three and four roles
        var role3 = new SecurityRole("TestRole3");
        var role4 = new SecurityRole("TestRole4");

        yield return
        (
            SecurityRole.Administrator.Or(SecurityRole.SystemIntegration).Or(role3),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration, role3 }
        );

        yield return
        (
            SecurityRole.Administrator.Or(SecurityRole.SystemIntegration.Or(role3)),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration, role3 }
        );

        yield return
        (
            SecurityRole.Administrator.Or(SecurityRole.SystemIntegration).Or(role3).Or(role4),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration, role3, role4 }
        );

        yield return
        (
            SecurityRole.Administrator.Or(SecurityRole.SystemIntegration.Or(role3.Or(role4))),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration, role3, role4 }
        );

        yield return
        (
            SecurityRole.Administrator.Or(SecurityRole.SystemIntegration).Or(role3.Or(role4)),
            new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration, role3, role4 }
        );

        yield return
        (
            SecurityRole.Administrator.Or(DomainSecurityRule.CurrentUser).Or(SecurityRole.SystemIntegration).Or(role3),
            DomainSecurityRule.CurrentUser.Or(new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration, role3 })
        );

        yield return
        (
            DomainSecurityRule.CurrentUser.Or(SecurityRole.Administrator.Or(SecurityRole.SystemIntegration.Or(role3))),
            DomainSecurityRule.CurrentUser.Or(new[] { SecurityRole.Administrator, SecurityRole.SystemIntegration, role3 })
        );
    }
}