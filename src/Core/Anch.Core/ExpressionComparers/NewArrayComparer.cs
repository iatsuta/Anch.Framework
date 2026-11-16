using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of array creation expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class NewArrayComparer(ExpressionComparer rootComparer) : ExpressionComparer<NewArrayExpression>
{
    protected override bool PureEquals(NewArrayExpression x, NewArrayExpression y)
    {
        return x.Expressions.SequenceEqual(y.Expressions, rootComparer);
    }
}
