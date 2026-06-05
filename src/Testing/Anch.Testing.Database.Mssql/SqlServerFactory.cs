using Anch.Testing.Database.ConnectionStringManagement;

using MartinCostello.SqlLocalDb;

using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Anch.Testing.Database.Mssql;

public class SqlServerFactory(
    IActualTestConnectionStringSource actualTestConnectionStringSource,
    TestDatabaseSettings settings) : ISqlServerFactory, IDisposable
{
    private readonly string? localDbInstanceName = TryGetLocalDbInstanceName(actualTestConnectionStringSource.ActualConnectionString, settings);

    public Server Create() =>
        new(
            new ServerConnection(
                new SqlConnection(
                    new SqlConnectionStringBuilder(actualTestConnectionStringSource.ActualConnectionString.Value) { InitialCatalog = "" }.ConnectionString)));

    private static string? TryGetLocalDbInstanceName(TestConnectionString connectionString, TestDatabaseSettings settings)
    {
        if (connectionString.TryGetLocalDbInstanceName() is { } localDbInstanceName)
        {
            using var localDbApi = new SqlLocalDbApi();

            if (settings.InitMode == DatabaseInitMode.RebuildSnapshot)
            {
                if (localDbApi.InstanceExists(localDbInstanceName))
                {
                    localDbApi.DeleteInstance(localDbInstanceName);
                }
            }

            localDbApi.CreateInstance(localDbInstanceName);

            return localDbInstanceName;
        }
        else
        {
            return null;
        }
    }

    public void Dispose()
    {
        if (this.localDbInstanceName != null && settings.InitMode == DatabaseInitMode.RebuildSnapshot)
        {
            using var localDbApi = new SqlLocalDbApi();

            if (localDbApi.InstanceExists(this.localDbInstanceName))
            {
                localDbApi.StopInstance(this.localDbInstanceName);
                localDbApi.DeleteInstance(this.localDbInstanceName, true);
            }
        }
    }
}
