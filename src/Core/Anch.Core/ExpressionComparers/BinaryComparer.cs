using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class BinaryComparer(ExpressionComparer rootComparer) : ExpressionComparer<BinaryExpression>
{
	protected override bool PureEquals(BinaryExpression x, BinaryExpression y)
    {
        return x.Method == y.Method
               && rootComparer.Equals(x.Left, y.Left)
               && rootComparer.Equals(x.Right, y.Right);
    }
}
