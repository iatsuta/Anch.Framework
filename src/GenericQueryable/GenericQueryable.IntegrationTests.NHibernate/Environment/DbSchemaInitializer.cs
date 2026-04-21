using NHibernate.Tool.hbm2ddl;

namespace GenericQueryable.IntegrationTests.Environment;

public class DbSchemaInitializer(global::NHibernate.Cfg.Configuration nhibConfiguration) : IDbSchemaInitializer
{
    private readonly string dbName = "TestSystem.sqlite";

    public async Task Initialize(CancellationToken cancellationToken)
    {
        if (File.Exists(this.dbName))
        {
            File.Delete(this.dbName);
        }

        var schemaExport = new SchemaExport(nhibConfiguration);

        await schemaExport.CreateAsync(false, true, cancellationToken);
    }
}