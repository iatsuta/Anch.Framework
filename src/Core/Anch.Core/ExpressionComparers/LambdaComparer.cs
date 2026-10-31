using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of lambda expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
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