using Anch.GenericQueryable.Fetching;
using Anch.GenericQueryable.IntegrationTests.Domain;

namespace Anch.GenericQueryable.IntegrationTests;

public static class AppFetchRule
{
    public static FetchRuleHeader<TestObject> TestFetchRule { get; } = FetchRuleHeader<TestObject>.Create(nameof(TestFetchRule));
}