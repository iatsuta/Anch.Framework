using HierarchicalExpand.IntegrationTests.Environment.UndirectView;

using Microsoft.EntityFrameworkCore;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class DbSchemaInitializer(
    AppDbContext dbContext,
    IEnumerable<IViewCreationScriptProvider> viewCreationScriptProviders) : IDbSchemaInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var createViewScript in viewCreationScriptProviders.SelectMany(v => v.GetScripts()))
        {
            await dbContext.Database.ExecuteSqlRawAsync(createViewScript, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}