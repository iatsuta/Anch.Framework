using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class EfEmptySchemaInitializer(IServiceProvider rootServiceProvider) : IEmptySchemaInitializer
{
    public async Task Initialize(CancellationToken ct)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        await dbContext.Database.EnsureDeletedAsync(ct);
        await dbContext.Database.EnsureCreatedAsync(ct);
    }
}