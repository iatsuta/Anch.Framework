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
    public async Task DefaultGenericQueryable_InvokeToArrayAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result = await testSet
            .WithFetch(AppFetchRule.TestFetchRule)
            .GenericToArrayAsync(ct);

        // Assert
        Assert.Single(result, testObj => testObj.Id == this.testObjId);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToListAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToListAsync(ct);

        // Assert
        Assert.Single(result, testObj => testObj.Id == this.testObjId);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToHashSetAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToHashSetAsync(ct);

        // Assert
        Assert.Single(result, testObj => testObj.Id == this.testObjId);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToDictionaryAsync(v => v.Id, ct);

        // Assert
        Assert.Single(result);
        Assert.True(result.ContainsKey(this.testObjId));
        Assert.Equal(this.testObjId, result[this.testObjId].Id);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryWithElementSelectorAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result = await testSet
            .WithFetch(r => r.Fetch(v => v.DeepFetchObjects).ThenFetch(v => v.FetchObject))
            .GenericToDictionaryAsync(v => v.Id, v => v, ct);

        // Assert
        Assert.Single(result);
        Assert.True(result.ContainsKey(this.testObjId));
        Assert.Equal(this.testObjId, result[this.testObjId].Id);
    }

    [CommonFact]
    public async Task GenericAsAsyncEnumerable_Should_Execute(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var testSet = queryableSource.GetQueryable<TestObject>();

        // Act
        var result = await testSet
            .GenericAsAsyncEnumerable()
            .Take(100)
            .ToArrayAsync(ct);

        // Assert
        Assert.Single(result, testObj => testObj.Id == this.testObjId);
    }

    ValueTask IAsyncLifetime.InitializeAsync() => this.InitializeAsync(TestContext.Current.CancellationToken);

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
}