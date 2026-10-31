using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of member initialization expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class MemberInitComparer(ExpressionComparer rootComparer) : ExpressionComparer<MemberInitExpression>
{
    private readonly MemberBindingComparer memberBindingComparer = new(rootComparer);

    protected override bool PureEquals(MemberInitExpression x, MemberInitExpression y)
    {
        return rootComparer.Equals(x.NewExpression, y.NewExpression) && x.Bindings.SequenceEqual(y.Bindings, this.memberBindingComparer);
    }
}