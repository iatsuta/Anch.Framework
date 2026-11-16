using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of unary expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class UnaryComparer(ExpressionComparer rootComparer) : ExpressionComparer<UnaryExpression>
{
    protected override bool PureEquals(UnaryExpression x, UnaryExpression y)
    {
        return x.Method == y.Method
               && rootComparer.Equals(x.Operand, y.Operand);
    }
}
