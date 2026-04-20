namespace GenericQueryable.IntegrationTests.Environment;

public class DbSchemeInitializer(TestDbContext dbContext) : IDbSchemeInitializer
{
    public async Task Initialize(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}