using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of method call expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
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