using CommonFramework.Testing;

namespace CommonFramework.Tests;

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

    [CommonFact]
    public async Task ToImmutableDictionary_Source_KeyValue_WithComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, x => x, EqualityComparer<int>.Default, ct);

        result.Should().HaveCount(3);
        result[1].Should().Be("a");
        result[2].Should().Be("bb");
        result[3].Should().Be("ccc");
    }

    [CommonFact]
    public async Task ToImmutableDictionary_Source_KeyValue_WithoutComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, x => x, ct);

        result.Should().ContainKey(1).WhoseValue.Should().Be("a");
    }

    [CommonFact]
    public async Task ToImmutableDictionary_Source_KeyOnly_WithoutComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, ct);

        result[2].Should().Be("bb");
    }

    [CommonFact]
    public async Task ToImmutableDictionary_Source_KeyOnly_WithComparer(CancellationToken ct)
    {
        var result = await GetStrings()
            .ToImmutableDictionaryAsync(x => x.Length, EqualityComparer<int>.Default, ct);

        result[3].Should().Be("ccc");
    }

    [CommonFact]
    public async Task ToImmutableDictionary_KeyValuePair_WithoutComparer(CancellationToken ct)
    {
        var result = await GetPairs()
            .ToImmutableDictionaryAsync(ct);

        result.Should().HaveCount(2);
        result[1].Should().Be("a");
    }

    [CommonFact]
    public async Task ToImmutableDictionary_KeyValuePair_WithComparer(CancellationToken ct)
    {
        var result = await GetPairs()
            .ToImmutableDictionaryAsync(EqualityComparer<int>.Default, ct);

        result[2].Should().Be("b");
    }
}