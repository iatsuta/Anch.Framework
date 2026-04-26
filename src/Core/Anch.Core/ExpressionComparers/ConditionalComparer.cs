using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class ConditionalComparer(ExpressionComparer rootComparer) : ExpressionComparer<ConditionalExpression>
{
	protected override bool PureEquals(ConditionalExpression x, ConditionalExpression y)
    {
        return rootComparer.Equals(x.Test, y.Test)
               && rootComparer.Equals(x.IfTrue, y.IfTrue)
               && rootComparer.Equals(x.IfFalse, y.IfFalse);
    }
}