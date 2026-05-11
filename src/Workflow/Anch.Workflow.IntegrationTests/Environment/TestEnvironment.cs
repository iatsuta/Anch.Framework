using Anch.Core;
using Anch.DependencyInjection;
using Anch.Testing.Database;
using Anch.Testing.Database.DependencyInjection;
using Anch.Testing.Database.Sqlite;
using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Persistence.Inline;
using Anch.Workflow.Tests.TaskWorkflow;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.IntegrationTests.Environment;

public abstract class TestEnvironment : ITestEnvironment
{
    protected abstract IServiceCollection AddServices(IServiceCollection services);

    public IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        services

            .AddWorkflow(ws => ws
                .Add<TaskWorkflow>()
                .SetDatabaseProvider<InlineWorkflowDatabaseProvider>())

            .Pipe(this.AddServices)

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