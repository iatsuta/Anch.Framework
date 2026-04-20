using CommonFramework.Testing;

[assembly: CommonTestFramework]

namespace GenericQueryable.UnitTests;

public class MainTests
{
    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeSumAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new decimal?[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericSumAsync(ct);

        //Assert
        result.Should().Be(baseSource.Sum());
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToArrayAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToArrayAsync(ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToListAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToListAsync(ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToHashSetAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToHashSetAsync(ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToHashSetWithComparerAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToHashSetAsync(EqualityComparer<int>.Default, ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource);
    }


    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource.ToDictionary(v => v));
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryWithComparerAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, EqualityComparer<int>.Default, ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource.ToDictionary(v => v));
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryWithElementSelectorAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, v => v, ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource.ToDictionary(v => v));
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeToDictionaryWithElementSelectorAndComparerAsync_MethodInvoked(CancellationToken ct)
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = await qSource.GenericToDictionaryAsync(v => v, v => v, EqualityComparer<int>.Default, ct);

        //Assert
        result.Should().BeEquivalentTo(baseSource.ToDictionary(v => v));
    }

    [CommonFact]
    public void DefaultGenericQueryable_InvokeToList_MethodInvoked()
    {
        // Arrange
        var baseSource = new[] { 1, 2, 3 };
        var qSource = baseSource.AsQueryable();

        // Act
        var result = qSource.ToList();

        //Assert
        result.Should().BeEquivalentTo(baseSource);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeSingleOrDefaultAsync_CollisionResolved(CancellationToken ct)
    {
        // Arrange
        var baseSource = 1;
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.GenericSingleOrDefaultAsync(_ => true, ct);

        //Assert
        result.Should().Be(baseSource);
    }

    [CommonFact]
    public async Task GenericAsAsyncEnumerable_Should_Execute(CancellationToken ct)
    {
        // Arrange
        var baseSource = 1;
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.GenericAsAsyncEnumerable().SingleAsync(ct);

        //Assert
        result.Should().Be(baseSource);
    }

    [CommonFact]
    public async Task DefaultGenericQueryable_InvokeFetch_FetchIgnored(CancellationToken ct)
    {
        // Arrange
        var baseSource = "abc";
        var qSource = new[] { baseSource }.AsQueryable();

        // Act
        var result = await qSource.WithFetch(nameof(string.Length))
            .GenericSingleOrDefaultAsync(_ => true, ct);

        //Assert
        result.Should().Be(baseSource);
    }
}