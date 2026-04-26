using CommonFramework;
using CommonFramework.DependencyInjection;
using CommonFramework.Testing.Database;
using CommonFramework.Testing.Database.DependencyInjection;
using CommonFramework.Testing.Database.Sqlite;

using HierarchicalExpand.DependencyInjection;
using HierarchicalExpand.IntegrationTests.Domain;
using HierarchicalExpand.IntegrationTests.Environment.UndirectView;
using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.IntegrationTests.Environment;

public abstract class TestEnvironment : ITestEnvironment
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return services

            .Pipe(this.InitializeServices)

            .AddSingleton<ScopeEvaluator>()

            .AddSingleton<IUndirectedAncestorViewScriptGenerator, UndirectedAncestorViewScriptGenerator>()
            .AddSingleton<IViewCreationScriptProvider, UndirectedAncestorViewScriptProvider>()

            .AddHierarchicalExpand(scb => scb
                .AddHierarchicalInfo(
                    v => v.Parent,
                    new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                    new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(view => view.Source, view => view.Target),
                    v => v.DeepLevel)

                .AddHierarchicalInfo(
                    v => v.Parent,
                    new AncestorLinkInfo<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                    new AncestorLinkInfo<TestHierarchicalObject, TestHierarchicalObjectUndirectAncestorLink>(view => view.Source, view => view.Target),
                    v => v.DeepLevel))

            .AddSingleton<ISharedTestDataInitializer, SharedTestDataInitializer>()

            .AddDatabaseTesting(dts => dts
                .SetProvider<SqliteDatabaseTestingProvider>()
                .SetEmptySchemaInitializer<IEmptySchemaInitializer>()
                .SetSharedTestDataInitializer<ISharedTestDataInitializer>()
                .SetSettings(new TestDatabaseSettings
                {
                    InitMode = DatabaseInitModeHelper.DatabaseInitMode,
                    DefaultConnectionString = new("Data Source=test.db;Pooling=False")
                })
                .RebindActualConnection<IMainConnectionStringSource>(connectionString => new MainConnectionStringSource(connectionString.Value)))

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    protected abstract IServiceCollection InitializeServices(IServiceCollection services);
}