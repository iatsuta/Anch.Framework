using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of binary expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class BinaryComparer(ExpressionComparer rootComparer) : ExpressionComparer<BinaryExpression>
{
    protected override bool PureEquals(BinaryExpression x, BinaryExpression y)
    {
        return x.Method == y.Method
               && rootComparer.Equals(x.Left, y.Left)
               && rootComparer.Equals(x.Right, y.Right);
    }
}
