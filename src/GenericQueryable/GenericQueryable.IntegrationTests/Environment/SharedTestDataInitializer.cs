using CommonFramework.GenericRepository;

using GenericQueryable.IntegrationTests.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.IntegrationTests.Environment;

public class SharedTestDataInitializer(IServiceProvider rootServiceProvider) : ISharedTestDataInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var genericRepository = serviceProvider.GetRequiredService<IGenericRepository>();

        var fetchObj = new FetchObject();

        await genericRepository.SaveAsync(fetchObj, cancellationToken);
        await genericRepository.SaveAsync(new TestObject { Id = this.TestObjId, FetchObject = fetchObj }, cancellationToken);
    }

    public Guid TestObjId { get; } = Guid.NewGuid();
}