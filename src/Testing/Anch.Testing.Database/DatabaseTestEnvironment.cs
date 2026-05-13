using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.DependencyInjection;
using Anch.Testing.Database.Hooks;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database;

public abstract class DatabaseTestEnvironment : ITestEnvironment
{
    protected abstract TestConnectionString MainConnectionString { get; }

    protected virtual DatabaseInitMode DatabaseInitMode { get; } = DatabaseInitMode.RebuildSnapshot;

    protected virtual bool RemoveDatabaseOnFailure { get; } = true;

    public IServiceProvider BuildServiceProvider(IServiceCollection baseServices, ServiceProviderBuildContext buildContext)
    {
        var actualConnectionString = this.GetActualConnectionString(buildContext);

        var services = baseServices
            .AddSingleton<IActualTestConnectionStringSource>(new ActualTestConnectionStringSource(actualConnectionString))
            .AddEnvironmentHook<PrepareDatabaseEnvironmentHook>(EnvironmentHookType.Before)
            .AddEnvironmentHook<CleanDatabaseEnvironmentHook>(EnvironmentHookType.After);

        if (buildContext.Index.IsMain)
        {
            return this.BuildServiceProvider(this.InitMainServices(services), actualConnectionString);
        }
        else
        {
            return this.BuildServiceProvider(services, actualConnectionString);
        }
    }

    private TestConnectionString GetActualConnectionString(ServiceProviderBuildContext buildContext)
    {
        switch (buildContext)
        {
            case PooledServiceProviderBuildContext pooledContext:
                return pooledContext.MainServiceProvider.GetRequiredService<IActualTestConnectionStringFactory>().Create(buildContext.Index);

            case { Index.IsMain: true }:
                return this.MainConnectionString;

            default:
                throw new InvalidOperationException("Unsupported build context.");
        }
    }

    private IServiceCollection InitMainServices(IServiceCollection services) =>
        services.AddDatabaseTesting(dts => dts.SetSettings(new TestDatabaseSettings
            {
                MainConnectionString = this.MainConnectionString,
                InitMode = this.DatabaseInitMode,
                RemoveDatabaseOnFailure = this.RemoveDatabaseOnFailure
            })
            .Pipe(this.InitDatabase));

    protected abstract void InitDatabase(IDatabaseTestingSetup dts);

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services, TestConnectionString actualConnectionString);
}