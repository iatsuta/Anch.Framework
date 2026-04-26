[assembly: AnchTestFramework]

namespace Anch.GenericQueryable.UnitTests;

public class MainTests
{
    private sealed record DecimalContainer(decimal? Value);

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeSumAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new decimal?[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericSumAsync(ct);

        // Assert
        Assert.Equal(baseSource.Sum(), result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeSumWithSelectorAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { new DecimalContainer(1), new DecimalContainer(2), new DecimalContainer(3) };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericSumAsync(v => v.Value, ct);

        // Assert
        Assert.Equal(baseSource.Sum(v => v.Value), result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToArrayAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToArrayAsync(ct);

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToListAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToListAsync(ct);

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToHashSetAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToHashSetAsync(ct);

        // Assert
        Assert.Equivalent(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToHashSetWithComparerAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { "a", "b", "c" };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToHashSetAsync(StringComparer.OrdinalIgnoreCase, ct);

        // Assert
        Assert.Equivalent(baseSource, result);
        Assert.Same(StringComparer.OrdinalIgnoreCase, result.Comparer);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, ct);

        // Assert
        Assert.Equivalent(baseSource.ToDictionary(v => v), result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryWithComparerAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { "a", "b", "c" };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, StringComparer.OrdinalIgnoreCase, ct);

        // Assert
        Assert.Equivalent(baseSource.ToDictionary(v => v), result);
        Assert.Same(StringComparer.OrdinalIgnoreCase, result.Comparer);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryWithElementSelectorAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, v => v, ct);

        // Assert
        Assert.Equivalent(baseSource.ToDictionary(v => v), result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryWithElementSelectorAndComparerAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { "a", "b", "c" };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, v => v, StringComparer.OrdinalIgnoreCase, ct);

        // Assert
        Assert.Equivalent(baseSource.ToDictionary(v => v), result);
        Assert.Same(StringComparer.OrdinalIgnoreCase, result.Comparer);
    }

    [AnchFact]
    public void DefaultGenericQueryable_InvokeToList_MethodInvoked()
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = qSource.ToList();

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeSingleAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = 1;
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.GenericSingleAsync(ct);

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeSingleAsyncWithFilter_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = 2;
        var qSource = new[] { 1, baseSource, 3 }.AsQueryable();

        // Act
        var result = await qSource.GenericSingleAsync(v => v == baseSource, ct);

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeSingleOrDefaultAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = 1;
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.GenericSingleOrDefaultAsync(ct);

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeSingleOrDefaultAsyncWithFilter_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = 1;
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.GenericSingleOrDefaultAsync(_ => true, ct);

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeFirstAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericFirstAsync(ct);

        // Assert
        Assert.Equal(baseSource.First(), result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeFirstOrDefaultAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericFirstOrDefaultAsync(ct);

        // Assert
        Assert.Equal(baseSource.First(), result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeFirstOrDefaultAsyncWithFilter_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericFirstOrDefaultAsync(v => v > 1, ct);

        // Assert
        Assert.Equal(2, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeCountAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericCountAsync(ct);

        // Assert
        Assert.Equal(baseSource.Length, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeAllAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericAllAsync(v => v > 0, ct);

        // Assert
        Assert.True(result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeAnyAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericAnyAsync(ct);

        // Assert
        Assert.True(result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeAnyAsyncWithFilter_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericAnyAsync(v => v == 2, ct);

        // Assert
        Assert.True(result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeContainsAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericContainsAsync(2, ct);

        // Assert
        Assert.True(result);
    }

    [AnchFact]
    public async Task GenericAsAsyncEnumerable_Should_Execute(CancellationToken ct)
    {
        // Arrange
        var baseSource = 1;
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.GenericAsAsyncEnumerable().SingleAsync(ct);

        // Assert
        Assert.Equal(baseSource, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeExecuteWithExpression_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.Execute(() => qSource.GenericCountAsync(ct));

        // Assert
        Assert.Equal(baseSource.Length, result);
    }

    [AnchFact]
    public void DefaultGenericQueryable_InvokeExecuteWithExecutor_MethodInvoked()
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = qSource.Execute(_ => qSource.Count());

        // Assert
        Assert.Equal(baseSource.Length, result);
    }

    [AnchFact]
    public async Task DefaultGenericQueryable_InvokeFetch_FetchIgnored(CancellationToken ct)
    {
        // Arrange
        var baseSource = "abc";
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.WithFetch(nameof(string.Length))
            .GenericSingleOrDefaultAsync(_ => true, ct);

        // Assert
        Assert.Equal(baseSource, result);
    }
}