using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NHibernate.Tool.hbm2ddl;

using System.Data.Common;

using ExampleApp.Infrastructure.DependencyInjection.UndirectView;

namespace ExampleApp.Infrastructure.Services;

public class DbSchemaInitializer(
    IConfiguration configuration,
    NHibernate.Cfg.Configuration nhibConfiguration,
    IServiceProvider serviceProvider,
    IEnumerable<IViewCreationScriptProvider> viewCreationScriptProviders) : IDbSchemaInitializer
{
    private readonly string dbName =
        new DbConnectionStringBuilder
            { ConnectionString = configuration.GetConnectionString("DefaultConnection") }["Data Source"].ToString()!;

    public async Task Initialize(CancellationToken cancellationToken)
    {
        if (File.Exists(this.dbName))
        {
            File.Delete(this.dbName);
        }

        var schemaExport = new SchemaExport(nhibConfiguration);

        await schemaExport.CreateAsync(false, true, cancellationToken);

        var session = serviceProvider.GetRequiredService<AutoCommitSession>();

        foreach (var createViewScript in viewCreationScriptProviders.SelectMany(v => v.GetScripts()))
        {
            await session.NativeSession.CreateSQLQuery(createViewScript).ExecuteUpdateAsync(cancellationToken);
        }
    }
}