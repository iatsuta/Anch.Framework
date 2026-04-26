using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class ConstantComparer : ExpressionComparer<ConstantExpression>
{
	protected override bool PureEquals(ConstantExpression x, ConstantExpression y)
    {
        return Equals(x.Value, y.Value);
    }
}
