using Anch.GenericQueryable.DependencyInjection;
using Anch.GenericQueryable.Fetching;
using Anch.GenericQueryable.IntegrationTests.Domain;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class GenericQueryableSetupConfigurator : IGenericQueryableSetupConfigurator
{
    public void Configure(IGenericQueryableSetup builder) =>
        builder
            .AddFetchRuleExpander<AppFetchRuleExpander>()
            .AddFetchRule(AppFetchRule.TestFetchRule, FetchRule<TestObject>.Create(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject));
}