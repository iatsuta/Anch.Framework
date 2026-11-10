using Anch.SecuritySystem;
using Anch.Testing.Xunit;

using ExampleApp.Application;

namespace ExampleApp.IntegrationTests;

public abstract class SecuritySystemRuleTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    private static SecurityRole Def => ExampleSecurityRole.DefaultRole;

    private static SecurityRole Oth => ExampleSecurityRole.OtherRole;

    private async Task<bool> HasAccessAsync(string user, DomainSecurityRule rule, CancellationToken ct) =>
        await this.GetEvaluator<ISecuritySystem>().EvaluateAsync(
            TestingScopeMode.Read,
            ss => ss.HasAccessAsync(rule with { CustomCredential = user }, ct));

    private async Task CreateUserAsync(string user, SecurityRole[] roles, CancellationToken ct)
    {
        foreach (var role in roles)
        {
            await this.AuthManager.For(user).AddRoleAsync(role, ct);
        }
    }

    [Theory]
    [AnchMemberData(nameof(HasAccess_RuleCombinations_ExpectedResult_Cases))]
    public async Task HasAccess_RuleCombinations_ExpectedResult(
        string caseName,
        SecurityRole[] userRoles,
        Func<DomainSecurityRule> ruleFactory,
        bool expected,
        CancellationToken ct)
    {
        // Arrange
        var user = $"ss_rule_{caseName}";
        await this.CreateUserAsync(user, userRoles, ct);

        // Act
        var result = await this.HasAccessAsync(user, ruleFactory(), ct);

        // Assert
        Assert.Equal(expected, result);
    }

    public IEnumerable<object?[]> HasAccess_RuleCombinations_ExpectedResult_Cases()
    {
        // single role
        yield return ["single_has", new[] { Def }, () => Def.ToSecurityRule(), true];
        yield return ["single_hasnt", new[] { Oth }, () => Def.ToSecurityRule(), false];

        // roles array = any of
        yield return ["any_has_one", new[] { Oth }, () => new[] { Def, Oth }.ToSecurityRule(), true];
        yield return ["any_has_none", new[] { ExampleSecurityRole.NotificationRole }, () => new[] { Def, Oth }.ToSecurityRule(), false];

        // Or
        yield return ["or_right_only", new[] { Oth }, () => Def.Or(Oth), true];
        yield return ["or_left_only", new[] { Def }, () => Def.Or(Oth), true];
        yield return ["or_none", new[] { ExampleSecurityRole.NotificationRole }, () => Def.Or(Oth), false];

        // And
        yield return ["and_both", new[] { Def, Oth }, () => Def.And(Oth), true];
        yield return ["and_left_only", new[] { Def }, () => Def.And(Oth), false];
        yield return ["and_right_only", new[] { Oth }, () => Def.And(Oth), false];

        // Negate
        yield return ["negate_has", new[] { Def }, () => Def.Negate(), false];
        yield return ["negate_hasnt", new[] { Oth }, () => Def.Negate(), true];

        // Except
        yield return ["except_only_left", new[] { Def }, () => Def.Except(Oth), true];
        yield return ["except_both", new[] { Def, Oth }, () => Def.Except(Oth), false];
        yield return ["except_none", new[] { ExampleSecurityRole.NotificationRole }, () => Def.Except(Oth), false];

        // nested
        yield return ["nested_or_and", new[] { Oth, ExampleSecurityRole.NotificationRole }, () => Def.Or(Oth.And(ExampleSecurityRole.NotificationRole)), true];
        yield return ["nested_or_and_fail", new[] { Oth }, () => Def.Or(Oth.And(ExampleSecurityRole.NotificationRole)), false];
        yield return ["nested_negate_or", new[] { ExampleSecurityRole.NotificationRole }, () => Def.Or(Oth).Negate(), true];

        // CurrentUser is not applicable without a domain object: it is "indifferent" (null) in the three-valued logic.
        var cu = DomainSecurityRule.CurrentUser;
        var notif = ExampleSecurityRole.NotificationRole;

        // role.Except(CurrentUser) == role
        yield return ["cu_except_has", new[] { Def }, () => Def.Except(cu), true];
        yield return ["cu_except_hasnt", new[] { Oth }, () => Def.Except(cu), false];

        // role.Or(CurrentUser): the undecided branch makes the result permissive
        yield return ["cu_or_has", new[] { Def }, () => Def.Or(cu), true];
        yield return ["cu_or_hasnt", new[] { Oth }, () => Def.Or(cu), true];

        // role.And(CurrentUser) == role
        yield return ["cu_and_has", new[] { Def }, () => Def.And(cu), true];
        yield return ["cu_and_hasnt", new[] { Oth }, () => Def.And(cu), false];

        // CurrentUser alone and negated: undecided => permissive
        yield return ["cu_alone", new[] { Oth }, () => cu, true];
        yield return ["cu_negate", new[] { Oth }, () => cu.Negate(), true];

        // complex combinations
        yield return ["cu_anyof_except_has", new[] { Oth }, () => Def.Or(Oth).Except(cu), true];
        yield return ["cu_anyof_except_hasnt", new[] { notif }, () => Def.Or(Oth).Except(cu), false];

        yield return ["cu_except_or_role_left", new[] { Def }, () => Def.Except(cu).Or(Oth), true];
        yield return ["cu_except_or_role_right", new[] { Oth }, () => Def.Except(cu).Or(Oth), true];
        yield return ["cu_except_or_role_none", new[] { notif }, () => Def.Except(cu).Or(Oth), false];

        yield return ["cu_and_or_cu_hasnt", new[] { notif }, () => Def.And(Oth).Or(cu), true];

        yield return ["cu_or_and_role_has", new[] { Oth }, () => Def.Or(cu).And(Oth), true];
        yield return ["cu_or_and_role_hasnt", new[] { Def }, () => Def.Or(cu).And(Oth), false];

        yield return ["cu_except_and_role_both", new[] { Def, Oth }, () => Def.Except(cu).And(Oth), true];
        yield return ["cu_except_and_role_one", new[] { Def }, () => Def.Except(cu).And(Oth), false];

        yield return ["cu_except_role_and_cu", new[] { Def }, () => Def.Except(Oth).Except(cu), true];
        yield return ["cu_except_role_and_cu_both", new[] { Def, Oth }, () => Def.Except(Oth).Except(cu), false];
    }

    [AnchFact]
    public async Task CheckAccess_AccessDenied_Throws(CancellationToken ct)
    {
        // Arrange
        var user = "ss_rule_check_denied";
        await this.CreateUserAsync(user, [Oth], ct);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => this.GetEvaluator<ISecuritySystem>().EvaluateAsync(
            TestingScopeMode.Read,
            ss => ss.CheckAccessAsync(Def.ToSecurityRule() with { CustomCredential = user }, ct)));
    }

    [AnchFact]
    public async Task CheckAccess_AccessGranted_DoesNotThrow(CancellationToken ct)
    {
        // Arrange
        var user = "ss_rule_check_granted";
        await this.CreateUserAsync(user, [Def], ct);

        // Act & Assert
        await this.GetEvaluator<ISecuritySystem>().EvaluateAsync(
            TestingScopeMode.Read,
            ss => ss.CheckAccessAsync(Def.Or(Oth) with { CustomCredential = user }, ct));
    }
}
