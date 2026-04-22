using GenericQueryable.EntityFramework;
using GenericQueryable.IntegrationTests.Domain;

using Microsoft.EntityFrameworkCore;

namespace GenericQueryable.IntegrationTests.Environment;

public class TestDbContext(
    DbContextOptions<TestDbContext> options,
    IMainConnectionStringSource mainConnectionStringSource,
    IGenericQueryableSetupConfigurator genericQueryableSetupConfigurator) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder
            .UseSqlite(mainConnectionStringSource.ConnectionString)
            .UseGenericQueryable(genericQueryableSetupConfigurator.Configure);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestObject>();

        modelBuilder.Entity<FetchObject>();

        modelBuilder.Entity<DeepFetchObject>();

        base.OnModelCreating(modelBuilder);
    }
}