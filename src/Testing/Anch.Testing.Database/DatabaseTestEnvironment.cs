using Anch.Core;
using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.DependencyInjection;
using Anch.Testing.Database.Hooks;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database;

public abstract class DatabaseTestEnvironment : ITestEnvironment
{
    private IServiceProvider? mainServiceProvider;

    protected abstract TestConnectionString MainConnectionString { get; }

    protected virtual DatabaseInitMode DatabaseInitMode { get; } = DatabaseInitMode.RebuildSnapshot;

    protected virtual bool RemoveDatabaseOnFailure { get; } = true;

    public IServiceProvider BuildServiceProvider(IServiceCollection baseServices, ServiceProviderIndex serviceProviderIndex)
    {
        var services = baseServices
            .AddEnvironmentHook<PrepareDatabaseEnvironmentHook>(EnvironmentHookType.Before)
            .AddEnvironmentHook<CleanDatabaseEnvironmentHook>(EnvironmentHookType.After);

        if (serviceProviderIndex.IsMain)
        {
            return this.mainServiceProvider ??= this.BuildServiceProvider(this.InitMainServices(services), this.MainConnectionString);
        }
        else
        {
            var actualConnectionString = (this.mainServiceProvider ?? throw new InvalidOperationException("Main service provider is not initialized."))
                .GetRequiredService<IActualConnectionStringResolver>().GetActualConnectionString(serviceProviderIndex);

            return this.BuildServiceProvider(services, actualConnectionString);
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