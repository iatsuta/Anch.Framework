using GenericQueryable.DependencyInjection;
using GenericQueryable.Fetching;
using GenericQueryable.IntegrationTests.Domain;

namespace GenericQueryable.IntegrationTests.Environment;

public class GenericQueryableSetupConfigurator : IGenericQueryableSetupConfigurator
{
    public void Configure(IGenericQueryableSetup builder) =>
        builder
            .AddFetchRuleExpander<AppFetchRuleExpander>()
            .AddFetchRule(AppFetchRule.TestFetchRule, FetchRule<TestObject>.Create(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject));
}