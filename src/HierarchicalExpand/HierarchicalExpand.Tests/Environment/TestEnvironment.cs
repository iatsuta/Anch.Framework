using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;
using HierarchicalExpand.DependencyInjection;
using HierarchicalExpand.Tests.Domain;
using HierarchicalExpand.Tests.Environment;

using Microsoft.Extensions.DependencyInjection;

[assembly: CommonTestFramework<TestEnvironment>]

namespace HierarchicalExpand.Tests.Environment;

public class TestEnvironment : ITestEnvironment
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        services
            .AddSingleton<TestQueryableSource>()
            .AddSingletonFrom<IQueryableSource, TestQueryableSource>()
            .AddSingleton(Substitute.For<IGenericRepository>())

            .AddHierarchicalExpand(scb => scb
                .AddHierarchicalInfo(
                    v => v.Parent,
                    new AncestorLinkInfo<DomainObject, DirectAncestorLink>(link => link.From, link => link.To),
                    new AncestorLinkInfo<DomainObject, UnDirectAncestorLink>(view => view.From, view => view.To)))

            .AddEnvironmentHook(EnvironmentHookType.After, sp => sp.GetRequiredService<TestQueryableSource>().Reset())

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
}