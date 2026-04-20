using CommonFramework.DependencyInjection;
using CommonFramework.Testing;

using GenericQueryable.DependencyInjection;
using GenericQueryable.Fetching;
using GenericQueryable.IntegrationTests.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.IntegrationTests.Environment;

public class TestServiceProviderBuilderBase : ITestServiceProviderBuilder
{
    public virtual IServiceProvider Build(IServiceCollection services) =>

        services
            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

    protected static void SetupGenericQueryable(IGenericQueryableSetup builder) =>

        builder
            .AddFetchRuleExpander<AppFetchRuleExpander>()
            .AddFetchRule(AppFetchRule.TestFetchRule, FetchRule<TestObject>.Create(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject));
}