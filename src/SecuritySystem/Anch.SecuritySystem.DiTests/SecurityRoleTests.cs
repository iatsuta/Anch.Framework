using Anch.HierarchicalExpand;
using Anch.SecuritySystem.DiTests.Rules;
using Anch.SecuritySystem.Expanders;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DiTests;

public class SecurityRoleTests(IServiceProvider rootServiceProvider)
{
    [Fact]
    public void AdministratorRole_ShouldNotContains_SystemIntegrationRole()
    {
        // Arrange
        var securityRoleSource = rootServiceProvider.GetRequiredService<ISecurityRoleSource>();

        // Act
        var adminRole = securityRoleSource.GetSecurityRole(SecurityRole.Administrator);

        // Assert
        Assert.DoesNotContain(SecurityRole.SystemIntegration, adminRole.Information.Children);
    }

    [Fact]
    public void SecurityRoleExpander_ExpandDeepChild_AllRolesExpanded()
    {
        // Arrange
        var expander = rootServiceProvider.GetRequiredService<ISecurityRoleGroupExpander>();

        var expectedResult = new DomainSecurityRule.ExpandedRoleGroupSecurityRule(
        [
            new DomainSecurityRule.ExpandedRolesSecurityRule([ExampleSecurityRole.TestRole, ExampleSecurityRole.TestRole2, ExampleSecurityRole.TestRole3])
                { CustomRestriction = SecurityPathRestriction.Default },

            new DomainSecurityRule.ExpandedRolesSecurityRule([SecurityRole.Administrator]) { CustomRestriction = SecurityPathRestriction.Ignored },
        ]);

        // Act
        var expandResult = expander.Expand(ExampleSecurityRole.TestRole3);

        // The order of roles inside a group is not part of the contract: sort them by name.
        var sortedExpandResult = new DomainSecurityRule.ExpandedRoleGroupSecurityRule(
            expandResult.Children.Select(child =>
                new DomainSecurityRule.ExpandedRolesSecurityRule(child.SecurityRoles.OrderBy(role => role.Name))
                {
                    CustomRestriction = child.CustomRestriction,
                    CustomExpandType = child.CustomExpandType
                }));

        // Assert
        Assert.Equal(expectedResult, sortedExpandResult);
    }

    [Fact]
    public void SecurityRoleExpander_ExpandWithDefaultExpandType_RoleResolved()
    {
        // Arrange
        var expander = rootServiceProvider.GetRequiredService<ISecurityOperationExpander>();

        // Act
        var expandResult = expander.Expand(new DomainSecurityRule.OperationSecurityRule(ExampleSecurityOperation.EmployeeView));

        // Assert
        Assert.Equal(new[] { ExampleSecurityRole.TestRole }, expandResult.SecurityRoles);
    }

    [Fact]
    public void SecurityRoleExpander_ExpandWithCustomExpandType_SecurityRuleCorrected()
    {
        // Arrange
        var expander = rootServiceProvider.GetRequiredService<ISecurityOperationExpander>();

        // Act
        var expandResult = expander.Expand(ExampleSecurityOperation.EmployeeView.ToSecurityRule(HierarchicalExpandType.None));

        // Assert
        Assert.Equal(new[] { ExampleSecurityRole.TestRole }.ToSecurityRule(HierarchicalExpandType.None), expandResult);
    }

    [Fact]
    public void SecurityRoleExpander_FullExpandWithCustomExpandType_SecurityRuleCorrected()
    {
        // Arrange
        var expander = rootServiceProvider.GetRequiredService<ISecurityRuleExpander>();

        var customExpandType = HierarchicalExpandType.All;

        var expectedResult = new DomainSecurityRule.ExpandedRoleGroupSecurityRule(
        [
            new DomainSecurityRule.ExpandedRolesSecurityRule([ExampleSecurityRole.TestRole])
                { CustomRestriction = SecurityPathRestriction.Default, CustomExpandType = customExpandType },

            new DomainSecurityRule.ExpandedRolesSecurityRule([SecurityRole.Administrator])
                { CustomRestriction = SecurityPathRestriction.Ignored }
        ]);

        // Act
        var expandResult = expander.FullRoleExpand(ExampleSecurityOperation.EmployeeView.ToSecurityRule(customExpandType));

        // Assert
        Assert.Equal(expectedResult, expandResult);
    }

    [Fact]
    public void SecurityRoleExpander_FullExpandWithCustomExpandTypeFromOperations_SecurityRuleCorrected()
    {
        // Arrange
        var expander = rootServiceProvider.GetRequiredService<ISecurityRuleExpander>();

        var customExpandType = HierarchicalExpandType.None;

        // Act
        var expandResult = expander.FullRoleExpand(ExampleSecurityOperation.BusinessUnitView);

        var expectedResult = new DomainSecurityRule.ExpandedRoleGroupSecurityRule(
        [
            new DomainSecurityRule.ExpandedRolesSecurityRule([ExampleSecurityRole.TestRole4])
                { CustomRestriction = SecurityPathRestriction.Default, CustomExpandType = customExpandType },

            new DomainSecurityRule.ExpandedRolesSecurityRule([SecurityRole.Administrator])
                { CustomRestriction = SecurityPathRestriction.Ignored }
        ]);

        // Assert
        Assert.Equal(expectedResult, expandResult);
    }
}