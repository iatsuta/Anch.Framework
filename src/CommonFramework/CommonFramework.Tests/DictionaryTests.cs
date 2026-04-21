namespace CommonFramework.Tests;

public class DictionaryTests
{
    [Fact]
    public void ToImmutableDictionary_ShouldPreserveAllKeyValuePairs()
    {
        // Arrange
        var source = Enumerable.Range(0, 10).Select(v => (v, v)).ToArray();

        // Act
        var dict = source.ToImmutableDictionary();

        // Assert
        Assert.Equivalent(source.OrderBy(pair => pair.Item1), dict.Select(pair => (pair.Key, pair.Value)).OrderBy(pair => pair.Key));
    }
}