using Anch.DependencyInjection;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.DependencyInjection;
using Anch.HierarchicalExpand.Tests.Domain;
using Anch.HierarchicalExpand.Tests.Environment;
using Anch.Testing.Xunit;

using Microsoft.Extensions.DependencyInjection;
[assembly: AnchTestFramework<TestEnvironment>]

namespace Anch.HierarchicalExpand.Tests.Environment;

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
                    new AncestorLinkInfo<DomainObject, UnDirectAncestorLink>(view => view.From, view => view.To))).AddEnvironmentHook(EnvironmentHookType.After, sp => sp.GetRequiredService<TestQueryableSource>().Reset())

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
}