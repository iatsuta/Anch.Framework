//namespace CommonFramework.Tests;

//public class AsyncEnumerableExtensionsTests
//{
//    private static async IAsyncEnumerable<string> GetStrings()
//    {
//        yield return "a";
//        yield return "bb";
//        yield return "ccc";
//        await Task.CompletedTask;
//    }

//    private static async IAsyncEnumerable<KeyValuePair<int, string>> GetPairs()
//    {
//        yield return new KeyValuePair<int, string>(1, "a");
//        yield return new KeyValuePair<int, string>(2, "b");
//        await Task.CompletedTask;
//    }

//    [Fact]
//    public async Task ToImmutableDictionary_Source_KeyValue_WithComparer()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var result = await GetStrings()
//            .ToImmutableDictionaryAsync(x => x.Length, x => x, EqualityComparer<int>.Default, cancellationToken);

//        result.Should().HaveCount(3);
//        result[1].Should().Be("a");
//        result[2].Should().Be("bb");
//        result[3].Should().Be("ccc");
//    }

//    [Fact]
//    public async Task ToImmutableDictionary_Source_KeyValue_WithoutComparer()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var result = await GetStrings()
//            .ToImmutableDictionaryAsync(x => x.Length, x => x, cancellationToken);

//        result.Should().ContainKey(1).WhoseValue.Should().Be("a");
//    }

//    [Fact]
//    public async Task ToImmutableDictionary_Source_KeyOnly_WithoutComparer()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var result = await GetStrings()
//            .ToImmutableDictionaryAsync(x => x.Length, cancellationToken);

//        result[2].Should().Be("bb");
//    }

//    [Fact]
//    public async Task ToImmutableDictionary_Source_KeyOnly_WithComparer()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var result = await GetStrings()
//            .ToImmutableDictionaryAsync(x => x.Length, EqualityComparer<int>.Default, cancellationToken);

//        result[3].Should().Be("ccc");
//    }

//    [Fact]
//    public async Task ToImmutableDictionary_KeyValuePair_WithoutComparer()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var result = await GetPairs()
//            .ToImmutableDictionaryAsync(cancellationToken);

//        result.Should().HaveCount(2);
//        result[1].Should().Be("a");
//    }

//    [Fact]
//    public async Task ToImmutableDictionary_KeyValuePair_WithComparer()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var result = await GetPairs()
//            .ToImmutableDictionaryAsync(EqualityComparer<int>.Default, cancellationToken);

//        result[2].Should().Be("b");
//    }
//}