using Anch.Testing.Xunit;

namespace Anch.Testing.Tests;

public class TheoryTests(IServiceProvider serviceProvider)
{
    [Theory]
    [MemberData(nameof(GetTest2Cases))]
    public void TestSync(string value)
    {
    }

    [Theory]
    [AnchMemberData(nameof(GetTest1Cases))]
    public async Task Test1(ABC abc, CancellationToken ct)
    {
    }

    [Theory]
    [MemberData(nameof(GetTest2Cases))]
    public async Task Test2_1(string value, CancellationToken ct)
    {
    }

    [Theory]
    [AnchMemberData(nameof(GetTest2Cases))]
    public async Task Test2_2(string value, CancellationToken ct)
    {
    }

    [Theory]
    [AnchInlineData(123)]
    [AnchInlineData(234)]
    public async Task Test3_1(int value, CancellationToken ct)
    {
    }

    [Theory]
    [InlineData(345)]
    [InlineData(456)]
    public async Task Test3_2(int value)
    {
    }

    public IEnumerable<object?[]> GetTest1Cases()
    {
        yield return new object?[] { new ABC(123M) };
        yield return new object?[] { new ABC(234M) };
    }

    public static IEnumerable<object?[]> GetTest2Cases()
    {
        yield return new object?[] { "345" };
        yield return new object?[] { "567" };
    }

    public record ABC(decimal Value)
    {

    }
}