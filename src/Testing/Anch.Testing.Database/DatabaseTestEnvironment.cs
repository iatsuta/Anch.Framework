using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.DependencyInjection;
using Anch.Testing.Database.Hooks;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database;

public abstract class DatabaseTestEnvironment : ITestEnvironment
{
    protected abstract TestConnectionString RawConnectionString { get; }

    protected virtual DatabaseInitMode DatabaseInitMode { get; } = DatabaseInitMode.RebuildSnapshot;

    protected virtual bool RemoveDatabaseOnFailure { get; } = true;

    public IServiceProvider BuildServiceProvider(IServiceCollection baseServices, ServiceProviderBuildContext buildContext)
    {
        var actualConnectionString = this.GetActualConnectionString(buildContext);

        var services = baseServices
            .AddSingleton<IActualTestConnectionStringSource>(new ActualTestConnectionStringSource(actualConnectionString))
            .AddEnvironmentHook<PrepareDatabaseEnvironmentHook>(EnvironmentHookType.Before)
            .AddEnvironmentHook<CleanDatabaseEnvironmentHook>(EnvironmentHookType.After);

        var fullServices = buildContext.Index.IsMain ? this.InitializeMainServices(services) : services;

        return this.BuildServiceProvider(fullServices, actualConnectionString);
    }

    protected virtual TestConnectionString GetActualConnectionString(ServiceProviderBuildContext buildContext) =>

        buildContext switch
        {
            PooledServiceProviderBuildContext pooledContext => pooledContext.MainServiceProvider.GetRequiredService<IActualTestConnectionStringFactory>()
                .Create(buildContext.Index),
            { Index.IsMain: true } => this.RawConnectionString,
            _ => throw new InvalidOperationException("Unsupported build context.")
        };

    protected TestDatabaseSettings DatabaseSettings => field ??= new TestDatabaseSettings
    {
        RawConnectionString = this.RawConnectionString,
        InitMode = this.DatabaseInitMode,
        RemoveDatabaseOnFailure = this.RemoveDatabaseOnFailure
    };

    protected virtual IServiceCollection InitializeMainServices(IServiceCollection services) =>
        services.AddDatabaseTesting(dts => dts.SetSettings(this.DatabaseSettings).Pipe(this.InitDatabase));

    protected abstract void InitDatabase(IDatabaseTestingSetup dts);

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services, TestConnectionString actualConnectionString);
}