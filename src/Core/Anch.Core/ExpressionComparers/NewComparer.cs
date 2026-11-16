using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of object creation expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class NewComparer(ExpressionComparer rootComparer) : ExpressionComparer<NewExpression>
{
    protected override bool PureEquals(NewExpression x, NewExpression y)
    {
        return x.Arguments.SequenceEqual(y.Arguments, rootComparer)

               && ((x.Members is null && y.Members is null)

                   || (x.Members is not null && y.Members is not null && x.Members.SequenceEqual(y.Members)));
    }
}