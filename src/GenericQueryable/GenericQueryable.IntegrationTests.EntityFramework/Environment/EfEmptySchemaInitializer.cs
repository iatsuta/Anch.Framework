using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.IntegrationTests.Environment;

public class EfEmptySchemaInitializer(IServiceProvider rootServiceProvider) : IEmptySchemaInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}