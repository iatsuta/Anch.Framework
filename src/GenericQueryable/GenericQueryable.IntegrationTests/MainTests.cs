using CommonFramework.GenericRepository;
using CommonFramework.Testing;

using GenericQueryable.IntegrationTests.Domain;
using GenericQueryable.IntegrationTests.Environment;

using Microsoft.Extensions.DependencyInjection;

namespace GenericQueryable.IntegrationTests;

public abstract class MainTests(IServiceProvider rootServiceProvider) : IAsyncLifetime
{
    private readonly Guid testObjId = Guid.NewGuid();

    protected virtual async ValueTask InitializeAsync(CancellationToken ct)
    {
        {
            await using var scope = rootServiceProvider.CreateAsyncScope();

            await scope.ServiceProvider.GetRequiredService<IDbSchemaInitializer>().Initialize(ct);
        }

        {
            await using var scope = rootServiceProvider.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var genericRepository = serviceProvider.GetRequiredService<IGenericRepository>();

            var fetchObj = new FetchObject();

            await genericRepository.SaveAsync(fetchObj, ct);
            await genericRepository.SaveAsync(new TestObject { Id = this.testObjId, FetchObject = fetchObj }, ct);
        }
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToListAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var serviceProvider = scope.ServiceProvider;

        var queryableSource = serviceProvider.GetRequiredService<IQueryableSource>();

        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result0 = await testSet
            .WithFetch(AppFetchRule.TestFetchRule)
            .GenericToArrayAsync(ct);

        var result1 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToListAsync(ct);

        var result2 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToHashSetAsync(ct);

        var result3 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToDictionaryAsync(v => v.Id, ct);

        var result4 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToDictionaryAsync(v => v.Id, v => v, ct);

        var result5 = await testSet
            //.WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericAsAsyncEnumerable()
            .Take(100)
            .ToArrayAsync(ct);

        //Assert
        result0.Should().ContainSingle(testObj => testObj.Id == this.testObjId);
    }


    ValueTask IAsyncLifetime.InitializeAsync() => this.InitializeAsync(TestContext.Current.CancellationToken);

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
}