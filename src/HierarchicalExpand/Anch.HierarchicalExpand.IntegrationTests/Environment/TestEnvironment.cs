using Anch.Core;
using Anch.DependencyInjection;
using Anch.HierarchicalExpand.DependencyInjection;
using Anch.HierarchicalExpand.IntegrationTests.Domain;
using Anch.HierarchicalExpand.IntegrationTests.Environment.UndirectView;
using Anch.Testing.Database;
using Anch.Testing.Database.DependencyInjection;
using Anch.Testing.Database.Sqlite;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

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

            .AddDatabaseTesting(dts => dts
                .SetProvider<SqliteDatabaseTestingProvider>()
                .SetEmptySchemaInitializer<IEmptySchemaInitializer>(register: false)
                .SetTestDataInitializer<TestDataInitializer>()
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