namespace Anch.Core.Tests;

public class AsyncEnumerableExtensionsTests
{
    private static async IAsyncEnumerable<string> GetStrings()
    {
        yield return "a";
        yield return "bb";
        yield return "ccc";
        await Task.CompletedTask;
    }

    private static async IAsyncEnumerable<KeyValuePair<int, string>> GetPairs()
    {
        yield return new KeyValuePair<int, string>(1, "a");
        yield return new KeyValuePair<int, string>(2, "b");
        await Task.CompletedTask;
    }

    [AnchFact]
    public async Task ToImmutableDictionary_Source_KeyValue_WithComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, x => x, EqualityComparer<int>.Default, ct);

        Assert.Equal(3, result.Count);
        Assert.Equal("a", result[1]);
        Assert.Equal("bb", result[2]);
        Assert.Equal("ccc", result[3]);
    }

    [AnchFact]
    public async Task ToImmutableDictionary_Source_KeyValue_WithoutComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, x => x, ct);

        Assert.True(result.ContainsKey(1));
        Assert.Equal("a", result[1]);
    }

    [AnchFact]
    public async Task ToImmutableDictionary_Source_KeyOnly_WithoutComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, ct);

        Assert.Equal("bb", result[2]);
    }

    [AnchFact]
    public async Task ToImmutableDictionary_Source_KeyOnly_WithComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, EqualityComparer<int>.Default, ct);

        Assert.Equal("ccc", result[3]);
    }

    [AnchFact]
    public async Task ToImmutableDictionary_KeyValuePair_WithoutComparer(CancellationToken ct)
    {
        var result = await GetPairs()
            .ToImmutableDictionaryAsync(ct);

        Assert.Equal(2, result.Count);
        Assert.Equal("a", result[1]);
    }

    [AnchFact]
    public async Task ToImmutableDictionary_KeyValuePair_WithComparer(CancellationToken ct)
    {
        var result = await GetPairs()
            .ToImmutableDictionaryAsync(EqualityComparer<int>.Default, ct);

        Assert.Equal("b", result[2]);
    }
}