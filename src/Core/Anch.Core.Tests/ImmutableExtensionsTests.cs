namespace Anch.Core.Tests;

public class ImmutableExtensionsTests
{
    private async IAsyncEnumerable<int> GetAsyncEnumerable(params int[] items)
    {
        foreach (var item in items)
        {
            yield return item;
            await Task.Yield();
        }
    }

    [AnchFact]
    public async Task ToImmutableArrayAsync_ShouldReturnArray(CancellationToken ct)
    {
        var source = this.GetAsyncEnumerable(1, 2, 3);

        var result = await source.ToImmutableArrayAsync(ct);

        Assert.Equal([1, 2, 3], result);
    }

    [AnchFact]
    public async Task ToImmutableListAsync_ShouldReturnList(CancellationToken ct)
    {
        var source = this.GetAsyncEnumerable(1, 2, 3);

        var result = await source.ToImmutableListAsync(ct);

        Assert.Equal([1, 2, 3], result);
    }

    [AnchFact]
    public async Task ToImmutableHashSetAsync_ShouldReturnSet(CancellationToken ct)
    {
        var source = this.GetAsyncEnumerable(1, 2, 2, 3);

        var result = await source.ToImmutableHashSetAsync(ct);

        Assert.Equivalent(new[] { 1, 2, 3 }, result);
    }

    [AnchFact]
    public async Task ToImmutableArrayAsync_Empty_ShouldReturnEmpty(CancellationToken ct)
    {
        var source = this.GetAsyncEnumerable();

        var result = await source.ToImmutableArrayAsync(ct);

        Assert.Empty(result);
    }

    [AnchFact]
    public async Task ToImmutableArrayAsync_NullSource_ShouldThrow(CancellationToken ct)
    {
        IAsyncEnumerable<int> source = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableArrayAsync(ct));
    }

    [AnchFact]
    public async Task ToImmutableListAsync_NullSource_ShouldThrow(CancellationToken ct)
    {
        IAsyncEnumerable<int> source = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableListAsync(ct));
    }

    [AnchFact]
    public async Task ToImmutableHashSetAsync_NullSource_ShouldThrow(CancellationToken ct)
    {
        IAsyncEnumerable<int> source = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableHashSetAsync(ct));
    }
}