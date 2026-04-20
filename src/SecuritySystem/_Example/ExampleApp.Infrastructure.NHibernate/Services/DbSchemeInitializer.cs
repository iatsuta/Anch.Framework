using System.Data.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NHibernate.Tool.hbm2ddl;

namespace ExampleApp.Infrastructure.Services;

public class DbSchemeInitializer(
    IConfiguration configuration,
    NHibernate.Cfg.Configuration nhibConfiguration,
    IServiceProvider serviceProvider,
    IEnumerable<CreateViewSql> createViewSqlList) : IDbSchemeInitializer
{
    private readonly string dbName =
        new DbConnectionStringBuilder { ConnectionString = configuration.GetConnectionString("DefaultConnection") }["Data Source"].ToString()!;

    public async Task Initialize(CancellationToken cancellationToken)
    {
        if (File.Exists(this.dbName))
        {
            File.Delete(this.dbName);
        }

        var schemaExport = new SchemaExport(nhibConfiguration);

        await schemaExport.CreateAsync(false, true, cancellationToken);

        var session = serviceProvider.GetRequiredService<AutoCommitSession>();

        foreach (var createViewSql in createViewSqlList)
        {
            await session.NativeSession.CreateSQLQuery(createViewSql.Text).ExecuteUpdateAsync(cancellationToken);
        }
    }
}