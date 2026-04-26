using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class NewArrayComparer(ExpressionComparer rootComparer) : ExpressionComparer<NewArrayExpression>
{
	protected override bool PureEquals(NewArrayExpression x, NewArrayExpression y)
    {
        return x.Expressions.SequenceEqual(y.Expressions, rootComparer);
    }
}
