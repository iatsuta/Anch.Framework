using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class MethodCallComparer(ExpressionComparer rootComparer) : ExpressionComparer<MethodCallExpression>
{
	protected override bool PureEquals(MethodCallExpression x, MethodCallExpression y)
	{
		return x.Method == y.Method
		       && rootComparer.Equals(x.Object, y.Object)
		       && x.Arguments.SequenceEqual(y.Arguments, rootComparer);
	}

	public override int GetHashCode(MethodCallExpression obj)
	{
		return base.GetHashCode(obj) ^ obj.Arguments.Count;
	}
}