using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of member access expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class MemberComparer(ExpressionComparer rootComparer) : ExpressionComparer<MemberExpression>
{
    protected override bool PureEquals(MemberExpression x, MemberExpression y)
    {
        return x.Member == y.Member && rootComparer.Equals(x.Expression, y.Expression);
    }

    public override int GetHashCode(MemberExpression obj)
    {
        return base.GetHashCode(obj) ^ obj.Member.GetHashCode();
    }
}