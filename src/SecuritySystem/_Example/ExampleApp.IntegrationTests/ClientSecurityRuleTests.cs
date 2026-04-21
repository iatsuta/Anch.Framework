using CommonFramework.Testing;
using ExampleApp.Domain;

using SecuritySystem;
using SecuritySystem.AvailableSecurity;

namespace ExampleApp.IntegrationTests;

public abstract class ClientSecurityRuleTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [CommonFact]
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