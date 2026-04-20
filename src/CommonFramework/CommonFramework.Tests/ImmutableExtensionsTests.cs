using CommonFramework.Testing;

namespace CommonFramework.Tests;

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

    [CommonFact]
    public async Task ToImmutableArrayAsync_ShouldReturnArray(CancellationToken ct)
    {
        var source = GetAsyncEnumerable(1, 2, 3);

        var result = await source.ToImmutableArrayAsync(ct);

        result.Should().Equal(1, 2, 3);
    }

    [CommonFact]
    public async Task ToImmutableListAsync_ShouldReturnList(CancellationToken ct)
    {
        var source = GetAsyncEnumerable(1, 2, 3);

        var result = await source.ToImmutableListAsync(ct);

        result.Should().Equal(1, 2, 3);
    }

    [CommonFact]
    public async Task ToImmutableHashSetAsync_ShouldReturnSet(CancellationToken ct)
    {
        var source = GetAsyncEnumerable(1, 2, 2, 3);

        var result = await source.ToImmutableHashSetAsync(ct);

        result.Should().BeEquivalentTo([1, 2, 3]);
    }

    [CommonFact]
    public async Task ToImmutableArrayAsync_Empty_ShouldReturnEmpty(CancellationToken ct)
    {
        var source = GetAsyncEnumerable();

        var result = await source.ToImmutableArrayAsync(ct);

        result.Should().BeEmpty();
    }

    [CommonFact]
    public async Task ToImmutableArrayAsync_NullSource_ShouldThrow(CancellationToken ct)
    {
        IAsyncEnumerable<int> source = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableArrayAsync(ct));
    }

    [CommonFact]
    public async Task ToImmutableListAsync_NullSource_ShouldThrow(CancellationToken ct)
    {
        IAsyncEnumerable<int> source = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableListAsync(ct));
    }

    [CommonFact]
    public async Task ToImmutableHashSetAsync_NullSource_ShouldThrow(CancellationToken ct)
    {
        IAsyncEnumerable<int> source = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableHashSetAsync(ct));
    }
}