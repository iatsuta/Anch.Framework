using Anch.HierarchicalExpand.IntegrationTests.Environment.UndirectView;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Tool.hbm2ddl;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class NHibEmptySchemaInitializer(
    NHibernate.Cfg.Configuration nhibConfiguration,
    IServiceProvider rootServiceProvider,
    IEnumerable<IViewCreationScriptProvider> viewCreationScriptProviders) : IEmptySchemaInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        var schemaExport = new SchemaExport(nhibConfiguration);

        await schemaExport.CreateAsync(false, true, cancellationToken);

        await using var scope = rootServiceProvider.CreateAsyncScope();

        var session = scope.ServiceProvider.GetRequiredService<NHibAutoCommitSession>();

        foreach (var createViewScript in viewCreationScriptProviders.SelectMany(v => v.GetScripts()))
        {
            await session.NativeSession.CreateSQLQuery(createViewScript).ExecuteUpdateAsync(cancellationToken);
        }
    }
}