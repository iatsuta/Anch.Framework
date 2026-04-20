//namespace CommonFramework.Tests;

//public class ImmutableExtensionsTests
//{
//    private async IAsyncEnumerable<int> GetAsyncEnumerable(params int[] items)
//    {
//        foreach (var item in items)
//        {
//            yield return item;
//            await Task.Yield();
//        }
//    }

//    [Fact]
//    public async Task ToImmutableArrayAsync_ShouldReturnArray()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var source = GetAsyncEnumerable(1, 2, 3);

//        var result = await source.ToImmutableArrayAsync(cancellationToken);

//        result.Should().Equal(1, 2, 3);
//    }

//    [Fact]
//    public async Task ToImmutableListAsync_ShouldReturnList()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var source = GetAsyncEnumerable(1, 2, 3);

//        var result = await source.ToImmutableListAsync(cancellationToken);

//        result.Should().Equal(1, 2, 3);
//    }

//    [Fact]
//    public async Task ToImmutableHashSetAsync_ShouldReturnSet()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var source = GetAsyncEnumerable(1, 2, 2, 3);

//        var result = await source.ToImmutableHashSetAsync(cancellationToken);

//        result.Should().BeEquivalentTo([1, 2, 3]);
//    }

//    [Fact]
//    public async Task ToImmutableArrayAsync_Empty_ShouldReturnEmpty()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        var source = GetAsyncEnumerable();

//        var result = await source.ToImmutableArrayAsync(cancellationToken);

//        result.Should().BeEmpty();
//    }

//    [Fact]
//    public async Task ToImmutableArrayAsync_NullSource_ShouldThrow()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        IAsyncEnumerable<int> source = null!;
//        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableArrayAsync(cancellationToken));
//    }

//    [Fact]
//    public async Task ToImmutableListAsync_NullSource_ShouldThrow()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        IAsyncEnumerable<int> source = null!;
//        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableListAsync(cancellationToken));
//    }

//    [Fact]
//    public async Task ToImmutableHashSetAsync_NullSource_ShouldThrow()
//    {
//        var cancellationToken = TestContext.Current.CancellationToken;

//        IAsyncEnumerable<int> source = null!;
//        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.ToImmutableHashSetAsync(cancellationToken));
//    }
//}