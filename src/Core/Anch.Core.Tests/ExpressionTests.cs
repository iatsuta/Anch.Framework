using System.Linq.Expressions;
using Anch.Core.ExpressionComparers;

namespace Anch.Core.Tests;

public class ExpressionTests
{
    public record A;

    public record B(A A);

    public record C(B B);

    [Fact]
    public void OverrideInput_ComposesExpressionThroughNewInput()
    {
        // Arrange
        Expression<Func<B, A>> expr = b => b.A;

        Expression<Func<C, A>> expectedResult = c => c.B.A;

        // Act
        var result = expr.OverrideInput((C c) => c.B);

        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }

    [Fact]
    public void UnCurrying_FlattensNestedLambdaIntoMultiParameterLambda()
    {
        // Arrange
        Expression<Func<string, Expression<Func<int, string>>>> expr = a => b => a + b;

        Expression<Func<string, int, string>> expectedResult = (a, b) => a + b;

        // Act
        var result = expr.UnCurrying();

        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }

    [Fact]
    public void Not_NegatesExpressionBody()
    {
        // Arrange
        Expression<Func<int, bool>> expr = v => v == 123;

        Expression<Func<int, bool>> expectedResult = v => !(v == 123);

        // Act
        var result = expr.Not();

        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }

    [Fact]
    public void ToEnumerableAny_WrapsPredicateWithAnyCall()
    {
        // Arrange
        Expression<Func<int, bool>> expr = v => v == 123;

        Expression<Func<IEnumerable<int>, bool>> expectedResult = source => source.Any(v => v == 123);

        // Act
        var result = expr.ToEnumerableAny("source");

        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }

    [Fact]
    public void ToCollectionFilter_WrapsPredicateWithWhereCall()
    {
        // Arrange
        Expression<Func<int, bool>> expr = v => v == 123;

        Expression<Func<IEnumerable<int>, IEnumerable<int>>> expectedResult = source => source.Where(v => v == 123);

        // Act
        var result = expr.ToCollectionFilter("source");

        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }
}