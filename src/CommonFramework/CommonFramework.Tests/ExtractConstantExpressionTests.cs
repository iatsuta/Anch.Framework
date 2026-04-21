using System.Linq.Expressions;

namespace CommonFramework.Tests;

public class ExtractConstantExpressionTests
{
    private readonly FakeRefClass fakeRef = null!;

    public record FakeRefClass(int Value);

    private static class InnerClass1
    {
        public static class InnerClass2
        {
            public static int? Value { get; } = 123;
        }
    }

    private static readonly Tuple<int> StaticValue1 = new(123);

    [Fact]
    public void Should_ExtractConstant_FromLocalVariable()
    {
        // Arrange
        var val = 123;

        Expression<Func<int>> expr = () => val;

        // Act
        var result = expr.Body.GetConstantValue<int>();

        // Assert
        Assert.Equal(Maybe.Return(val), result);
    }

    [Fact]
    public void Should_ExtractConstant_FromAnonymousTypeProperty()
    {
        // Arrange
        var val = new { InnerVal = 123 };

        Expression<Func<int>> expr = () => val.InnerVal;

        // Act
        var result = expr.Body.GetConstantValue<int>();

        // Assert
        Assert.Equal(Maybe.Return(val.InnerVal), result);
    }

    [Fact]
    public void Should_ExtractConstant_FromStaticFieldMemberAccess()
    {
        // Arrange
        Expression<Func<int>> expr = () => StaticValue1.Item1;

        // Act
        var result = expr.Body.GetConstantValue<int>();

        // Assert
        Assert.Equal(Maybe.Return(StaticValue1.Item1), result);
    }

    [Fact]
    public void Should_ExtractConstant_String()
    {
        // Arrange
        var val = "abc";

        Expression<Func<string>> expr = () => val;

        // Act
        var result = expr.Body.GetConstantValue<string>();

        // Assert
        Assert.Equal(Maybe.Return(val), result);
    }

    [Fact]
    public void Should_ExtractConstant_FromNestedStaticProperty()
    {
        // Arrange
        Expression<Func<int?>> expr = () => InnerClass1.InnerClass2.Value;

        // Act
        var result = expr.Body.GetConstantValue<int?>();

        // Assert
        Assert.Equal(Maybe.Return(InnerClass1.InnerClass2.Value), result);
    }

    [Fact]
    public void Should_ReturnNothing_ForInstanceMemberAccess()
    {
        // Arrange
        Expression<Func<int>> expr = () => this.fakeRef.Value;

        // Act
        var result = expr.Body.GetConstantValue<int>();

        // Assert
        Assert.Equal(Maybe<int>.Nothing, result);
    }
}