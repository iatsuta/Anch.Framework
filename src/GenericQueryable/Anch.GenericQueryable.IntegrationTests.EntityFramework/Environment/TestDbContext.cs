using Anch.GenericQueryable.DependencyInjection;
using Anch.GenericQueryable.EntityFramework;
using Anch.GenericQueryable.IntegrationTests.Domain;
using Anch.GenericQueryable.IntegrationTests.Visitors;

using Microsoft.EntityFrameworkCore;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class TestDbContext(
    DbContextOptions<TestDbContext> options,
    IMainConnectionStringSource mainConnectionStringSource) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder
            .UseSqlite(mainConnectionStringSource.ConnectionString)
            .UseGenericQueryable(s => s.SetSetupType<EfGenericQueryableExtensionSetup>());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestObject>();

        modelBuilder.Entity<FetchObject>();

        modelBuilder.Entity<DeepFetchObject>();

        base.OnModelCreating(modelBuilder);
    }

    private class EfGenericQueryableExtensionSetup : GenericQueryableSetupConfigurator, IEfGenericQueryableExtensionInnerSetup
    {
        public override void Initialize(IGenericQueryableSetup setup)
        {
            base.Initialize(setup);

            setup.SetVisitor<LinkIdVisitor>();
        }
    }
}