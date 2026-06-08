using NHibernate.Tool.hbm2ddl;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class NHibEmptySchemaInitializer(global::NHibernate.Cfg.Configuration nhibConfiguration) : IEmptySchemaInitializer
{
    public async Task Initialize(CancellationToken ct)
    {
        var schemaExport = new SchemaExport(nhibConfiguration);

        await schemaExport.CreateAsync(false, true, ct);
    }
}