using Anch.GenericQueryable.IntegrationTests.Domain;
using Anch.GenericQueryable.IntegrationTests.Environment;
using Anch.GenericRepository;
using Anch.Testing.Xunit;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.IntegrationTests;

public abstract class MainTests(IServiceProvider rootServiceProvider)
{
    private readonly Guid testObjId = rootServiceProvider.GetRequiredService<ITestDataInitializer>().TestObjId;

    [AnchFact]
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

    [AnchFact]
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

    [AnchFact]
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

    [AnchFact]
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

    [AnchFact]
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

    [AnchFact]
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
}