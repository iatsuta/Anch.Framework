using Anch.Core;
using Anch.DependencyInjection;
using Anch.Testing.Database;
using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.DependencyInjection;
using Anch.Testing.Database.Sqlite;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public abstract class TestEnvironment : DatabaseTestEnvironment
{
    protected abstract IServiceCollection AddServices(IServiceCollection services);

    protected override void InitDatabase(IDatabaseTestingSetup dts) =>

        dts.SetProvider<SqliteDatabaseTestingProvider>()
            .SetEmptySchemaInitializer<IEmptySchemaInitializer>(register: false)
            .SetTestDataInitializer<ITestDataInitializer>(register: false);

    protected override DatabaseInitMode DatabaseInitMode { get; } = DatabaseInitModeHelper.DatabaseInitMode;

    protected override TestConnectionString MainConnectionString { get; } = new("Data Source=test.db;Pooling=False");

    protected override IServiceProvider BuildServiceProvider(IServiceCollection services, TestConnectionString actualConnectionString) =>

        services

            .AddSingleton<IGenericQueryableSetupConfigurator, GenericQueryableSetupConfigurator>()

            .Pipe(this.AddServices)

            .AddSingleton<IMainConnectionStringSource>(new MainConnectionStringSource(actualConnectionString.Value))
            .AddSingleton<ITestDataInitializer, TestDataInitializer>()

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
}