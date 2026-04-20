//namespace CommonFramework.Tests;

//public class DictionaryTests
//{
//    [Fact]
//    public void ToImmutableDictionary_ShouldPreserveAllKeyValuePairs()
//    {
//        // Arrange
//        var source = Enumerable.Range(0, 10).Select(v => (v, v)).ToArray();

//        // Act
//        var dict = source.ToImmutableDictionary();

//        // Assert
//        source.OrderBy(pair => pair.Item1).Should().BeEquivalentTo(dict.Select(pair => (pair.Key, pair.Value)).OrderBy(pair => pair.Key));
//    }
//}