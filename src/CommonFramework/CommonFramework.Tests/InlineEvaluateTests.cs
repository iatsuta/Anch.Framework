using System.Linq.Expressions;

using CommonFramework.ExpressionComparers;
using CommonFramework.ExpressionEvaluate;

namespace CommonFramework.Tests;

public class InlineEvaluateTests
{
    [Fact]
    public void InlineEvaluate_WhenSecondParameterIsConstant_RewritesToSimplifiedUnaryExpression()
    {
        // Arrange
        Expression<Func<int, int, int>> testExpression = (x, y) => x + y;

        Expression<Func<int, int>> expectedResult = x => x + 1;

        // Act
        var result = ExpressionEvaluateHelper.InlineEvaluate(Expression<Func<int, int>> (ee) => x => ee.Evaluate(testExpression, x, 1));

        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }

    [Fact]
    public void InlineEvaluate_EmployeeBusinessUnitIdPath_TransformsToExpectedEnumerableExpression()
    {
        // Arrange
        Expression<Func<Employee, BusinessUnit?>> singlePath = employee => employee.BusinessUnit;
        Expression<Func<BusinessUnit, Guid>> idPath = bu => bu.Id;

        Expression<Func<Employee, IEnumerable<Guid>>> expectedResult = employee =>
            employee.BusinessUnit != null ? new[] { employee.BusinessUnit.Id } : Array.Empty<Guid>();

        // Act
        var result =
            ExpressionEvaluateHelper.InlineEvaluate(ee =>
            {
                return singlePath.Select(IEnumerable<Guid> (securityContext) =>
                    securityContext != null ? new[] { ee.Evaluate(idPath, securityContext) } : Array.Empty<Guid>());
            });


        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }

    [Fact]
    public void InlineEvaluate_ReplacesCompileWithExpressionInlining()
    {
        // Arrange
        Expression<Func<int, int>> testExpression = x => x + 1;

        Expression<Func<IEnumerable<int>, IEnumerable<int>>> expectedResult = stream => stream.Select(x => x + 1);

        // Act
        var result = ExpressionEvaluateHelper.InlineEvaluate(Expression<Func<IEnumerable<int>, IEnumerable<int>>> (ee) =>
            stream => stream.Select(ee.Compile(testExpression)));

        // Assert
        Assert.Equal(expectedResult, result, ExpressionComparer.Default);
    }

    public class BusinessUnit
    {
        public Guid Id { get; set; }
    }

    public class Employee
    {
        public BusinessUnit? BusinessUnit { get; set; }
    }
}