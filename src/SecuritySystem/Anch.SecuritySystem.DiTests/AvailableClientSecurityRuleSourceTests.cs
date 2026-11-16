using System.Collections.Immutable;

using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.AvailableSecurity;
using Anch.SecuritySystem.DiTests.Rules;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.SecurityRuleInfo;
using Anch.SecuritySystem.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DiTests;

/// <summary>
/// Checks where <see cref="AvailableClientSecurityRuleSource"/> sends the access checks: primitive rules are matched against the cached available roles,
/// custom rules (custom restriction or credential) go to the permission systems one by one.
/// </summary>
public class AvailableClientSecurityRuleSourceTests(IServiceProvider rootServiceProvider)
{
    private static readonly SecurityRuleCredential.AnyUserCredential CustomCredential = new();

    [Fact]
    public async Task GetAvailableSecurityRules_PrimitiveRuleAndUserHasRole_RuleAvailableViaRoleCache()
    {
        // Arrange
        var env = this.CreateEnvironment([ExampleSecurityRole.TestRole], Info("Primitive", ExampleSecurityRole.TestRole.ToSecurityRule()));

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Equal(["Primitive"], result);

        env.AssertRolesLoaded(1);
        env.AssertPermissionSystemNotUsed();
    }

    [Fact]
    public async Task GetAvailableSecurityRules_PrimitiveRuleAndUserHasNoRole_RuleExcludedViaRoleCache()
    {
        // Arrange
        var env = this.CreateEnvironment([ExampleSecurityRole.TestRole2], Info("Primitive", ExampleSecurityRole.TestRole.ToSecurityRule()));

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Empty(result);

        env.AssertRolesLoaded(1);
        env.AssertPermissionSystemNotUsed();
    }

    [Fact]
    public async Task GetAvailableSecurityRules_SeveralPrimitiveRules_RolesLoadedOnce()
    {
        // Arrange
        var env = this.CreateEnvironment(
            [ExampleSecurityRole.TestRole],
            Info("Allowed", ExampleSecurityRole.TestRole.ToSecurityRule()),
            Info("AllowedAny", new[] { ExampleSecurityRole.TestRole, ExampleSecurityRole.TestRole2 }.ToSecurityRule()),
            Info("Denied", ExampleSecurityRole.TestRole3.ToSecurityRule()));

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Equal(["Allowed", "AllowedAny"], result);

        env.AssertRolesLoaded(1);
        env.AssertPermissionSystemNotUsed();
    }

    [Fact]
    public async Task GetAvailableSecurityRules_RepeatedEnumeration_RolesLoadedOnce()
    {
        // Arrange
        var env = this.CreateEnvironment([ExampleSecurityRole.TestRole], Info("Primitive", ExampleSecurityRole.TestRole.ToSecurityRule()));

        // Act
        await env.GetAvailableNamesAsync();
        await env.GetAvailableNamesAsync();

        // Assert
        env.AssertRolesLoaded(1);
        env.AssertPermissionSystemNotUsed();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAvailableSecurityRules_OperationRule_ExpandedToRolesAndCheckedViaRoleCache(bool userHasRole)
    {
        // Arrange: the operation is expanded by the access optimizer into the non-expanded rule of TestRole4.
        var env = this.CreateEnvironment(
            userHasRole ? [ExampleSecurityRole.TestRole4] : [ExampleSecurityRole.TestRole2],
            Info("Operation", ExampleSecurityOperation.BusinessUnitView.ToSecurityRule()));

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Equal(userHasRole ? ["Operation"] : [], result);

        env.AssertRolesLoaded(1);
        env.AssertPermissionSystemNotUsed();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAvailableSecurityRules_CustomRestrictionRule_CheckedByPermissionSystem(bool permissionSourceResult)
    {
        // Arrange
        var rule = ExampleSecurityRole.TestRole.ToSecurityRule() with { CustomRestriction = SecurityPathRestriction.Ignored };

        var env = this.CreateEnvironment([ExampleSecurityRole.TestRole], Info("Custom", rule));

        env.SetPermissionSourceResult(permissionSourceResult);

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Equal(permissionSourceResult ? ["Custom"] : [], result);

        env.AssertRolesLoaded(0);
        env.AssertPermissionSystemUsed(permissionSourcesRequested: 1, accessChecks: 1);
    }

    [Fact]
    public async Task GetAvailableSecurityRules_CustomCredentialRule_CheckedByPermissionSystem()
    {
        // Arrange
        var rule = ExampleSecurityRole.TestRole.ToSecurityRule() with { CustomCredential = CustomCredential };

        var env = this.CreateEnvironment([ExampleSecurityRole.TestRole], Info("Custom", rule));

        env.SetPermissionSourceResult(true);

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Equal(["Custom"], result);

        env.AssertRolesLoaded(0);
        env.AssertPermissionSystemUsed(permissionSourcesRequested: 1, accessChecks: 1);
    }

    [Fact]
    public async Task GetAvailableSecurityRules_PrimitiveAndCustomRules_CustomRuleOnlyCostsPermissionSystemCall()
    {
        // Arrange
        var custom = ExampleSecurityRole.TestRole2.ToSecurityRule() with { CustomRestriction = SecurityPathRestriction.Ignored };

        var env = this.CreateEnvironment(
            [ExampleSecurityRole.TestRole],
            Info("Primitive1", ExampleSecurityRole.TestRole.ToSecurityRule()),
            Info("Custom", custom),
            Info("Primitive2", ExampleSecurityRole.TestRole3.ToSecurityRule()));

        env.SetPermissionSourceResult(true);

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Equal(["Primitive1", "Custom"], result);

        env.AssertRolesLoaded(1);
        env.AssertPermissionSystemUsed(permissionSourcesRequested: 1, accessChecks: 1);
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public async Task GetAvailableSecurityRules_OrOfPrimitiveAndCustom_CustomPartCheckedOnlyWhenPrimitiveFails(bool userHasPrimitiveRole, int expectedPermissionChecks)
    {
        // Arrange
        var custom = ExampleSecurityRole.TestRole2.ToSecurityRule() with { CustomRestriction = SecurityPathRestriction.Ignored };

        var env = this.CreateEnvironment(
            userHasPrimitiveRole ? [ExampleSecurityRole.TestRole] : [ExampleSecurityRole.TestRole3],
            Info("Or", ExampleSecurityRole.TestRole.ToSecurityRule().Or(custom)));

        env.SetPermissionSourceResult(true);

        // Act
        var result = await env.GetAvailableNamesAsync();

        // Assert
        Assert.Equal(["Or"], result);

        env.AssertRolesLoaded(1);
        env.AssertPermissionSystemUsed(permissionSourcesRequested: expectedPermissionChecks, accessChecks: expectedPermissionChecks);
    }

    private TestEnvironmentContext CreateEnvironment(SecurityRole[] availableRoles, params ClientSecurityRuleInfo[] infos)
    {
        var roleSource = Substitute.For<IAvailableSecurityRoleSource>();

        roleSource.GetAvailableSecurityRoles(Arg.Any<bool>())
                  .Returns(_ => availableRoles.Select(role => new FullSecurityRole(role.Name, new SecurityRoleInfo(Guid.NewGuid()))).ToAsyncEnumerable());

        var permissionSource = Substitute.For<IPermissionSource>();
        permissionSource.HasAccessAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        var permissionSystem = Substitute.For<IPermissionSystem>();
        permissionSystem.GetPermissionSources(Arg.Any<DomainSecurityRule.RoleBaseSecurityRule>()).Returns([permissionSource]);

        var securitySystem = new SecuritySystem(
            rootServiceProvider.GetRequiredKeyedService<ISecurityRuleOptimizer>(ISecurityRuleOptimizer.AccessKey),
            Substitute.For<IAccessDeniedExceptionService>(),
            roleSource,
            ImmutableArray.Create(permissionSystem));

        var infoSource = Substitute.For<IClientSecurityRuleInfoSource>();
        infoSource.GetInfos().Returns(infos);

        return new TestEnvironmentContext(new AvailableClientSecurityRuleSource(infoSource, securitySystem), roleSource, permissionSystem, permissionSource);
    }

    private static ClientSecurityRuleInfo Info(string name, DomainSecurityRule implementation) => new(name, implementation);

    private sealed class TestEnvironmentContext(
        AvailableClientSecurityRuleSource source,
        IAvailableSecurityRoleSource roleSource,
        IPermissionSystem permissionSystem,
        IPermissionSource permissionSource)
    {
        public async Task<string[]> GetAvailableNamesAsync() =>
            await source.GetAvailableSecurityRules().Select(rule => rule.Name).ToArrayAsync();

        public void SetPermissionSourceResult(bool result) =>
            permissionSource.HasAccessAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(result));

        public void AssertRolesLoaded(int expectedCalls) =>
            roleSource.Received(expectedCalls).GetAvailableSecurityRoles(Arg.Any<bool>());

        public void AssertPermissionSystemNotUsed() => this.AssertPermissionSystemUsed(0, 0);

        public void AssertPermissionSystemUsed(int permissionSourcesRequested, int accessChecks)
        {
            permissionSystem.Received(permissionSourcesRequested).GetPermissionSources(Arg.Any<DomainSecurityRule.RoleBaseSecurityRule>());
            permissionSource.Received(accessChecks).HasAccessAsync(Arg.Any<CancellationToken>());
        }
    }
}
