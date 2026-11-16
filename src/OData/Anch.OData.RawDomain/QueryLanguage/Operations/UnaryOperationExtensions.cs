using System.Linq.Expressions;

namespace Anch.OData.Domain.QueryLanguage.Operations;

/// <summary>Extensions for <see cref="UnaryOperation"/>.</summary>
public static class UnaryOperationExtensions
{
    /// <summary>Returns the operator as written in the text form of the tree.</summary>
    /// <param name="operation">The operation.</param>
    public static string ToFormatString(this UnaryOperation operation) =>
        operation switch
        {
            UnaryOperation.Not => "!",
            UnaryOperation.Plus => "+",
            UnaryOperation.Negate => "-",
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

    /// <summary>Returns the matching LINQ node type.</summary>
    /// <param name="operation">The operation.</param>
    public static ExpressionType ToExpressionType(this UnaryOperation operation) =>
        operation switch
        {
            UnaryOperation.Not => ExpressionType.Not,
            UnaryOperation.Plus => ExpressionType.UnaryPlus,
            UnaryOperation.Negate => ExpressionType.Negate,
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

    /// <summary>Returns the precedence of the operation: a larger number binds tighter.</summary>
    /// <param name="operation">The operation.</param>
    public static int GetPriority(this UnaryOperation operation) =>
        operation switch
        {
            UnaryOperation.Not => 10,
            UnaryOperation.Negate => 20,
            UnaryOperation.Plus => 20,
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
}
