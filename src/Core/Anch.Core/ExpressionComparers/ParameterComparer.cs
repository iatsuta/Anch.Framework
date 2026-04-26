using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class ParameterComparer : ExpressionComparer<ParameterExpression>
{
	protected override bool PureEquals(ParameterExpression x, ParameterExpression y)
	{
		return x.Name == y.Name;
	}
}