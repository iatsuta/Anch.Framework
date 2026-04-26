using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class LambdaComparer(ExpressionComparer rootComparer) : ExpressionComparer<LambdaExpression>
{
	protected override bool PureEquals(LambdaExpression x, LambdaExpression y)
	{
		return x.Parameters.SequenceEqual(y.Parameters, rootComparer.ParameterComparer) && rootComparer.Equals(x.Body, y.Body);
	}

	public override int GetHashCode(LambdaExpression obj)
	{
		return base.GetHashCode(obj) ^ obj.Parameters.Count;
	}
}