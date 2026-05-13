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
        var actualConnectionString = this.GetActualConnectionString(serviceProviderIndex);

        var services = baseServices
            .AddSingleton<IActualTestConnectionStringSource>(new ActualTestConnectionStringSource(actualConnectionString))
            .AddEnvironmentHook<PrepareDatabaseEnvironmentHook>(EnvironmentHookType.Before)
            .AddEnvironmentHook<CleanDatabaseEnvironmentHook>(EnvironmentHookType.After);

        if (serviceProviderIndex.IsMain)
        {
            return this.mainServiceProvider ??= this.BuildServiceProvider(this.InitMainServices(services), actualConnectionString);
        }
        else
        {
            return this.BuildServiceProvider(services, actualConnectionString);
        }
    }

    private TestConnectionString GetActualConnectionString(ServiceProviderIndex serviceProviderIndex)
    {
        if (serviceProviderIndex.IsMain)
        {
            return this.MainConnectionString;
        }
        else
        {
            return (this.mainServiceProvider ?? throw new InvalidOperationException("Main service provider is not initialized."))
                .GetRequiredService<IActualTestConnectionStringFactory>().Create(serviceProviderIndex);
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