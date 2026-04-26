using Anch.SecuritySystem;
using Anch.SecuritySystem.AvailableSecurity;
using Anch.SecuritySystem.Testing;
using ExampleApp.Domain;

namespace ExampleApp.IntegrationTests;

public abstract class ClientSecurityRuleTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [AnchFact]
    public async Task GetAvailableSecurityRules_ReturnsExpectedClientSecurityRules(CancellationToken ct)
    {
        // Arrange
        var expectedResult = new DomainSecurityRule.ClientSecurityRule[]
        {
            new(nameof(BusinessUnit) + SecurityRule.View),
            new(nameof(TestObject) + SecurityRule.View)
        };

        // Act
        var result = await this.GetEvaluator<IAvailableClientSecurityRuleSource>().EvaluateAsync(TestingScopeMode.Read,
            async availableClientSecurityRuleSource =>
                await availableClientSecurityRuleSource.GetAvailableSecurityRules().ToArrayAsync(ct));

        // Assert
        Assert.Equivalent(expectedResult, result.OrderBy(v => v.Name));
    }
}