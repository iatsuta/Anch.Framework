namespace Anch.Testing.Tests;

public class SimpleTheoryTests(IServiceProvider serviceProvider)
{
    [Theory]
    [MemberData(nameof(GetTest2Cases))]
    public void TestSync(string value)
    {
    }

    public static IEnumerable<object?[]> GetTest2Cases()
    {
        yield return new object?[] { "345" };
        yield return new object?[] { "567" };
    }
}