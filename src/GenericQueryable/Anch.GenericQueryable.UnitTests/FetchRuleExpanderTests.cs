using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable.UnitTests;

public class FetchRuleExpanderTests
{
    [Fact]
    public void TryExpand_ReturnsNull_When_NoHeadersRegisteredForSourceType()
    {
        // Arrange
        var expander = new FetchRuleHeaderExpander([]);

        var fetchRuleHeader = new SomeFetchRuleHeader<Foo>();

        // Act
        var result = expander.TryExpand(fetchRuleHeader);

        // Assert
        Assert.Null(result);
    }

    private sealed record Foo;

    private sealed record SomeFetchRuleHeader<T> : FetchRuleHeader<T>
    {
    }
}