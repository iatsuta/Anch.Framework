using Anch.GenericQueryable.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Anch.Workflow.IntegrationTests.Environment;

public class TestDbContext(
    DbContextOptions<TestDbContext> options,
    IMainConnectionStringSource mainConnectionStringSource) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder
            .UseSqlite(mainConnectionStringSource.ConnectionString)
            .UseGenericQueryable();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestObject>();

        base.OnModelCreating(modelBuilder);
    }
}