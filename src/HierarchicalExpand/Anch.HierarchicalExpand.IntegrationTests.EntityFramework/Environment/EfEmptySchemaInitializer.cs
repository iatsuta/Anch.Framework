using Anch.HierarchicalExpand.IntegrationTests.Environment.UndirectView;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class EfEmptySchemaInitializer(
    IServiceProvider rootServiceProvider,
    IEnumerable<IViewCreationScriptProvider> viewCreationScriptProviders) : IEmptySchemaInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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