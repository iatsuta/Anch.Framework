using Anch.GenericQueryable.IntegrationTests.Domain;
using Anch.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class TestDataInitializer(IServiceProvider rootServiceProvider) : ITestDataInitializer
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

    public Guid TestObjId { get; } = new("{16129B46-C49C-40D0-A787-419501DCF223}");
}