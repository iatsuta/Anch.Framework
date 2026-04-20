using CommonFramework.GenericRepository;

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

            await scope.ServiceProvider.GetRequiredService<IDbSchemeInitializer>().Initialize(ct);
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

    ValueTask IAsyncLifetime.InitializeAsync() => this.InitializeAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task DefaultGenericQueryable_InvokeToListAsync_MethodInvoked()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var scope = rootServiceProvider.CreateAsyncScope();

        var serviceProvider = scope.ServiceProvider;

        var queryableSource = serviceProvider.GetRequiredService<IQueryableSource>();

        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result0 = await testSet
            .WithFetch(AppFetchRule.TestFetchRule)
            .GenericToArrayAsync(cancellationToken);

        var result1 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToListAsync(cancellationToken);

        var result2 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToHashSetAsync(cancellationToken);

        var result3 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToDictionaryAsync(v => v.Id, cancellationToken);

        var result4 = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToDictionaryAsync(v => v.Id, v => v, cancellationToken);

        var result5 = await testSet
            //.WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericAsAsyncEnumerable()
            .Take(100)
            .ToArrayAsync(cancellationToken);

        //Assert
        result0.Should().ContainSingle(testObj => testObj.Id == this.testObjId);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}