using Anch.HierarchicalExpand.IntegrationTests.Environment.UndirectView;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class EfEmptySchemaInitializer(
    IServiceProvider rootServiceProvider,
    IEnumerable<IViewCreationScriptProvider> viewCreationScriptProviders) : IEmptySchemaInitializer
{
    public async Task Initialize(CancellationToken ct)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureDeletedAsync(ct);
        await dbContext.Database.EnsureCreatedAsync(ct);

        await dbContext.SaveChangesAsync(ct);

        foreach (var createViewScript in viewCreationScriptProviders.SelectMany(v => v.GetScripts()))
        {
            await dbContext.Database.ExecuteSqlRawAsync(createViewScript, ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }
}