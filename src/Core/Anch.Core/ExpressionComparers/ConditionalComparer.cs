using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of conditional expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class ConditionalComparer(ExpressionComparer rootComparer) : ExpressionComparer<ConditionalExpression>
{
    protected override bool PureEquals(ConditionalExpression x, ConditionalExpression y)
    {
        return rootComparer.Equals(x.Test, y.Test)
               && rootComparer.Equals(x.IfTrue, y.IfTrue)
               && rootComparer.Equals(x.IfFalse, y.IfFalse);
    }
}