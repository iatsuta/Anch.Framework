using HierarchicalExpand.IntegrationTests.Environment.UndirectView;

using Microsoft.Extensions.DependencyInjection;

using NHibernate.Tool.hbm2ddl;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class DbSchemaInitializer(
    NHibernate.Cfg.Configuration nhibConfiguration,
    IServiceProvider serviceProvider,
    IEnumerable<IViewCreationScriptProvider> viewCreationScriptProviders) : IDbSchemaInitializer
{
    private readonly string dbName = "test.db";

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