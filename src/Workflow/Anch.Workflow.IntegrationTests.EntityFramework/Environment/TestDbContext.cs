using Anch.GenericQueryable.EntityFramework;
using Anch.Workflow.Tests.TaskWorkflow;

using Microsoft.EntityFrameworkCore;

namespace Anch.Workflow.IntegrationTests.Environment;

public class TestDbContext(
    DbContextOptions<TestDbContext> options,
    IMainConnectionStringSource mainConnectionStringSource) : DbContext(options)
{
    private const string DefaultIdPostfix = "Id";

    private const string DefaultSchema = "app";

    private const int DefaultMaxLength = 255;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder
            .UseSqlite(mainConnectionStringSource.ConnectionString)
            .UseGenericQueryable();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        {
            var entity = modelBuilder.Entity<TaskWorkflowObject>().ToTable(nameof(TaskWorkflowObject), DefaultSchema);
            entity.HasKey(v => v.Id);

            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PostProcessWork).IsRequired();
        }

        base.OnModelCreating(modelBuilder);
    }
}